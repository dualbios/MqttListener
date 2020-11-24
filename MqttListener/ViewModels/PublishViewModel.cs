using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;
using MqttListener.Core;

namespace MqttListener.ViewModels
{
    public class PublishViewModel : BaseViewModel
    {
        private readonly ObservableCollection<PostMessageItem> _postMessageItems;
        private readonly IServiceProvider _serviceProvider;
        private Listener _listener;
        private string _paylolad;
        private RelayCommand _postCommand;
        private RelayCommand _selectedItemDoubleClickCommand;
        private PostMessageItem _selectedPostItem;
        private TopicItem _selectedTopicItem;
        private string _state;
        private string _topic;

        public PublishViewModel(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _listener = _serviceProvider.GetService<Listener>();
            _postMessageItems = new ObservableCollection<PostMessageItem>();
        }

        public object Root => _listener.Root;

        public string Payload
        {
            get => _paylolad;
            set => SetProperty(ref _paylolad, value);
        }

        public ICommand PostCommand => _postCommand ??= new RelayCommand(async x => await Post());
        public IEnumerable<PostMessageItem> PostMessageItems => _postMessageItems;
        public ICommand SelectedItemDoubleClickCommand => _selectedItemDoubleClickCommand ??= new RelayCommand(x => SelectedItemDoubleClick(x));

        public PostMessageItem SelectedPostItem
        {
            get => _selectedPostItem;
            set => SetProperty(ref _selectedPostItem, value);
        }

        public TopicItem SelectedTopicItem
        {
            get => _selectedTopicItem;
            set
            {
                SetProperty(ref _selectedTopicItem, value);
                Topic = value == null ? null : String.Join("/", _selectedTopicItem.GetFullName());
            }
        }

        public string State
        {
            get => _state;
            private set => SetProperty(ref _state, value);
        }

        public string Topic
        {
            get => _topic;
            set => SetProperty(ref _topic, value);
        }

        private async Task Post()
        {
            try
            {
                State = "Publishing ...";
                await _listener.Post(Topic, Payload);
                await Task.Delay(250);
                State = "Sent.";

                _postMessageItems.Add(new PostMessageItem(DateTime.Now, Payload, Topic));
            }
            catch (Exception e)
            {
                State = e.Message;
            }
        }

        private void SelectedItemDoubleClick(object x)
        {
            PostMessageItem item = x as PostMessageItem;
            if (item == null)
                return;

            Payload = item.Payload;
            Topic = item.Topic;
            State = "Restored from history.";
        }
    }
}