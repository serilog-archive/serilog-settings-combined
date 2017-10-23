using System.Collections.Generic;

namespace Serilog.Settings.Combined
{
    class ConfigBuilder : IConfigBuilder
    {
        List<IEnumerable<KeyValuePair<string, string>>> _sources;

        public ConfigBuilder()
        {
            _sources = new List<IEnumerable<KeyValuePair<string, string>>>();
        }

        public IEnumerable<KeyValuePair<string, string>> BuildCombinedEnumerable()
        {
            IEnumerable<KeyValuePair<string, string>> Combined()
            {
                var result = new Dictionary<string, string>();
                foreach (var source in _sources)
                {
                    foreach (var kvp in source)
                    {
                        result[kvp.Key] = kvp.Value;
                    }
                }
                return result;
            }

            foreach (var kvp in Combined())
            {
                yield return kvp;
            }
        }

        public IConfigBuilder AddSource(IEnumerable<KeyValuePair<string, string>> source)
        {
            _sources.Add(source);
            return this;
        }
    }
}
