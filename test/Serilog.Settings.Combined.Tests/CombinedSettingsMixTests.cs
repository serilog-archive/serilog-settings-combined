#if APPSETTINGS
using System.Linq;
using Serilog.Events;
using Serilog.Tests.Support;
using TestDummies;
using Xunit;

namespace Serilog.Settings.Combined.Tests
{
    public class CombinedSettingsMixTests
    {
        [Fact]
        public void CombinedCanMixAndMatchMultipleSources()
        {
            LogEvent evt = null;
            // typical scenario : doing most of the config in code / default value
            // .... and override a few things from config files
            var log = new LoggerConfiguration()
                .ReadFrom.Combined(builder => builder
                    .AddKeyValuePair("minimum-level", "Verbose")
                    .AddKeyValuePair("enrich:with-property:AppName", "DeclaredInInitial")
                    .AddKeyValuePair("using:TestDummies" ,"TestDummies")
                    .AddKeyValuePair("write-to:DummyRollingFile.pathFormat", "DeclaredInInitial")
                    .AddAppSettings(filePath: "Samples/ConfigOverrides.config")
                    .AddKeyValuePair("enrich:with-property:ExtraProp", "AddedAtTheVeryEnd")
                )
                .WriteTo.Sink(new DelegatingSink(e => evt = e))
                .CreateLogger();

            log.Information("Has a test property");
            Assert.True(DummyRollingFileSink.Emitted.Any(), "Events should be written to DummyRollingFile");
            Assert.Equal("DefinedInConfigFile", DummyRollingFileSink.PathFormat);

            Assert.NotNull(evt);
            Assert.Equal("DeclaredInInitial", evt.Properties["AppName"].LiteralValue());
            Assert.Equal("DeclaredInConfigFile", evt.Properties["ServerName"].LiteralValue());
            Assert.Equal("AddedAtTheVeryEnd", evt.Properties["ExtraProp"].LiteralValue());
        }
    }
}
#endif
