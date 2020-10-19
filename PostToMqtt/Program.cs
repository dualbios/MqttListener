using System;
using System.Text;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client.Options;
using MQTTnet.Extensions.ManagedClient;

namespace PostToMqtt
{
    internal class Program
    {
        public static async Task Run(string topic, string message)
        {
            IManagedMqttClient _mqttClient = new MqttFactory().CreateManagedMqttClient();

            var options = new ManagedMqttClientOptionsBuilder()
                .WithClientOptions(new MqttClientOptionsBuilder()
                    .WithClientId("_appConfigurationOptions.Value.ClientId")
                    .WithTcpServer("192.168.0.111", 1883)
                    .WithCredentials("mqtt", "mqtt")
                    .Build())
                .WithAutoReconnectDelay(TimeSpan.MaxValue)
                .Build();

            await _mqttClient.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic("#").Build());
            await _mqttClient.StartAsync(options);
            await _mqttClient.PublishAsync(new MqttApplicationMessage() { Topic = topic, Payload = Encoding.UTF8.GetBytes(message) });
        }

        private static void Main(string[] args)
        {
            Run("test", "value-1").GetAwaiter().GetResult();
        }
    }
}