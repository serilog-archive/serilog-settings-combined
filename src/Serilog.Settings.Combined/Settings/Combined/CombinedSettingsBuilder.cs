using System;
using System.Collections.Generic;

namespace Serilog.Settings.Combined
{
    /// <summary>
    /// Builder to combine multiple collections of settings into one
    /// </summary>
    public sealed class CombinedSettingsBuilder : ICombinedSettingsBuilder
    {
        List<IEnumerable<KeyValuePair<string, string>>> _settings = new List<IEnumerable<KeyValuePair<string, string>>>();

        /// <inheritdoc />
        public ICombinedSettingsBuilder AddKeyValuePairs(IEnumerable<KeyValuePair<string, string>> keyValuePairs)
        {
            if (keyValuePairs == null) throw new ArgumentNullException(nameof(keyValuePairs));

            _settings.Add(keyValuePairs);
            return this;
        }

        /// <inheritdoc />
        public ICombinedSettingsBuilder AddKeyValuePair(KeyValuePair<string, string> kvp)
        {
            return AddKeyValuePairs(new List<KeyValuePair<string, string>> { kvp });
        }

        /// <inheritdoc />
        public ICombinedSettingsBuilder AddKeyValuePair(string key, string value)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));

            return AddKeyValuePair(new KeyValuePair<string, string>(key, value));
        }

        /// <summary>
        /// Creates a collection of settings by concatenating all the key-value settings passed in previously
        /// </summary>
        /// <returns>The result of combining all the previous settings</returns>
        public IEnumerable<KeyValuePair<string, string>> BuildSettings()
        {
            foreach (var source in _settings)
            {
                foreach (var setting in source)
                {
                    yield return setting;
                }
            }
        }
    }
}
