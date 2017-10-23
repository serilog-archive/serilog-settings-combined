using System.Collections.Generic;

namespace Serilog.Settings.Combined
{
    /// <summary>
    /// The entry point for fluent combination of multiple source of settings
    /// </summary>
    public interface IConfigBuilder
    {
        /// <summary>
        /// Adds a source of key-value settings to the builder, after all the previously declared sources
        /// </summary>
        /// <param name="source">an enumerable collection of settings</param>
        /// <returns>the builder object to allow chaining</returns>
        IConfigBuilder AddSource(IEnumerable<KeyValuePair<string, string>> source);
    }
}
