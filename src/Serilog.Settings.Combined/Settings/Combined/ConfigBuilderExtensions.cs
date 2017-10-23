using System;
using System.Collections.Generic;

namespace Serilog.Settings.Combined
{
    /// <summary>
    /// Contains extensions in order to build a configuration from multiple sources
    /// </summary>
    public static class ConfigBuilderExtensions
    {
        /// <summary> 
        /// Adds a series of key-value settings to the combined configuration 
        /// </summary> 
        /// <param name="builder">the builder</param> 
        /// <param name="keyValuePairs">the key-value pairs to add</param> 
        /// <returns>the builder object to allow chaining</returns> 
        public static IConfigBuilder AddKeyValuePairs(this IConfigBuilder builder, IEnumerable<KeyValuePair<string, string>> keyValuePairs)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            if (keyValuePairs == null) throw new ArgumentNullException(nameof(keyValuePairs));

            return builder.AddSource(keyValuePairs);
        }

        /// <summary> 
        /// Adds a single key-value setting to the combined configuration 
        /// </summary> 
        /// <param name="builder">the builder</param> 
        /// <param name="keyValuePair">a key-value setting</param> 
        /// <returns>the builder object to allow chaining</returns> 
        public static IConfigBuilder AddKeyValuePair(this IConfigBuilder builder, KeyValuePair<string, string> keyValuePair)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));

            return builder.AddSource(new[] { keyValuePair });
        }


        /// <summary> 
        /// Adds a single key-value setting to the combined configuration 
        /// </summary> 
        /// <param name="builder">the builder</param> 
        /// <param name="key">the key of the setting</param> 
        /// <param name="value">the value of the setting</param> 
        /// <returns>the builder object to allow chaining</returns> 
        public static IConfigBuilder AddKeyValuePair(this IConfigBuilder builder, string key, string value)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));

            return builder.AddSource(new Dictionary<string, string>()
            {
                {key, value}
            });
        }
    }
}
