using System;
using System.Collections.Generic;
using System.Reflection;

namespace Hansen.Kafka.Utility.Configuration
{
    public class ConnectionConfiguration
    {

        [ConfigurationPropertyName("bootstrap.servers")]
        public string BootstrapServers { get; set; }

        public IEnumerable<KeyValuePair<string, string>> ToPairs()
        {
            var props = GetType().GetProperties();
            List<KeyValuePair<string, string>> res = new List<KeyValuePair<string, string>>();
            foreach (PropertyInfo propertyInfo in props)
            {
                var nameAtt = propertyInfo.GetCustomAttribute<ConfigurationPropertyNameAttribute>();
                if (nameAtt == null) { throw new Exception("Missing attribute name"); }

                object value = propertyInfo.GetValue(this);

                if (value != null)
                {
                    res.Add(new KeyValuePair<string, string>(nameAtt.Name, value.ToString()));
                }
            }

            return res;
        }

        internal class ConfigurationPropertyNameAttribute : Attribute
        {
            public string Name { get; set; }

            public ConfigurationPropertyNameAttribute(string name)
            {
                Name = name;
            }
        }
    }
}