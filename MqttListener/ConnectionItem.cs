using MqttListener.ViewModels;

namespace MqttListener
{
    public class ConnectionItem : BaseViewModel
    {
        private string _connectionName;
        private string _host;
        private string _password;
        private string _port;
        private string _protocol;
        private string _qos;
        private string _topic;
        private string _username;

        public ConnectionItem()
        {
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

        public string Topic
        {
            get => _topic;
            set => SetProperty(ref _topic, value);
        }

        public string Username
        {
            get => _username;
            set => SetProperty(ref _username, value);
        }
    }
}