using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MqttListener.Configuration;
using MqttListener.Interfaces;
using uPLibrary.Networking.M2Mqtt;

namespace MqttListener.ViewModels
{
    public class ServerViewModel : BaseViewModel
    {
        private readonly IDialogHost _dialogHost = null;
        private readonly IServiceProvider _serviceProvider;
        private RelayCommand _disconnectCommand;
        private bool _isConnected;
        private string _lastMessage;
        private string _lastTopic;
        private MqttClient mqttClient;

        public ServerViewModel(IServiceProvider serviceProvider, IDialogHost dialogHost)
        {
            _serviceProvider = serviceProvider;

            Root = new[] { new TopicItem("Root") }.ToList();
            Configuration = _serviceProvider.GetService<IOptions<AppConfiguration>>().Value;

            _dialogHost = dialogHost;
            _dialogHost.Show(new OpenConnectionDialogViewModel(serviceProvider, ConnectAction), OpenOkAction, CancelAction);
        }

        public AppConfiguration Configuration { get; private set; }

        public ICommand DisconnectCommand => _disconnectCommand ??= new RelayCommand(x => Disconnect());

        private void Disconnect()
        {
            if (mqttClient.IsConnected)
            {
                mqttClient.Disconnect();
                mqttClient.MqttMsgPublishReceived -= MqttClient_MqttMsgPublishReceived;
                mqttClient = null;
                IsConnected = false;

                Configuration = _serviceProvider.GetService<IOptions<AppConfiguration>>().Value;

                _dialogHost.Show(new OpenConnectionDialogViewModel(_serviceProvider, ConnectAction), OpenOkAction, CancelAction);
            }
        }

        public bool IsConnected
        {
            get => _isConnected;
            private set => SetProperty(ref _isConnected, value);
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

        public IList<TopicItem> Root { get; }

        private void CancelAction()
        {
        }

        private void ConnectAction(ConnectionItem connectionItem, CancellationToken token)
        {
            if (!int.TryParse(connectionItem.Port, out int port))
            {
                port = 1883;
            }

            byte.TryParse(connectionItem.Qos, out byte qos);

            mqttClient = new MqttClient(connectionItem.Host, port, false, null, null, MqttSslProtocols.None);

            mqttClient.Connect(Configuration.ClientId, connectionItem.Username, connectionItem.Password);
            mqttClient.Subscribe(new string[] { connectionItem.Topic }, new byte[] { qos });
            mqttClient.MqttMsgPublishReceived += MqttClient_MqttMsgPublishReceived;

            IsConnected = mqttClient.IsConnected;

            if (!IsConnected)
            {
                mqttClient.MqttMsgPublishReceived -= MqttClient_MqttMsgPublishReceived;
                mqttClient = null;

                throw new Exception("Can not instantiate connection.");
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

        private void MqttClient_MqttMsgPublishReceived(object sender, uPLibrary.Networking.M2Mqtt.Messages.MqttMsgPublishEventArgs e)
        {
            string message = Encoding.UTF8.GetString(e.Message);
            TopicItem topicItem = ParseTopic(e.Topic, message);
            InsertTopic(topicItem, Root[0]);

            LastTopic = e.Topic;
            LastMessage = message;
        }

        private void OpenOkAction()
        {
        }

        private TopicItem ParseTopic(string eTopic, string message)
        {
            string[] topicNames = eTopic.Split('/');
            if (topicNames.Length == 0)
                return null;

            TopicItem topicItem = new TopicItem(topicNames[0]);
            TopicItem currect = topicItem;
            foreach (string name in topicNames.Skip(1))
            {
                TopicItem item = new TopicItem(name);
                currect.Child.Add(item);
                currect = item;
            }

            currect.Message = message;

            return topicItem;
        }
    }
}