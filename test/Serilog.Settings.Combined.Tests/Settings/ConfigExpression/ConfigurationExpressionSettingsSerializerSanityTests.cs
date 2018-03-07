using System;
using System.Collections;
using System.Linq;
using Serilog.Context;
using Serilog.Core;
using Serilog.Events;
using Serilog.Settings.Combined.Tests.Support;
using Serilog.Settings.Combined.Tests.Support.Formatting;
using Serilog.Tests.Support;
using TestDummies;
using TestDummies.Console;
using TestDummies.Console.Themes;
using Xunit;

using ConfigExpr = System.Linq.Expressions.Expression<System.Func<Serilog.LoggerConfiguration, Serilog.LoggerConfiguration>>;

namespace Serilog.Settings.Combined.Tests.Settings.ConfigExpression
{
    public class ConfigurationExpressionSettingsSerializerSanityTests
    {
        [Fact]
        public void MinimumLevel()
        {
            ConfigExpr expressionToTest = lc => lc
                .MinimumLevel.Warning();

            LogEvent evt = null;
            TestThatReadFromExpressionBehavesTheSameAsLoggerConfig(expressionToTest,
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
        public void MinimumLevelIs()
        {
            ConfigExpr expressionToTest = lc => lc
                .MinimumLevel.Is(LogEventLevel.Warning);

            LogEvent evt = null;
            TestThatReadFromExpressionBehavesTheSameAsLoggerConfig(expressionToTest,
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
            TestThatReadFromExpressionBehavesTheSameAsLoggerConfig(expressionToTest,
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

        [Fact]
        public void PropertyEnrichment()
        {
            ConfigExpr expressionToTest = lc => lc
                .Enrich.WithProperty("Property1", "PropertyValue1", /*destructureObjects*/ false)
                .Enrich.WithProperty("Property2", "PropertyValue2", /*destructureObjects*/ false);

            LogEvent evt = null;
            TestThatReadFromExpressionBehavesTheSameAsLoggerConfig(expressionToTest,
                arrange: lc =>
                {
                    evt = null;
                    return lc.WriteTo.Sink(new DelegatingSink(e => evt = e));
                },
                test: (testCase, logger) =>
                {
                    logger.Write(Some.InformationEvent());
                    Assert.NotNull(evt);
                    Assert.Equal("PropertyValue1", evt.Properties["Property1"].LiteralValue());
                    Assert.Equal("PropertyValue2", evt.Properties["Property2"].LiteralValue());
                });
        }

        [Fact]
        public void LogContextEnrichment()
        {
            ConfigExpr expressionToTest = lc => lc
                .Enrich.FromLogContext();

            LogEvent evt = null;
            TestThatReadFromExpressionBehavesTheSameAsLoggerConfig(expressionToTest,
                arrange: lc =>
                {
                    evt = null;
                    return lc.WriteTo.Sink(new DelegatingSink(e => evt = e));
                },
                test: (testCase, logger) =>
                {
                    using (LogContext.PushProperty("Property", "Value1"))
                    {
                        logger.Write(Some.InformationEvent());
                    }
                    Assert.NotNull(evt);
                    Assert.Equal("Value1", evt.Properties["Property"].LiteralValue());

                    evt = null;

                    using (LogContext.PushProperty("Property", "Value2"))
                    {
                        logger.Write(Some.InformationEvent());
                    }
                    Assert.NotNull(evt);
                    Assert.Equal("Value2", evt.Properties["Property"].LiteralValue());
                });
        }

        [Fact]
        public void ExtensionMethodEnrichment()
        {
            ConfigExpr expressionToTest = lc => lc
                .Enrich.WithDummyThreadId()
                .Enrich.WithDummyUserName("MyUserName");

            LogEvent evt = null;
            TestThatReadFromExpressionBehavesTheSameAsLoggerConfig(expressionToTest,
                arrange: lc =>
                {
                    evt = null;
                    return lc.WriteTo.Sink(new DelegatingSink(e => evt = e));
                },
                test: (testCase, logger) =>
                {
                    logger.Write(Some.InformationEvent());
                    Assert.NotNull(evt);
                    Assert.Equal("MyUserName", evt.Properties[DummyUserNameEnricher.PropertyName].LiteralValue());
                    Assert.NotNull(evt.Properties[DummyThreadIdEnricher.PropertyName].LiteralValue());
                });
        }

        [Fact]
        public void WriteToSinkWithSimpleParams()
        {
            ConfigExpr expressionToTest = lc => lc
                .WriteTo.DummyRollingFile(
                    /*pathFormat*/ @"C:\toto.log",
                    /*restrictedToMinimumLevel*/ LogEventLevel.Verbose,
                    /*outputTemplate*/ null,
                    /*formatProvider*/ null);

            DummyRollingFileSink.Reset();

            TestThatReadFromExpressionBehavesTheSameAsLoggerConfig(expressionToTest,
                arrange: lc => lc,
                test: (testCase, logger) =>
                {
                    logger.Write(Some.InformationEvent());
                    Assert.NotNull(DummyRollingFileSink.Emitted.FirstOrDefault());
                    Assert.Equal(@"C:\toto.log", DummyRollingFileSink.PathFormat);
                    Assert.Null(DummyRollingFileSink.OutputTemplate);
                    DummyRollingFileSink.Reset();
                });
        }

        [Fact]
        public void AuditToSinkWithSimpleParams()
        {
            ConfigExpr expressionToTest = lc => lc
                .AuditTo.DummyRollingFile(
                    /*pathFormat*/ @"C:\toto.log",
                    /*restrictedToMinimumLevel*/ LogEventLevel.Verbose,
                    /*outputTemplate*/ null,
                    /*formatProvider*/ null);

            DummyRollingFileAuditSink.Reset();

            TestThatReadFromExpressionBehavesTheSameAsLoggerConfig(expressionToTest,
                arrange: lc => lc,
                test: (testCase, logger) =>
                {

                    logger.Write(Some.InformationEvent());
                    Assert.NotNull(DummyRollingFileAuditSink.Emitted.FirstOrDefault());
                    Assert.Equal(@"C:\toto.log", DummyRollingFileAuditSink.PathFormat);
                    Assert.Null(DummyRollingFileAuditSink.OutputTemplate);
                    DummyRollingFileAuditSink.Reset();
                });
        }

        [Fact]
        public void FilterExpression()
        {
            ConfigExpr expressionToTest = lc => lc
                .Filter.ByExcluding("filter = 'exclude'");

            LogEvent evt = null;
            TestThatReadFromExpressionBehavesTheSameAsLoggerConfig(expressionToTest,
                arrange: lc =>
                {
                    evt = null;
                    return lc.WriteTo.Sink(new DelegatingSink(e => evt = e));
                },
                test: (testCase, logger) =>
                {

                    logger.ForContext("filter", "exclude").Information("This will not be logged because filter = exclude is set");
                    Assert.Null(evt);

                    logger.ForContext("filter", "keep it !").Information("This will be logged because filter will let it through");
                    Assert.NotNull(evt);
                });
        }

        [Fact]
        public void AbstractTypeOrInterfaceImplementationsWithDefaultConstructor()
        {
            ConfigExpr expressionToTest = lc => lc
                .WriteTo.DummyWithFormatter(LogEventLevel.Verbose, new MyCustomTextFormatter())
                .WriteTo.DummyConsole(LogEventLevel.Verbose, new MyCustomConsoleTheme())
                ;

            DummySink.Reset();
            DummyConsoleSink.Reset();
            TestThatReadFromExpressionBehavesTheSameAsLoggerConfig(expressionToTest,
                arrange: lc => lc,
                test: (testCase, logger) =>
                {
                    Assert.NotNull(DummySink.Formatter);
                    Assert.IsType<MyCustomTextFormatter>(DummySink.Formatter);

                    Assert.NotNull(DummyConsoleSink.Theme);
                    Assert.IsType<MyCustomConsoleTheme>(DummyConsoleSink.Theme);

                    DummySink.Reset();
                    DummyConsoleSink.Reset();
                });
        }

        [Fact]
        public void AbstractTypeOrInterfaceImplementationsThroughPublicStaticProperty()
        {
            ConfigExpr expressionToTest = lc => lc
                    .WriteTo.DummyWithFormatter(LogEventLevel.Verbose, CustomFormatters.Formatter)
                    .WriteTo.DummyConsole(LogEventLevel.Verbose, ConsoleThemes.Theme1)
                ;

            DummySink.Reset();
            DummyConsoleSink.Reset();
            TestThatReadFromExpressionBehavesTheSameAsLoggerConfig(expressionToTest,
                arrange: lc => lc,
                test: (testCase, logger) =>
                {
                    Assert.NotNull(DummySink.Formatter);
                    Assert.Equal(CustomFormatters.Formatter, DummySink.Formatter);

                    Assert.NotNull(DummyConsoleSink.Theme);
                    Assert.Equal(ConsoleThemes.Theme1, DummyConsoleSink.Theme);

                    DummySink.Reset();
                    DummyConsoleSink.Reset();
                });
        }

        [Fact]
        public void AbstractTypeOrInterfaceImplementationsThroughPublicStaticField()
        {
            ConfigExpr expressionToTest = lc => lc
                    .WriteTo.DummyWithFormatter(LogEventLevel.Verbose, CustomFormatters.FormatterField)
                    .WriteTo.DummyConsole(LogEventLevel.Verbose, ConsoleThemes.Theme1Field)
                ;

            DummySink.Reset();
            DummyConsoleSink.Reset();
            TestThatReadFromExpressionBehavesTheSameAsLoggerConfig(expressionToTest,
                arrange: lc => lc,
                test: (testCase, logger) =>
                {
                    Assert.NotNull(DummySink.Formatter);
                    Assert.Equal(CustomFormatters.FormatterField, DummySink.Formatter);

                    Assert.NotNull(DummyConsoleSink.Theme);
                    Assert.Equal(ConsoleThemes.Theme1Field, DummyConsoleSink.Theme);

                    DummySink.Reset();
                    DummyConsoleSink.Reset();
                });
        }

        void TestThatReadFromExpressionBehavesTheSameAsLoggerConfig(
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
