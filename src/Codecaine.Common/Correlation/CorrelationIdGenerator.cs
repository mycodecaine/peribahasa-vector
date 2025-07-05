using Codecaine.Common.Abstractions;
using OpenTelemetry;

namespace Codecaine.Common.Correlation
{
    public class CorrelationIdGenerator : ICorrelationIdGenerator
    {
        public Guid Get()
        {
            var correlationId = Baggage.GetBaggage("correlation_id");            
            if (Guid.TryParse(correlationId, out var guid))
            {
                return guid;
            }
            return Guid.Empty;
        }

        public Guid Set()
        {
            var correlationId =  Guid.NewGuid();
            Baggage.SetBaggage("correlation_id", correlationId.ToString());
            return correlationId;
        }
    }
}
