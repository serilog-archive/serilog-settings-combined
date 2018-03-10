using System;
using System.Collections.Generic;
using Serilog.Settings.Combined;

namespace Serilog.Debugging
{
    /// <summary>
    /// Extensions to help in debugging Combined settings issues
    /// </summary>
    public static class CombinedSetingsBuilderDebuggingExtensions
    {
        /// <summary>
        /// Allows to inspect the results of settings combination
        /// </summary>
        /// <param name="self">The builder to allow calling in a fluent way</param>
        /// <param name="action">An action that will be called with a copy of the list of registered settings</param>
        /// <returns>An object allowing configuration to continue.</returns>
        public static CombinedSettingsBuilder Inspect(this CombinedSettingsBuilder self, Action<IEnumerable<KeyValuePair<string, string>>> action)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));
            if (action == null) throw new ArgumentNullException(nameof(action));

            var settings = self.BuildSettings();
            action(new List<KeyValuePair<string, string>>(settings));

            return self;
        }
    }
}
