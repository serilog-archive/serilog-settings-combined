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
        public static LoggerConfiguration Combined(this LoggerSettingsConfiguration lsc, Func<ICombinedSettingsBuilder, ICombinedSettingsBuilder> combine)
        {
            if (combine == null) throw new ArgumentNullException(nameof(combine));

            var builder = (CombinedSettingsBuilder)combine(new CombinedSettingsBuilder());
            var combinedSettings = builder.BuildSettings();
            return lsc.KeyValuePairs(combinedSettings);
        }
    }
}
