﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Serilog.Events;
using Serilog.Settings.ConfigExpression.Tests.Support;
using TestDummies;
using Xunit;

namespace Serilog.Settings.ConfigExpression.Tests
{
    public class ConfigurationExpressionSettingsSerializerTests
    {
        [Fact]
        public void SupportMinimumLevel()
        {
            var actual = new ConfigurationExpressionSettingsSerializer()
                .SerializeToKeyValuePairs(lc =>
                    lc
                        .MinimumLevel.Verbose()
                        .MinimumLevel.Debug()
                        .MinimumLevel.Information()
                        .MinimumLevel.Warning()
                        .MinimumLevel.Error()
                        .MinimumLevel.Fatal()
                ).ToList();

            var expected = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("minimum-level", "Verbose"),
                new KeyValuePair<string, string>("minimum-level", "Debug"),
                new KeyValuePair<string, string>("minimum-level", "Information"),
                new KeyValuePair<string, string>("minimum-level", "Warning"),
                new KeyValuePair<string, string>("minimum-level", "Error"),
                new KeyValuePair<string, string>("minimum-level", "Fatal")
            };

            Assert.Equal(expected.ToList(), actual, new KeyValuePairComparer<string, string>());
        }

        [Fact]
        public void SupportMinimumLevelIs()
        {
            var actual = new ConfigurationExpressionSettingsSerializer()
                .SerializeToKeyValuePairs(lc =>
                    lc
                        .MinimumLevel.Is(LogEventLevel.Verbose)
                        .MinimumLevel.Is(LogEventLevel.Debug)
                        .MinimumLevel.Is(LogEventLevel.Information)
                        .MinimumLevel.Is(LogEventLevel.Warning)
                        .MinimumLevel.Is(LogEventLevel.Error)
                        .MinimumLevel.Is(LogEventLevel.Fatal)
                ).ToList();

            var expected = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("minimum-level", "Verbose"),
                new KeyValuePair<string, string>("minimum-level", "Debug"),
                new KeyValuePair<string, string>("minimum-level", "Information"),
                new KeyValuePair<string, string>("minimum-level", "Warning"),
                new KeyValuePair<string, string>("minimum-level", "Error"),
                new KeyValuePair<string, string>("minimum-level", "Fatal")
            };

            Assert.Equal(expected.ToList(), actual, new KeyValuePairComparer<string, string>());
        }

        [Fact]
        public void SupportMinimumLevelOverrides()
        {
            var actual = new ConfigurationExpressionSettingsSerializer()
                .SerializeToKeyValuePairs(lc =>
                    lc
                        .MinimumLevel.Override("Foo", LogEventLevel.Error)
                        .MinimumLevel.Override("Bar.Qux", LogEventLevel.Warning)
                ).ToList();

            var expected = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("minimum-level:override:Foo", "Error"),
                new KeyValuePair<string, string>("minimum-level:override:Bar.Qux", "Warning")
            };

            Assert.Equal(expected.ToList(), actual, new KeyValuePairComparer<string, string>());
        }

        [Fact]
        public void SupportEnrichWithProperty()
        {
            var actual = new ConfigurationExpressionSettingsSerializer()
                .SerializeToKeyValuePairs(lc =>
                    lc
                        .Enrich.WithProperty("Prop1", "Prop1Value", false)
                        .Enrich.WithProperty("Prop2", 42, false)
                        .Enrich.WithProperty("Prop3", new Uri("https://www.perdu.com/bar"), false)
                        .Enrich.WithProperty("Prop4", true, false)
                ).ToList();

            var expected = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("enrich:with-property:Prop1", "Prop1Value"),
                new KeyValuePair<string, string>("enrich:with-property:Prop2", "42"),
                new KeyValuePair<string, string>("enrich:with-property:Prop3", "https://www.perdu.com/bar"),
                new KeyValuePair<string, string>("enrich:with-property:Prop4", "True"),
            };

            Assert.Equal(expected.ToList(), actual, new KeyValuePairComparer<string, string>());
        }

        [Fact]
        public void SupportEnrichWithExtensionMethod()
        {
            var actual = new ConfigurationExpressionSettingsSerializer()
                .SerializeToKeyValuePairs(lc =>
                    lc.Enrich.WithDummyThreadId()
                ).ToList();

            var expected = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("using:TestDummies", typeof(DummyLoggerConfigurationExtensions).GetTypeInfo().Assembly.FullName),
                new KeyValuePair<string, string>("enrich:WithDummyThreadId", "")
            };

            Assert.Equal(expected.ToList(), actual, new KeyValuePairComparer<string, string>());
        }


        [Fact]
        public void SupportEnrichFromLogContext()
        {
            var actual = new ConfigurationExpressionSettingsSerializer()
                .SerializeToKeyValuePairs(lc =>
                    lc.Enrich.FromLogContext()
                ).ToList();

            var expected = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("enrich:FromLogContext", "")
            };

            Assert.Equal(expected.ToList(), actual, new KeyValuePairComparer<string, string>());
        }

        [Fact]
        public void SupportWriteTo()
        {
            var actual = new ConfigurationExpressionSettingsSerializer()
                .SerializeToKeyValuePairs(lc =>
                    lc.WriteTo.DummyRollingFile(
                            @"C:\toto.log",
                            LogEventLevel.Warning,
                            null,
                            null)
                ).ToList();

            var expected = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("using:TestDummies", typeof(DummyLoggerConfigurationExtensions).GetTypeInfo().Assembly.FullName),
                new KeyValuePair<string, string>("write-to:DummyRollingFile.pathFormat", @"C:\toto.log"),
                new KeyValuePair<string, string>("write-to:DummyRollingFile.restrictedToMinimumLevel", "Warning")
            };

            Assert.Equal(expected.ToList(), actual, new KeyValuePairComparer<string, string>());
        }

        [Fact]
        public void SupportAuditTo()
        {
            var actual = new ConfigurationExpressionSettingsSerializer()
                .SerializeToKeyValuePairs(lc =>
                    lc
                        .AuditTo.DummyRollingFile(
                            @"C:\toto.log",
                            LogEventLevel.Warning,
                            null,
                            null)
                ).ToList();

            var expected = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("using:TestDummies", typeof(DummyLoggerConfigurationExtensions).GetTypeInfo().Assembly.FullName),
                new KeyValuePair<string, string>("audit-to:DummyRollingFile.pathFormat", @"C:\toto.log"),
                new KeyValuePair<string, string>("audit-to:DummyRollingFile.restrictedToMinimumLevel", "Warning")
            };

            Assert.Equal(expected.ToList(), actual, new KeyValuePairComparer<string, string>());
        }

        [Fact]
        public void SupportFilter()
        {
            var actual = new ConfigurationExpressionSettingsSerializer()
                .SerializeToKeyValuePairs(lc =>
                    lc.Filter.ByExcluding("filter = 'exclude'")
                ).ToList();

            var expected = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("using:Serilog.Filters.Expressions", "Serilog.Filters.Expressions, Version=1.1.0.0, Culture=neutral, PublicKeyToken=24c2f752a8e58a10"),
                new KeyValuePair<string, string>("filter:ByExcluding.expression", "filter = 'exclude'"),
            };

            Assert.Equal(expected.ToList(), actual, new KeyValuePairComparer<string, string>());
        }

        // TODO : support for simple cases of default constructor of an implementation of abstract type
        // TODO : special handling of "default" value of parameters -> do not generate a kvp in that case ?
        // TODO : support for the static member syntax ....
        // TODO : support for Filter ??? how ?
        // TODO : short assembly name in Using statements ? 

    }
}
