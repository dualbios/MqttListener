using System;
using Microsoft.Extensions.Options;

namespace MqttListener.Configuration
{
    public interface IWritableOptions<out T> : IOptions<T> where T : class, new()
    {
        void Save();
    }
}