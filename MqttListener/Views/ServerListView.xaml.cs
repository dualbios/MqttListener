using System.Windows;
using System.Windows.Controls;

namespace MqttListener.Views
{
    /// <summary>
    /// Interaction logic for ServerListView.xaml
    /// </summary>
    public partial class ServerListView : UserControl
    {
        public ServerListView()
        {
            InitializeComponent();
        }

        private void PasswordBox_OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            PasswordBox passwordBox = sender as PasswordBox;

            if (this.DataContext != null && passwordBox?.DataContext != null)
            {
                ((dynamic)passwordBox.DataContext).Password = passwordBox.Password;
            }
        }
    }
}