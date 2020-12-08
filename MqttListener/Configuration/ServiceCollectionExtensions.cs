using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace MqttListener.Configuration
{
    public static class ServiceCollectionExtensions
    {
        public static void ConfigureWritable<T>(
            this IServiceCollection services,
            IConfigurationSection section,
            string file = "appsettings.json") where T : class, new()
        {
            services.Configure<T>(section);
            services.AddSingleton<IWritableOptions<T>>(provider =>
            {
                var configuration = (IConfigurationRoot)provider.GetService<IConfiguration>();
                var options = provider.GetService<IOptionsMonitor<T>>();
                return new WritableOptions<T>(options, configuration, section.Key, file);
            });
        }
        public static void ConfigureWritable<T>(
            this IServiceCollection services,
            IConfigurationSection section,
            Action<T> initializeAction,
            string file = "appsettings.json") where T : class, new()
        {
            services.Configure<T>(section);
            services.AddSingleton<IWritableOptions<T>>(provider =>
            {
                var configuration = (IConfigurationRoot)provider.GetService<IConfiguration>();
                var options = provider.GetService<IOptionsMonitor<T>>();
                initializeAction?.Invoke(options.CurrentValue);
                return new WritableOptions<T>(options, configuration, section.Key, file);
            });
        }
    }
}