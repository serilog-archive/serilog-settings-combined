using System;
using System.Collections.Generic;
using Serilog.Core;
using Serilog.Events;

namespace TestDummies
{
    public class DummyRollingFileSink : ILogEventSink
    {
        [ThreadStatic]
        // ReSharper disable ThreadStaticFieldHasInitializer
        public static List<LogEvent> Emitted = new List<LogEvent>();
        // ReSharper restore ThreadStaticFieldHasInitializer

        public DummyRollingFileSink(string pathFormat, string outputTemplate)
        {
            PathFormat = pathFormat;
            OutputTemplate = outputTemplate;
        }

        [ThreadStatic]
        public static string OutputTemplate;

        [ThreadStatic]
        public static string PathFormat;

        public void Emit(LogEvent logEvent)
        {
            Emitted.Add(logEvent);
        }
    }
}
