using System.Windows;
using System.Windows.Controls;

namespace MqttListener.Views
{
    /// <summary>
    /// Interaction logic for PostView.xaml
    /// </summary>
    public partial class PublishView : UserControl
    {
        public PublishView()
        {
            InitializeComponent();
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            SelectedItemHelper.Content = e.NewValue;
        }
    }
}