using Serilog.Core;
using Serilog.Events;

namespace TestDummies
{
    public class DummyUserNameEnricher : ILogEventEnricher
    {
        public const string PropertyName = "UserName";

        readonly string _userName;

        public DummyUserNameEnricher(string userName)
        {
            _userName = userName;
        }

        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            if (_userName != null)
            {
                logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty(PropertyName, _userName));
            }
        }
    }
}
