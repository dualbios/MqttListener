using System;
using MqttListener.ViewModels;

namespace MqttListener.Core
{
    public class TopicMessageHistoryItem : BaseViewModel
    {
        public TopicMessageHistoryItem(DateTime dateTime, string message)
        {
            DateTime = dateTime;
            Message = message;
        }

        public DateTime DateTime { get; }
        public string Message { get; }
    }
}