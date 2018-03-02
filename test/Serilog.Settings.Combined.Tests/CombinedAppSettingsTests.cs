#if APPSETTINGS
using System.Linq;
using Serilog.Events;
using Serilog.Tests.Support;
using TestDummies;
using Xunit;

namespace Serilog.Settings.Combined.Tests
{
    public class CombinedAppSettingsTests
    {
        public CombinedAppSettingsTests()
        {
            DummyRollingFileSink.Reset();
        }

        [Fact]
        public void CombinedCanMergeSettingsFromMultipleConfigFiles()
        {
            LogEvent evt = null;
            var log = new LoggerConfiguration()
                .ReadFrom.Combined(builder => builder
                        .AddAppSettings(filePath: "Samples/Initial.config")
                        .AddAppSettings(filePath: "Samples/Second.config")
                        .AddAppSettings(filePath: "Samples/Third.config")
                    )
                .WriteTo.Sink(new DelegatingSink(e => evt = e))
                .CreateLogger();

            log.Information("Has a test property");
            Assert.True(DummyRollingFileSink.Emitted.Any(), "Events should be written to DummyRollingFile");
            Assert.Equal("DefinedInThird", DummyRollingFileSink.PathFormat);
            Assert.Equal("DefinedInSecond", DummyRollingFileSink.OutputTemplate);

            Assert.NotNull(evt);
            Assert.Equal("OverridenInThird", evt.Properties["AppName"].LiteralValue());
            Assert.Equal("DeclaredInSecond", evt.Properties["ServerName"].LiteralValue());
        }

        [Fact]
        public void CombinedCanMergeAppSettingsWithInMemoryKeyValuePairs()
        {
            LogEvent evt = null;
            var log = new LoggerConfiguration()
                .ReadFrom.Combined(builder => builder
                    .AddKeyValuePair("minimum-level", "Verbose")
                    .AddKeyValuePair("using:TestDummies", "TestDummies")
                    .AddKeyValuePair("write-to:DummyRollingFile.restrictedToMinimumLevel", "Debug")
                    .AddKeyValuePair("enrich:with-property:AppName", "DeclaredInKeyValuePairs")
                    .AddAppSettings(filePath: "Samples/Second.config")
                    .AddKeyValuePair("write-to:DummyRollingFile.pathFormat", "DefinedInKeyValuePairs")
                    .AddKeyValuePair("enrich:with-property:AppName", "OverridenInKeyValuePairs")
                )
                .WriteTo.Sink(new DelegatingSink(e => evt = e))
                .CreateLogger();

            log.Information("Has a test property");
            Assert.True(DummyRollingFileSink.Emitted.Any(), "Events should be written to DummyRollingFile");
            Assert.Equal("DefinedInKeyValuePairs", DummyRollingFileSink.PathFormat);
            Assert.Equal("DefinedInSecond", DummyRollingFileSink.OutputTemplate);

            Assert.NotNull(evt);
            Assert.Equal("OverridenInKeyValuePairs", evt.Properties["AppName"].LiteralValue());
            Assert.Equal("DeclaredInSecond", evt.Properties["ServerName"].LiteralValue());
        }
    }
}

#endif
