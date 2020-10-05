using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MqttListener.Views
{
    /// <summary>
    /// Interaction logic for ConnectionItemView.xaml
    /// </summary>
    public partial class ConnectionItemView : UserControl
    {
        public ConnectionItemView()
        {
            InitializeComponent();

            DataContextChanged += ConnectionItemView_DataContextChanged;
            PasswordBox.PasswordChanged += PasswordBox_PasswordChanged;
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            var connectionItem = DataContext as ConnectionItem;
            connectionItem.Password = PasswordBox.Password;
        }

        private void ConnectionItemView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
        }
    }
}
