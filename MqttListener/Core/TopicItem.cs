using System.Collections.Generic;
using System.Collections.ObjectModel;
using MqttListener.ViewModels;

namespace MqttListener.Core
{
    public class TopicItem : BaseViewModel
    {
        private ObservableCollection<TopicItem> _child = new ObservableCollection<TopicItem>();
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

        public string Message
        {
            get => _message;
            set => SetProperty(ref _message, value);
        }

        public string Name
        {
            get => _name;
            private set => SetProperty(ref _name, value);
        }
    }
}