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

using System.Collections.Generic;

namespace Serilog.Settings.Combined
{
    /// <summary>
    /// A builder that allows to combine sources of key-value settings in a fluent way.
    /// </summary>
    public interface ICombinedSettingsBuilder
    {
        /// <summary>
        /// Adds a series of key-value settings to the combined configuration
        /// </summary>
        /// <param name="keyValuePairs">the key-value pairs to add</param>
        /// <returns>a <see cref="ICombinedSettingsBuilder"/> with the added source to allow chaining</returns>
        ICombinedSettingsBuilder AddKeyValuePairs(IEnumerable<KeyValuePair<string, string>> keyValuePairs);

        /// <summary>
        /// Adds a single key-value setting to the combined configuration
        /// </summary>
        /// <param name="kvp">a key-value setting</param>
        /// <returns>the builder object to allow chaining</returns>
        ICombinedSettingsBuilder AddKeyValuePair(KeyValuePair<string, string> kvp);

        /// <summary>
        /// Adds a single key-value setting to the combined configuration
        /// </summary>
        /// <param name="key">the key of the setting</param>
        /// <param name="value">the value of the setting</param>
        /// <returns>the builder object to allow chaining</returns>
        ICombinedSettingsBuilder AddKeyValuePair(string key, string value);
    }
}
