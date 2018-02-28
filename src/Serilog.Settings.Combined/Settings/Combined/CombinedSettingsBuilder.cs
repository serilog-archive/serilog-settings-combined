// Copyright 2013-2017 Serilog Contributors
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

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
