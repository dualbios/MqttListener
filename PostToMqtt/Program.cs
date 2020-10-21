using System;
using System.Text;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client.Options;
using MQTTnet.Client.Publishing;
using MQTTnet.Diagnostics;
using MQTTnet.Extensions.ManagedClient;

namespace PostToMqtt
{
    internal class Program
    {
        private static void Logger_LogMessagePublished(object sender, MqttNetLogMessagePublishedEventArgs e)
        {
        }

        private static async Task<IManagedMqttClient> ConnectClientAsync()
        {
            IMqttNetLogger logger = new MqttNetLogger("sss");
            logger.LogMessagePublished += Logger_LogMessagePublished;
            IManagedMqttClient _mqttClient = new MqttFactory().CreateManagedMqttClient(logger);

            var options = new ManagedMqttClientOptionsBuilder()
                .WithClientOptions(new MqttClientOptionsBuilder()
                    .WithClientId("_appConfigurationOptions.Value.ClientId")
                    .WithTcpServer("192.168.0.111", 1883)
                    .WithCredentials("mqtt", "mqtt")
                    .Build())
                .WithAutoReconnectDelay(TimeSpan.MaxValue)
                .Build();

            await _mqttClient.StartAsync(options);

            await Task.Delay(3000);

            return _mqttClient;
        }

        private static void Main(string[] args)
        {
            IManagedMqttClient client = ConnectClientAsync().GetAwaiter().GetResult();
            string mess = "String.Empty";

            while (!string.IsNullOrEmpty(mess = Console.ReadLine()))
            {
                int index = mess.IndexOf(' ');
                string[] strings = new string[2]{ mess.Substring(0, index), mess.Substring(index) } ;
                MqttClientPublishResult result = client.PublishAsync(
                    new MqttApplicationMessageBuilder()
                        .WithTopic(strings[0])
                        .WithPayload(Encoding.UTF8.GetBytes(strings[1]))
                        .WithAtMostOnceQoS()
                        .Build())
                    .GetAwaiter()
                    .GetResult();
            }
        }
    }
}