using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Serilog.Settings.Combined.Tests
{
    public class ConfigBuilderTests
    {

        [Fact]
        public void ConfigBuilderConsumesEnumerablesAsLateAsPossible()
        {
            var consumeCount = 0;

            IEnumerable<KeyValuePair<string, string>> Enumerable1()
            {
                consumeCount++;
                yield break;
            }

            IEnumerable<KeyValuePair<string, string>> Enumerable2()
            {
                consumeCount++;
                yield break;
            }

            var builder = new ConfigBuilder();
            builder.AddSource(Enumerable1());
            builder.AddSource(Enumerable2());
            Assert.Equal(0, consumeCount);

            var combined = builder.BuildCombinedEnumerable();
            Assert.Equal(0, consumeCount);

            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed 
            combined.ToList();

            Assert.Equal(2, consumeCount);
        }
    }
}
