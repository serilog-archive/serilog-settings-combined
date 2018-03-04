using Serilog.Formatting;

namespace Serilog.Settings.Combined.Tests.Support.Formatting
{
    public static class CustomFormatters
    {
        public static ITextFormatter Formatter { get; } = new MyCustomTextFormatter();
        public static ITextFormatter FormatterField = new MyCustomTextFormatter();
    }
}
