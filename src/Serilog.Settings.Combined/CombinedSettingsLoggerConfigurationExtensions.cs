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
        /// Configure the logger from multiple sources of settings specified in the Serilog key-value setting format.
        /// </summary>
        /// <param name="lsc">The settings configuration object.</param>
        /// <param name="build">the operations to execute on the builder to configure other sources of settings.</param>
        /// <returns>Configuration object allowing method chaining.</returns>
        public static LoggerConfiguration Combined(this LoggerSettingsConfiguration lsc, Func<IConfigBuilder, IConfigBuilder> build)
        {
            var configBuilder = new ConfigBuilder();
            configBuilder = (ConfigBuilder)build(configBuilder);
            var enumerable = configBuilder.BuildCombinedEnumerable();

            return lsc.KeyValuePairs(enumerable);
        }
    }
}
