using MqttListener.ViewModels;

namespace MqttListener.Configuration
{
    public class SettingTopicItem : BaseViewModel
    {
        private string _name;

        public SettingTopicItem(string name)
        {
            Name = name;
        }

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }
    }

    public class NewSettingTopicItem : SettingTopicItem
    {
        public NewSettingTopicItem() : base(null)
        {
            
        }
    }
}