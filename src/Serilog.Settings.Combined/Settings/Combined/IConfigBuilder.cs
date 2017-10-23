using System.Collections.Generic;

namespace Serilog.Settings.Combined
{
    public interface IConfigBuilder
    {
        IConfigBuilder AddSource(IEnumerable<KeyValuePair<string, string>> source);
    }
}
