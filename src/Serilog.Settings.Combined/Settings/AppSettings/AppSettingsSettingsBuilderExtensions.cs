#if NET45
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
        public static ICombinedSettingsBuilder AddAppSettings(this ICombinedSettingsBuilder builder, string settingPrefix = null, string filePath = null)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            var appSettings = new AppSettingsSettings(settingPrefix, filePath);
            builder.AddKeyValuePairs(appSettings.GetKeyValuePairs());
            return builder;
        }
    }
}
#endif
