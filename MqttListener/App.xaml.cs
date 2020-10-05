using System.Windows;
using MqttListener.ViewModels;

namespace MqttListener
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            var context = new MainWindowViewModel();
            var view = new MainWindow() { DataContext = context };
            view.Show();
        }
    }
}