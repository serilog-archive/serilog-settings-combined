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

#if APPSETTINGS
using System;
using Serilog.Settings.AppSettings;
using Serilog.Settings.Combined;

namespace Serilog
{
    /// <summary>
    /// Extensions to allow combination of settings originating from config file appSettings
    /// </summary>
    public static class AppSettingsSettingsBuilderExtensions
    {
        /// <summary>
        /// Reads the &lt;appSettings&gt; element of App.config or Web.config, searching for keys
        /// that look like: <code>serilog:*</code>, which are used to configure
        /// the logger.
        /// </summary>
        /// <param name="builder">The combined settings builder</param>
        /// <param name="settingPrefix">Prefix to use when reading keys in appSettings. If specified the value
        /// will be prepended to the setting keys and followed by :, for example "myapp" will use "myapp:serilog:minumum-level. If null
        /// no prefix is applied.</param>
        /// <param name="filePath">Specify the path to an alternative .config file location. If the file does not exist it will be ignored.
        /// By default, the current application's configuration file will be used.</param>
        /// <returns>An object allowing configuration to continue.</returns>
        public static CombinedSettingsBuilder AddAppSettings(this CombinedSettingsBuilder builder, string settingPrefix = null, string filePath = null)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            var appSettings = new AppSettingsSettings(settingPrefix, filePath);
            builder.AddKeyValuePairs(appSettings.GetKeyValuePairs());
            return builder;
        }
    }
}
#endif
