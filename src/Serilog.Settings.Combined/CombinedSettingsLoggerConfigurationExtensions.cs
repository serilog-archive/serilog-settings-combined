using System;
using Serilog.Configuration;
using Serilog.Settings.Combined;

namespace Serilog
{
    public static class CombinedSettingsLoggerConfigurationExtensions
    {
        public static LoggerConfiguration Combined(this LoggerSettingsConfiguration lsc, Func<IConfigBuilder, IConfigBuilder> build)
        {
            var configBuilder = new ConfigBuilder();
            configBuilder = (ConfigBuilder)build(configBuilder);
            var enumerable = configBuilder.BuildCombinedEnumerable();

            return lsc.KeyValuePairs(enumerable);
        }
    }
}
