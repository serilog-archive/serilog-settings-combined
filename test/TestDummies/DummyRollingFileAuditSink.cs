using System;
using System.Collections.Generic;
using Serilog.Core;
using Serilog.Events;

namespace TestDummies
{
    public class DummyRollingFileAuditSink : ILogEventSink
    {
        public DummyRollingFileAuditSink(string pathFormat, string outputTemplate)
        {
            PathFormat = pathFormat;
            OutputTemplate = outputTemplate;
        }

        [ThreadStatic]
        static List<LogEvent> _emitted;

        public static List<LogEvent> Emitted
        {
            get
            {
                if (_emitted == null)
                {
                    _emitted = new List<LogEvent>();
                }
                return _emitted;
            }
        }

        [ThreadStatic]
        public static string OutputTemplate;

        [ThreadStatic]
        public static string PathFormat;

        public void Emit(LogEvent logEvent)
        {
            Emitted.Add(logEvent);
        }

        public static void Reset()
        {
            _emitted = null;
            OutputTemplate = null;
            PathFormat = null;
        }
    }
}
