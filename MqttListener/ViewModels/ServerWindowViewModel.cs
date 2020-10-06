using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MqttListener.Configuration;
using uPLibrary.Networking.M2Mqtt;

namespace MqttListener.ViewModels
{
    public class ServerWindowViewModel : BaseViewModel
    {
        private readonly IServiceProvider _serviceProvider;
        private ICommand _connectCommand;
        private ConnectViewModel _connectViewModel;
        private ICommand _disconnectCommand;
        private bool _isConnected;
        private string _lastMessage;
        private string _lastTopic;
        private MqttClient mqttClient;

        public ServerWindowViewModel(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            Root = new[] { new TopicItem("Root") }.ToList();

            Configuration = _serviceProvider.GetService<IOptions<AppConfiguration>>().Value;

            ConnectViewModel = new ConnectViewModel(_serviceProvider);
            ConnectViewModel.Initialize(ConnectAction);
        }

        public AppConfiguration Configuration { get; private set; }

        public ICommand ConnectCommand => _connectCommand ??= new RelayCommand(o => Connect(), x => !IsConnected);

        public ConnectViewModel ConnectViewModel
        {
            get => _connectViewModel;
            set => SetProperty(ref _connectViewModel, value);
        }

        public ICommand DisconnectCommand => _disconnectCommand ??= new RelayCommand(o => Disconnect(), x => IsConnected);

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

        private void Connect()
        {
            ConnectViewModel = new ConnectViewModel(_serviceProvider);
            ConnectViewModel.Initialize(ConnectAction);
        }

        private void ConnectAction(ConnectionItem connectionItem)
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
            if (IsConnected)
            {
                ConnectViewModel = null;
            }

            if (!IsConnected)
            {
                mqttClient.MqttMsgPublishReceived -= MqttClient_MqttMsgPublishReceived;
                mqttClient = null;

                throw new Exception("Can not instantiate connection.");
            }
        }

        private void Disconnect()
        {
            if (!mqttClient.IsConnected)
                return;

            mqttClient.Disconnect();
            mqttClient.MqttMsgPublishReceived -= MqttClient_MqttMsgPublishReceived;
            mqttClient = null;
            IsConnected = false;

            Configuration = _serviceProvider.GetService<IOptions<AppConfiguration>>().Value;

            Connect();
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