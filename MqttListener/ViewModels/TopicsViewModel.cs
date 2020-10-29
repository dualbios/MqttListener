using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using MqttListener.Configuration;
using MqttListener.Interfaces;

namespace MqttListener.ViewModels
{
    public class TopicsViewModel : BaseViewModel, IDialog
    {
        public ConnectionItem ConnectionItem { get; }
        private RelayCommand _addCommand;
        private RelayCommand _closeCommand;
        private IDialogHost _dialogHost;
        private RelayCommand _removeCommand;

        public TopicsViewModel(ConnectionItem connectionItem)
        {
            ConnectionItem = connectionItem;
            IEnumerable<SettingTopicItem> items = (connectionItem.Topics ?? Enumerable.Empty<string>())
                .Select(x => new SettingTopicItem(x)).Concat(new SettingTopicItem[] { new NewSettingTopicItem(), });

            Topics = new ObservableCollection<SettingTopicItem>(items);
        }

        public ICommand AddCommand => _addCommand ??= new RelayCommand(x => AddItem());
        public ICommand CloseCommand => _closeCommand ??= new RelayCommand(x => Close());
        public ICommand RemoveCommand => _removeCommand ??= new RelayCommand(x => RemoveItem(x as SettingTopicItem));

        public ObservableCollection<SettingTopicItem> Topics { get; private set; }

        public void OnOpen(IDialogHost dialogHost)
        {
            _dialogHost = dialogHost;
        }

        private void AddItem()
        {
            SettingTopicItem newTopic = Topics.FirstOrDefault(x => x is NewSettingTopicItem);
            int indexOf = Topics.IndexOf(newTopic);
            Topics.Insert(indexOf, new SettingTopicItem("new_topic"));
        }

        private void Close()
        {
            ConnectionItem.Topics = Topics.Where(x => !(x is NewSettingTopicItem)).Select(x => x.Name).ToList();
            _dialogHost.CloseDialog(true);
        }

        private void RemoveItem(SettingTopicItem settingTopicItem)
        {
            Topics.Remove(settingTopicItem);
        }
    }
}