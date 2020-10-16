using System.Collections.Generic;
using System.Linq;
using MqttListener.ViewModels;

namespace MqttListener.Configuration
{
    public class ConnectionItem : BaseViewModel
    {
        private string _connectionName;
        private string _host;
        private string _password;
        private string _port;
        private string _protocol;
        private string _qos;
        private List<string> _topic;
        private string _username;

        public ConnectionItem()
        {
            SupportedProtocols = new[] { "mqtt://" };
            SupportedQos = new[] { "0", "1", "2" };
        }

        public string ConnectionName
        {
            get => _connectionName;
            set => SetProperty(ref _connectionName, value);
        }

        public string Host
        {
            get => _host;
            set => SetProperty(ref _host, value);
        }

        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        public string Port
        {
            get => _port;
            set => SetProperty(ref _port, value);
        }

        public string Protocol
        {
            get => _protocol;
            set => SetProperty(ref _protocol, value);
        }

        public string Qos
        {
            get => _qos;
            set => SetProperty(ref _qos, value);
        }

        public IEnumerable<string> SupportedProtocols { get; private set; }
        public IEnumerable<string> SupportedQos { get; private set; }

        public List<string> Topics
        {
            get => _topic;
            set
            {
                SetProperty(ref _topic, value);
                OnPropertyChanged(nameof(TopicsList));
            }
        }

        public string TopicsList
        {
            get { return string.Join(";", Topics ?? Enumerable.Empty<string>()); }
        }

        public string Username
        {
            get => _username;
            set => SetProperty(ref _username, value);
        }
    }
}