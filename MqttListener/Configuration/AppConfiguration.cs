using System.Collections.Generic;

namespace MqttListener.Configuration
{
    public class AppConfiguration
    {
        public string ClientId { get; set; }
        public IList<ConnectionItem> Connections { get; set; }
    }
}