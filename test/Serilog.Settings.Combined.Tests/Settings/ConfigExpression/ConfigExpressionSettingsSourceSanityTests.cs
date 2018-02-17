using System;
using System.Collections;
using Serilog.Core;
using Serilog.Events;
using Serilog.Tests.Support;
using Xunit;

using ConfigExpr = System.Linq.Expressions.Expression<System.Func<Serilog.LoggerConfiguration, Serilog.LoggerConfiguration>>;

namespace Serilog.Settings.Combined.Tests.Settings.ConfigExpression
{
    public class ConfigExpressionSettingsSourceSanityTests
    {
        [Fact]
        public void MinimumLevel()
        {
            ConfigExpr expressionToTest = lc => lc
                .MinimumLevel.Warning();

            LogEvent evt = null;
            TestThatReadFromExpressionBehavesTheSameAsLogginConfig(expressionToTest,
                arrange: lc =>
               {
                   evt = null;
                   return lc.WriteTo.Sink(new DelegatingSink(e => evt = e));
               },
                test: (testCase, logger) =>
                {
                    logger.Write(Some.InformationEvent());
                    Assert.Null(evt);
                    logger.Write(Some.WarningEvent());
                    Assert.NotNull(evt);
                });
        }

        [Fact]
        public void MinimumLevelOverride()
        {
            ConfigExpr expressionToTest = lc => lc
                .MinimumLevel.Verbose()
                .MinimumLevel.Override("System.Collections", LogEventLevel.Error)
                .MinimumLevel.Override("System", LogEventLevel.Warning);

            LogEvent evt = null;
            TestThatReadFromExpressionBehavesTheSameAsLogginConfig(expressionToTest,
                arrange: lc =>
                {
                    evt = null;
                    return lc.WriteTo.Sink(new DelegatingSink(e => evt = e));
                },
                test: (testCase, logger) =>
                {
                    logger.Write(Some.InformationEvent());
                    Assert.NotNull(evt);

                    evt = null;
                    logger.ForContext<DateTime>().Write(Some.InformationEvent());
                    Assert.Null(evt);
                    logger.ForContext<DateTime>().Write(Some.WarningEvent());
                    Assert.NotNull(evt);

                    evt = null;
                    logger.ForContext<CollectionBase>().Write(Some.WarningEvent());
                    Assert.Null(evt);
                    logger.ForContext<CollectionBase>().Write(Some.ErrorEvent());
                    Assert.NotNull(evt);
                });
        }



        void TestThatReadFromExpressionBehavesTheSameAsLogginConfig(
            ConfigExpr expressionToTest,
            Func<LoggerConfiguration, LoggerConfiguration> arrange,
            Action<string, Logger> test
            )
        {
            var testCase = "Traditional";
            var loggerConfiguration = expressionToTest.Compile().Invoke(new LoggerConfiguration());
            loggerConfiguration = arrange(loggerConfiguration);
            var logger = loggerConfiguration.CreateLogger();

            test(testCase, logger);

            testCase = "ConfigExpression";
            var expressionLoggerConfiguration = new LoggerConfiguration()
                .ReadFrom.Combined(builder => builder.AddExpression(expressionToTest));
            expressionLoggerConfiguration = arrange(expressionLoggerConfiguration);
            var expressionLogger = expressionLoggerConfiguration.CreateLogger();

            test(testCase, expressionLogger);
        }
    }
}
