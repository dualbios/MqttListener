using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using MqttListener.ViewModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MqttListener.Core
{
    public class TopicItem : BaseViewModel
    {
        private readonly ObservableCollection<TopicItem> _child = new ObservableCollection<TopicItem>();
        private readonly ObservableCollection<TopicMessageHistoryItem> _messageHistory = new ObservableCollection<TopicMessageHistoryItem>();
        private string _message;
        private string _name;

        public TopicItem(string name, TopicItem parent)
        {
            Name = name;
            Parent = parent;
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

                    OnPropertyChanged(nameof(MessageCount));
                    OnPropertyChanged(nameof(OldMessage));
                    OnPropertyChanged(nameof(MessageIndented));
                    OnPropertyChanged(nameof(Child));
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

        public TopicItem Parent { get; }

        public int TopicCount
        {
            get => _child.Count(x => string.IsNullOrEmpty(x.Message));
        }

        public string OldMessage
        {
            get
            {
                if (_messageHistory.Count < 2)
                    return string.Empty;
                int index = _messageHistory.Count - 2;
                return IndentMessage(_messageHistory[index].Message);
            }
        }

        public string MessageIndented => IndentMessage(Message);

        private string IndentMessage(string message)
        {
            string result = message;
            if (!string.IsNullOrEmpty(result))
            {
                try
                {
                    JObject o = (JObject)JToken.ReadFrom(new JsonTextReader(new StringReader(result)));
                    result = o.ToString(Formatting.Indented);
                }
                catch  { }
            }

            return result;
        }

        public IEnumerable<string> GetFullName()
        {
            return InternalGetFullName().Reverse().ToList();
        }

        private IEnumerable<string> InternalGetFullName()
        {
            TopicItem ti = this;
            while (ti != null)
            {
                yield return ti.Name;
                ti = ti.Parent;
            }
        }
    }
}