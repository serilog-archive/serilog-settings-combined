using System;
using Serilog.Core;
using Serilog.Events;

namespace TestDummies
{
    public class DummyThreadIdEnricher : ILogEventEnricher
    {
        public const string PropertyName = "ThreadId";

        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty(PropertyName, Guid.NewGuid()));
        }
    }
}
