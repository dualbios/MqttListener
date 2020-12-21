using System;
using System.Collections.Generic;
using System.Windows;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MqttListener.Configuration;
using MqttListener.Core;
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

            services.ConfigureWritable<ConnectionsList>(
                Configuration.GetSection("ConnectionsList"),
                c =>
                {
                    if (c.Connections == null)
                        c.Connections = new List<ConnectionItem>() { new ConnectionItem() { ConnectionName = "_New_connection_" } };
                },
                connectionsJsonFileName);
            services.ConfigureWritable<AppConfiguration>(Configuration.GetSection("AppConfiguration"), connectionsJsonFileName);
            services.AddSingleton(x => new Listener(x));
            services.AddSingleton(x => new ServerListViewModel(x));
            services.AddSingleton(x => new TreeViewModel(x));
            services.AddSingleton(x => new HistoryViewModel(x));
            services.AddSingleton(x => new PublishViewModel(x));
            services.AddSingleton(x => new AboutViewModel());

            Provider = services.BuildServiceProvider();

            var context = new MainWindowViewModel(Provider);
            var view = new MainWindow() { DataContext = context };
            view.Closed += View_Closed;
            view.Show();
        }

        private void View_Closed(object sender, EventArgs e)
        {
            Provider.GetService<Listener>().Disconnect();
        }
    }
}