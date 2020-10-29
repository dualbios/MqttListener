using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using MqttListener.Core;

namespace MqttListener.ViewModels
{
    public class TreeViewModel : BaseViewModel
    {
        private readonly IServiceProvider _serviceProvider;
        private HistoryViewModel _historyLog;
        private bool _isPrettyView;
        private Listener _listener;
        private TopicItem _selectedTopicItem;

        public TreeViewModel(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _listener = serviceProvider.GetService<Listener>();

            SelectedTopicItem = _listener.Root[0];
        }

        public string ConnectionName => _listener.ConnectionName;

        public HistoryViewModel HistoryLog
        {
            get => _historyLog;
            private set => SetProperty(ref _historyLog, value);
        }

        public bool IsPrettyView
        {
            get => _isPrettyView;
            set => SetProperty(ref _isPrettyView, value);
        }

        public string LastMessage => _listener.LastMessage;

        public string LastTopic => _listener.LastTopic;

        public IList<TopicItem> Root => _listener.Root;

        public TopicItem SelectedTopicItem
        {
            get => _selectedTopicItem;
            set => SetProperty(ref _selectedTopicItem, value);
        }
    }
}