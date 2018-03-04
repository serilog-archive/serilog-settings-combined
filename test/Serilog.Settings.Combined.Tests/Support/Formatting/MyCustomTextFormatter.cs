using System;
using System.IO;
using Serilog.Events;
using Serilog.Formatting;

namespace Serilog.Settings.Combined.Tests.Support.Formatting
{
    public class MyCustomTextFormatter : ITextFormatter
    {
        public void Format(LogEvent logEvent, TextWriter output)
        {
            throw new NotImplementedException();
        }
    }
}
