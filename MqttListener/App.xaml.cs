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
        private const string connectionsJson = "connections.json";
        public IConfiguration Configuration { get; private set; }

        public IServiceProvider Provider { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            ConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.Add<WritableJsonConfigurationSource>(
                s =>
                {
                    s.FileProvider = null;
                    s.Path = connectionsJson;
                    s.Optional = false;
                    s.ReloadOnChange = true;
                    s.ResolveFileProvider();
                });
            configurationBuilder.AddJsonFile(connectionsJson, true, reloadOnChange: true);

            Configuration = configurationBuilder.Build();

            IServiceCollection services = new ServiceCollection();
            services.Configure<AppConfiguration>(Configuration);

            Provider = services.BuildServiceProvider();

            AppConfiguration settings = Provider.GetRequiredService<IOptions<AppConfiguration>>().Value;

            //string s = Configuration["Connections"];

            var context = new MainWindowViewModel(Provider);
            var view = new MainWindow() { DataContext = context };
            view.Show();
        }
    }
}