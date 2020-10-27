using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using MqttListener.Core;
using MqttListener.Interfaces;

namespace MqttListener.ViewModels
{
    public class HistoryViewModel : BaseViewModel, IDialog
    {
        private readonly TopicItem _topicItem;
        private RelayCommand _closeCommand;
        private IDialogHost _dialogHost;
        private TopicMessageHistoryItem _selectedMessageHistoryItem;

        public HistoryViewModel(TopicItem topicItem)
        {
            _topicItem = topicItem;
            SelectedMessageHistoryItem = _topicItem.MessageHistory?.OrderByDescending(x => x.DateTime).FirstOrDefault();
            TopicItemName = string.Join("\\", topicItem.GetFullName().Reverse());
        }

        public ICommand CloseCommand => _closeCommand ??= new RelayCommand(o => Close());
        public IEnumerable<TopicMessageHistoryItem> MessageHistory => _topicItem.MessageHistory;

        public bool IsHistoryEmpty => !_topicItem.MessageHistory.Any();

        public string NewMessage { get; private set; }

        public DateTime? NewTextDate { get; private set; }

        public string OldMessage { get; private set; }

        public DateTime? OldTextDate { get; private set; }

        public string TopicItemName { get; private set; }

        public TopicMessageHistoryItem SelectedMessageHistoryItem
        {
            get => _selectedMessageHistoryItem;
            set
            {
                if (SetProperty(ref _selectedMessageHistoryItem, value))
                {
                    var collection = _topicItem.MessageHistory as IList<TopicMessageHistoryItem>;
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