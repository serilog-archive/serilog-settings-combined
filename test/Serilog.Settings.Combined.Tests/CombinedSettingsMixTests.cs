#if APPSETTINGS
using System;
using System.Linq;
using Serilog.Debugging;
using Serilog.Events;
using Serilog.Tests.Support;
using TestDummies;
using Xunit;
using Xunit.Abstractions;

namespace Serilog.Settings.Combined.Tests
{
    public class CombinedSettingsMixTests
    {
        readonly ITestOutputHelper _outputHelper;

        public CombinedSettingsMixTests(ITestOutputHelper outputHelper)
        {
            _outputHelper = outputHelper;
        }

        [Fact]
        public void CombinedCanMixAndMatchMultipleSources()
        {
            LogEvent evt = null;
            // typical scenario : doing most of the config in code / default value
            // .... and override a few things from config files
            var log = new LoggerConfiguration()
                .ReadFrom.Combined(builder => builder
                    .AddExpression(lc => lc
                        .MinimumLevel.Verbose()
                        .Enrich.WithProperty("AppName", "DeclaredInInitial", /*destructureObjects:*/ false)
                        .WriteTo.DummyRollingFile(/*Formatter*/ null, /*pathFormat*/ "overridenInConfigFile", /*restrictedToMinimumLevel*/ LogEventLevel.Verbose)
                    )
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

        [Fact]
        public void CombinedSettingsCanBeInspected()
        {
            new LoggerConfiguration()
                .ReadFrom.Combined(builder => builder
                    .AddExpression(lc => lc
                        .MinimumLevel.Verbose()
                        .Enrich.WithProperty("AppName", "DeclaredInInitial", /*destructureObjects:*/ false)
                        .WriteTo.DummyRollingFile( /*Formatter*/ null, /*pathFormat*/ "overridenInConfigFile", /*restrictedToMinimumLevel*/ LogEventLevel.Verbose)
                    )
                    .AddAppSettings(filePath: "Samples/ConfigOverrides.config")
                    .AddKeyValuePair("enrich:with-property:ExtraProp", "AddedAtTheVeryEnd")
                    .Inspect(kvps =>
                    {
                        _outputHelper.WriteLine("====Settings DUMP====");
                        _outputHelper.WriteLine(String.Join(Environment.NewLine, kvps.Select(kvp => kvp.ToString())));
                    })
                );
        }
    }
}
#endif
