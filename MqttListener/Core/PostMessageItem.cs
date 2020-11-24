using System;
using MqttListener.ViewModels;

namespace MqttListener.Core
{
    public class PostMessageItem : BaseViewModel
    {
        public PostMessageItem(DateTime dateTime, string payload, string topic)
        {
            DateTime = dateTime;
            Payload = payload;
            Topic = topic;
        }

        public DateTime DateTime { get; }
        public string Payload { get; }
        public string Topic { get; }
    }
}