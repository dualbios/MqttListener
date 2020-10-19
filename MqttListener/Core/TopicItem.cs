using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using MqttListener.ViewModels;

namespace MqttListener.Core
{
    public class TopicItem : BaseViewModel
    {
        private readonly ObservableCollection<TopicItem> _child = new ObservableCollection<TopicItem>();
        private readonly ObservableCollection<TopicMessageHistoryItem> _messageHistory = new ObservableCollection<TopicMessageHistoryItem>();
        private string _message;
        private string _name;

        public TopicItem(string name)
        {
            Name = name;
        }

        public IList<TopicItem> Child
        {
            get => _child;
        }

        public int ChildCount
        {
            get => _child.Count;
        }

        public string Message
        {
            get => _message;
            set
            {
                if (SetProperty(ref _message, value))
                {
                    var historyItem = new TopicMessageHistoryItem(DateTime.Now, _message);

                    if (Application.Current.Dispatcher.CheckAccess())
                    {
                        _messageHistory.Add(historyItem);
                    }
                    else
                    {
                        Application.Current.Dispatcher.Invoke(() => _messageHistory.Add(historyItem));
                    }
                }
            }
        }

        public int MessageCount
        {
            get => _child.Count(x => !string.IsNullOrEmpty(x.Message));
        }

        public IEnumerable<TopicMessageHistoryItem> MessageHistory
        {
            get => _messageHistory;
        }

        public string Name
        {
            get => _name;
            private set => SetProperty(ref _name, value);
        }

        public int TopicCount
        {
            get => _child.Count(x => string.IsNullOrEmpty(x.Message));
        }
    }
}