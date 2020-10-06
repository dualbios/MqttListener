using System;
using System.Windows;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MqttListener.Configuration;
using MqttListener.ViewModels;

namespace MqttListener
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private const string connectionsJsonFileName = "connections.json";
        public IConfiguration Configuration { get; private set; }

        public IServiceProvider Provider { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            IServiceCollection services = new ServiceCollection();
            ConfigurationBuilder configurationBuilder = new ConfigurationBuilder();

            configurationBuilder.AddJsonFile(connectionsJsonFileName, true, reloadOnChange: true);
            Configuration = configurationBuilder.Build();

            services.Configure<AppConfiguration>(Configuration);
            services.ConfigureWritable<ConnectionsList>(Configuration.GetSection("ConnectionsList"), connectionsJsonFileName);

            Provider = services.BuildServiceProvider();

            var context = new MainWindowViewModel();
            var view = new MainWindow() { DataContext = context };
            view.Show();
        }
    }
}