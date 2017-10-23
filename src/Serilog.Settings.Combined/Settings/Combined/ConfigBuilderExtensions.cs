using System;
using System.Collections.Generic;

namespace Serilog.Settings.Combined
{
    public static class ConfigBuilderExtensions
    {
        public static IConfigBuilder AddKeyValuePairs(this IConfigBuilder builder, IReadOnlyDictionary<string, string> keyValuePairs)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            if (keyValuePairs == null) throw new ArgumentNullException(nameof(keyValuePairs));

            return builder.AddSource(keyValuePairs);
        }

        public static IConfigBuilder AddKeyValuePair(this IConfigBuilder builder, KeyValuePair<string, string> keyValuePair)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));

            return builder.AddSource(new[] { keyValuePair });
        }

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
