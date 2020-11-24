using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;
using MqttListener.Core;
using MqttListener.Interfaces;

namespace MqttListener.ViewModels
{
    public class HistoryViewModel : BaseViewModel, IDialog
    {
        private readonly IServiceProvider _serviceProvider;

        private RelayCommand _closeCommand;
        private IDialogHost _dialogHost;
        private TopicMessageHistoryItem _selectedMessageHistoryItem;
        private Listener _listener;
        private TopicItem _selectedTopicItem;

        public HistoryViewModel(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _listener = _serviceProvider.GetService<Listener>();
            SelectedTopicItem = _listener?.Root[0];
        }

        public object Root => _listener.Root;

        public ICommand CloseCommand => _closeCommand ??= new RelayCommand(o => Close());
        public IEnumerable<TopicMessageHistoryItem> MessageHistory => _selectedTopicItem?.MessageHistory;

        public bool IsHistoryEmpty => _selectedTopicItem?.MessageHistory == null ? false : !_selectedTopicItem.MessageHistory.Any();

        public string NewMessage { get; private set; }

        public DateTime? NewTextDate { get; private set; }

        public string OldMessage { get; private set; }

        public DateTime? OldTextDate { get; private set; }

        public string TopicItemName => _selectedTopicItem == null ? null : String.Join("/", _selectedTopicItem.GetFullName());

        public TopicItem SelectedTopicItem
        {
            get => _selectedTopicItem;
            set
            {
                if (SetProperty(ref _selectedTopicItem, value))
                {
                    OnPropertyChanged(null);
                }
            }
        }

        public TopicMessageHistoryItem SelectedMessageHistoryItem
        {
            get => _selectedMessageHistoryItem;
            set
            {
                if (SetProperty(ref _selectedMessageHistoryItem, value))
                {
                    if (_selectedTopicItem?.MessageHistory != null)
                    {
                        var collection = _selectedTopicItem.MessageHistory as IList<TopicMessageHistoryItem>;
                        if (collection != null)
                        {
                            int index = collection.IndexOf(_selectedMessageHistoryItem);
                            if (index == -1)
                            {
                                NewTextDate = null;
                                NewMessage = null;
                                OldMessage = null;
                                OldTextDate = null;
                            }
                            else
                            {
                                NewTextDate = _selectedMessageHistoryItem.DateTime;
                                NewMessage = _selectedMessageHistoryItem.Message;

                                TopicMessageHistoryItem oldItem = null;
                                if (index + 1 < collection.Count)
                                {
                                    oldItem = collection.ElementAt(index + 1);
                                }

                                OldTextDate = oldItem?.DateTime;
                                OldMessage = oldItem?.Message;
                            }

                            OnPropertyChanged(nameof(NewMessage));
                            OnPropertyChanged(nameof(NewTextDate));
                            OnPropertyChanged(nameof(OldMessage));
                            OnPropertyChanged(nameof(OldTextDate));
                        }
                    }
                }
            }
        }

        public void OnOpen(IDialogHost dialogHost)
        {
            _dialogHost = dialogHost;
        }

        private void Close()
        {
            _dialogHost?.CloseDialog(true);
        }
    }
}