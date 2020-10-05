using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace MqttListener.ViewModels
{
    public class MainWindowViewModel : BaseViewModel
    {
        private readonly string _clientId = "MqttListener";
        private readonly MqttClient mqttClient;
        private ICommand _connectCommand;
        private ICommand _disconnectCommand;
        private bool _isConnected;
        private string _lastTopic;
        private string _lastMessage;
        private ICommand _setTopicsCommand;

        public MainWindowViewModel()
        {
            Root = new[] { new TopicItem("Root") }.ToList();
            mqttClient = new MqttClient("192.168.0.111");
        }

        public ICommand ConnectCommand => _connectCommand ?? (_connectCommand = new RelayCommand(o => Connect(), x => !IsConnected));

        public ICommand DisconnectCommand => _disconnectCommand ?? (_disconnectCommand = new RelayCommand(o => Disconnect(), x => IsConnected));

        public bool IsConnected
        {
            get => _isConnected;
            private set => SetProperty(ref _isConnected, value);
        }

        public string LastTopic
        {
            get { return _lastTopic; }
            private set { SetProperty(ref _lastTopic, value); }
        }
        public string LastMessage
        {
            get { return _lastMessage; }
            private set { SetProperty(ref _lastMessage, value); }
        }

        public IList<TopicItem> Root { get; }

        public ICommand SetTopicsCommand => _setTopicsCommand ?? (_setTopicsCommand = new RelayCommand(o => SetTopics()));

        private void Connect()
        {
            mqttClient.Connect(_clientId, "mqtt", "mqtt");
            mqttClient.Subscribe(new string[] { "#" }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
            mqttClient.MqttMsgPublishReceived += MqttClient_MqttMsgPublishReceived;

            IsConnected = true;
        }

        private void Disconnect()
        {
            if (!mqttClient.IsConnected)
                return;

            mqttClient.Disconnect();
            IsConnected = false;
        }

        private void InsertTopic(TopicItem topicItem, TopicItem root)
        {
            TopicItem item = root.Child.FirstOrDefault(x => x.Name == topicItem.Name);
            if (item != null && topicItem.Child.Count == 0)
            {
                // item exists
                // update value
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

        private void SetTopics()
        {
            TopicItem topicItem1 = new TopicItem("testTopic");
            topicItem1.Child.Add(new TopicItem("sub topic 1-1"));
            topicItem1.Child.Add(new TopicItem("sub topic 1-2"));
            TopicItem topicItem2 = new TopicItem("testTopic 2");
            topicItem2.Child.Add(new TopicItem("sub topic 2-1"));
            topicItem2.Child.Add(new TopicItem("sub topic 2-2"));
            topicItem2.Child.Add(new TopicItem("sub topic 2-3"));

            Root[0].Child.Add(topicItem1);
            Root[0].Child.Add(topicItem2);
        }
    }
}