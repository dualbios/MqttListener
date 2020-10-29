using System;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MqttListener.Configuration
{
    public class WritableOptions<T> : IWritableOptions<T> where T : class, new()
    {
        private readonly IConfigurationRoot _configuration;
        private readonly string _file;
        private readonly IOptionsMonitor<T> _options;
        private readonly string _section;

        public WritableOptions(
            IOptionsMonitor<T> options,
            IConfigurationRoot configuration,
            string section,
            string file)
        {
            _options = options;
            _configuration = configuration;
            _section = section;
            _file = file;
        }

        public T Value => _options.CurrentValue;

        public T Get(string name) => _options.Get(name);

        public void Save()
        {
            string physicalPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), _file);

            var jObject = JsonConvert.DeserializeObject<JObject>(File.ReadAllText(physicalPath));
            T sectionObject = Value;

            jObject[_section] = JObject.Parse(JsonConvert.SerializeObject(sectionObject));
            File.WriteAllText(physicalPath, JsonConvert.SerializeObject(jObject, Formatting.Indented));
            _configuration?.Reload();
        }
    }
}