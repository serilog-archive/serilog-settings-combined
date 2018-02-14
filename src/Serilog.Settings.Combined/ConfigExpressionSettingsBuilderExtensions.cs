using System;
using System.Linq.Expressions;
using Serilog.Settings.Combined;
using Serilog.Settings.ConfigExpression;

namespace Serilog
{
    /// <summary>
    /// Extensions to allow combination of settings originating from config file appSettings
    /// </summary>
    public static class ConfigExpressionSettingsBuilderExtensions
    {
        /// <summary>
        /// Converts a configuration expression into a series of key-value pairs and add them to the pool of available settings
        /// </summary>
        /// <param name="builder">The combined settings builder</param>
        /// <param name="loggerConfigExpression">A configuration expression</param>
        /// <returns>An object allowing configuration to continue.</returns>
        public static ICombinedSettingsBuilder AddExpression(this ICombinedSettingsBuilder builder, Expression<Func<LoggerConfiguration, LoggerConfiguration>> loggerConfigExpression)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            if (loggerConfigExpression == null) throw new ArgumentNullException(nameof(loggerConfigExpression));

            var settingSource = new ConfigExpressionSettingsSource(loggerConfigExpression);

            return builder.AddKeyValuePairs(settingSource.GetKeyValuePairs());
        }
    }
}
