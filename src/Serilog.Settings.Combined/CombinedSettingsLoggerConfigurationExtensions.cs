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
using Serilog.Configuration;
using Serilog.Settings.Combined;

namespace Serilog
{
    /// <summary>
    /// Extensions for combined sources of key-value settings
    /// </summary>
    public static class CombinedSettingsLoggerConfigurationExtensions
    {
        /// <summary> 
        /// Apply settings specified from multiple sources and combine them keeping the last defined value for each key. 
        /// </summary> 
        /// <param name="lsc">The settings configuration object.</param>
        /// <param name="combine">a callback that allows to add Sources of settings to the configuration</param> 
        /// <returns>Configuration object allowing method chaining.</returns> 
        public static LoggerConfiguration Combined(this LoggerSettingsConfiguration lsc, Func<CombinedSettingsBuilder, CombinedSettingsBuilder> combine)
        {
            if (combine == null) throw new ArgumentNullException(nameof(combine));

            var builder = combine(new CombinedSettingsBuilder());
            var combinedSettings = builder.BuildSettings();
            return lsc.KeyValuePairs(combinedSettings);
        }
    }
}
