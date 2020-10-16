using System.Windows;
using System.Windows.Controls;
using MqttListener.Configuration;

namespace MqttListener.Converters
{
    public class TopicsDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate NewRowDataTemplate { get; set; }
        public DataTemplate TopicDataTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item == null)
                return NewRowDataTemplate;

            if (item.GetType() == typeof(SettingTopicItem))
                return TopicDataTemplate;

            return NewRowDataTemplate;
        }
    }
}