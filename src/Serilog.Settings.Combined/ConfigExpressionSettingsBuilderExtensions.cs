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

            var settingSource = new ConfigurationExpressionSettingsSerializer(loggerConfigExpression);

            return builder.AddKeyValuePairs(settingSource.GetKeyValuePairs());
        }
    }
}
