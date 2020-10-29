using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using MqttListener.Configuration;
using MqttListener.ViewModels;
using MQTTnet;
using MQTTnet.Client.Options;
using MQTTnet.Extensions.ManagedClient;

namespace MqttListener.Core
{
    public class Listener : BaseViewModel
    {
        private const string _rootTopicItemName = "Root";
        private readonly IManagedMqttClient _mqttClient = new MqttFactory().CreateManagedMqttClient();
        private readonly IServiceProvider _serviceProvider;
        private IWritableOptions<AppConfiguration> _appConfigurationOptions;
        private string _connectionName;
        private bool _isConnected;
        private string _lastMessage;
        private string _lastTopic;
        private IList<TopicItem> _root;

        public Listener(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            Root = new[] { new TopicItem(_rootTopicItemName, null) }.ToList();
            _appConfigurationOptions = _serviceProvider.GetService<IWritableOptions<AppConfiguration>>();
        }

        public event EventHandler OnConnectedEventHandler;

        public event EventHandler OnDisconnectedEventHandler;

        public string ConnectionName
        {
            get => _connectionName;
            private set => SetProperty(ref _connectionName, value);
        }

        public bool IsConnected
        {
            get => _isConnected;
            private set
            {
                if (SetProperty(ref _isConnected, value))
                {
                    if (value)
                    {
                        OnConnectedEventHandler?.Invoke(this, EventArgs.Empty);
                    }
                    else
                    {
                        OnDisconnectedEventHandler?.Invoke(this, EventArgs.Empty);
                    }
                }
            }
        }

        public string LastMessage
        {
            get { return _lastMessage; }
            private set { SetProperty(ref _lastMessage, value); }
        }

        public string LastTopic
        {
            get { return _lastTopic; }
            private set { SetProperty(ref _lastTopic, value); }
        }

        public IList<TopicItem> Root
        {
            get => _root;
            private set => SetProperty(ref _root, value);
        }

        public async Task ConnectAction(ConnectionItem connectionItem, CancellationToken token)
        {
            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();

            try
            {
                await Task.Delay(300, token);

                token.ThrowIfCancellationRequested();

                if (!int.TryParse(connectionItem.Port, out int port))
                {
                    port = 1883;
                }

                byte.TryParse(connectionItem.Qos, out byte qos);

                var options = new ManagedMqttClientOptionsBuilder()
                    .WithClientOptions(new MqttClientOptionsBuilder()
                        .WithClientId(_appConfigurationOptions.Value.ClientId)
                        .WithTcpServer(connectionItem.Host, port)
                        .WithCredentials(connectionItem.Username, /*connectionItem.Password*/"mqtt")
                        .Build())
                    .WithAutoReconnectDelay(TimeSpan.MaxValue)
                    .Build();

                MqttTopicFilter[] topicFilters = (connectionItem.Topics ?? Enumerable.Empty<string>())
                    .Select(x => new MqttTopicFilterBuilder().WithTopic(x).Build())
                    .ToArray();

                _mqttClient.UseConnectedHandler(x =>
                {
                    IsConnected = true;
                    ConnectionName = connectionItem.ConnectionName;
                    if (!tcs.Task.IsCompleted)
                    {
                        tcs.SetResult(true);
                    }
                });
                _mqttClient.ConnectingFailedHandler = new ConnectingFailedHandlerDelegate(x =>
                {
                    IsConnected = false;
                    _mqttClient.StopAsync();
                    if (!tcs.Task.IsCompleted)
                    {
                        tcs.SetException(new Exception("Can not instantiate connection.", x.Exception));
                    }
                });

                await _mqttClient.SubscribeAsync(topicFilters);
                await _mqttClient.StartAsync(options);

                _mqttClient.UseApplicationMessageReceivedHandler(MqttClientReceived);

                if (token.IsCancellationRequested)
                {
                    Disconnect();
                    if (!tcs.Task.IsCompleted)
                    {
                        tcs.SetCanceled();
                    }
                }
            }
            catch (Exception e)
            {
                tcs.SetException(new Exception("Can not instantiate connection.", e));
            }

            await tcs.Task;
        }

        public void Disconnect()
        {
            if (_mqttClient != null)
            {
                try
                {
                    _mqttClient?.StopAsync();
                }
                catch
                {
                }
                finally
                {
                    IsConnected = false;
                    LastTopic = null;
                    LastMessage = null;
                }

                Root = new[] { new TopicItem(_rootTopicItemName, null) }.ToList();
            }
        }

        private void InsertTopic(TopicItem topicItem, TopicItem root)
        {
            TopicItem item = root.Child.FirstOrDefault(x => x.Name == topicItem.Name);
            if (item != null && topicItem.Child.Count == 0)
            {
                item.Message = topicItem.Message;
                return;
            }

            if (item != null)
            {
                InsertTopic(topicItem.Child[0], item);
            }
            else
            {
                Application.Current.Dispatcher.Invoke(() => { root.Child.Add(topicItem); });
            }
        }

        private void MqttClientReceived(MqttApplicationMessageReceivedEventArgs arg)
        {
            string message = arg.ApplicationMessage.Payload == null ? null : Encoding.UTF8.GetString(arg.ApplicationMessage.Payload);
            TopicItem topicItem = ParseTopic(arg.ApplicationMessage.Topic, message);
            InsertTopic(topicItem, Root[0]);

            LastTopic = arg.ApplicationMessage.Topic;
            LastMessage = message;
        }

        private TopicItem ParseTopic(string eTopic, string message)
        {
            string[] topicNames = eTopic.Split('/');
            if (topicNames.Length == 0)
                return null;

            TopicItem topicItem = new TopicItem(topicNames[0], null);
            TopicItem currect = topicItem;
            foreach (string name in topicNames.Skip(1))
            {
                TopicItem item = new TopicItem(name, currect);
                currect.Child.Add(item);
                currect = item;
            }

            currect.Message = message;

            return topicItem;
        }
    }
}