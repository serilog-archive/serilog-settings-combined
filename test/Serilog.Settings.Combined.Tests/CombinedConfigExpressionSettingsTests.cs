using System.Linq;
using Serilog.Events;
using Serilog.Tests.Support;
using TestDummies;
using Xunit;

namespace Serilog.Settings.Combined.Tests
{
    public class CombinedConfigExpressionSettingsTests
    {
        public CombinedConfigExpressionSettingsTests()
        {
            DummyRollingFileSink.Reset();
        }

        [Fact]
        public void CombinedCanMergeSettingsFromMultipleConfigExpressions()
        {
            LogEvent evt = null;
            var log = new LoggerConfiguration()
                .ReadFrom.Combined(builder => builder
                    .AddExpression(lc => lc
                        .MinimumLevel.Verbose()
                        .Enrich.WithProperty("AppName", "DeclaredInInitial", /*destructureObjects:*/ false)
                        .WriteTo.DummyRollingFile(/*pathFormat*/ null, LogEventLevel.Debug, /*outputTemplate*/ null, /*formatProvider*/ null)
                    )
                    .AddExpression(lc => lc
                        .Enrich.WithProperty("ServerName", "DeclaredInSecond", /*destructureObjects:*/ false)
                        .WriteTo.DummyRollingFile(/*pathFormat*/ null, LogEventLevel.Debug, /*outputTemplate*/ "DefinedInSecond", /*formatProvider*/null)
                    )
                    .AddExpression(lc => lc
                        .Enrich.WithProperty("AppName", "OverridenInThird", /*destructureObjects:*/ false)
                        .WriteTo.DummyRollingFile(/*pathFormat*/ "DefinedInThird", LogEventLevel.Debug, /*outputTemplate*/ null, /*formatProvider*/null)
                    )
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
        public void CombinedCanMergeConfigExpressionWithInMemoryKeyValuePairs()
        {
            LogEvent evt = null;
            var log = new LoggerConfiguration()
                .ReadFrom.Combined(builder => builder
                    .AddKeyValuePair("minimum-level", "Verbose")
                    .AddKeyValuePair("using:TestDummies", "TestDummies")
                    .AddKeyValuePair("write-to:DummyRollingFile.restrictedToMinimumLevel", "Debug")
                    .AddKeyValuePair("enrich:with-property:AppName", "DeclaredInKeyValuePairs")
                    .AddExpression(lc => lc
                        .Enrich.WithProperty("ServerName", "DeclaredInSecond", /*destructureObjects:*/ false)
                        .WriteTo.DummyRollingFile(/*pathFormat*/ null, LogEventLevel.Debug, /*outputTemplate*/ "DefinedInSecond", /*formatProvider*/null)
                    )
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
