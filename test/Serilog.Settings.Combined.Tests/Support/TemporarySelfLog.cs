using System;
using System.Collections.Generic;
using Serilog.Debugging;
using Xunit.Abstractions;

namespace Serilog.Tests.Support
{
    public class TemporarySelfLog : IDisposable
    {
        TemporarySelfLog(Action<string> output)
        {
            SelfLog.Enable(output);
        }

        public void Dispose()
        {
            SelfLog.Disable();
        }

        public static IDisposable SaveTo(List<string> target)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            return new TemporarySelfLog(target.Add);
        }

        public static IDisposable WriteToXunitOutput(ITestOutputHelper outputHelper)
        {
            if (outputHelper == null) throw new ArgumentNullException(nameof(outputHelper));

            return new TemporarySelfLog(outputHelper.WriteLine);
        }
    }
}
