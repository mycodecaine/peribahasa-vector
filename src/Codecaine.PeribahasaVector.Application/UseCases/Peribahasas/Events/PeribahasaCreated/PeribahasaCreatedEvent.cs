using Codecaine.Common.CQRS.Events;
using Codecaine.PeribahasaVector.Domain.Events;
using Newtonsoft.Json;

namespace Codecaine.PeribahasaVector.Application.UseCases.Peribahasas.Events.PeribahasaCreated
{
    /// <summary>
    /// PeribahasaCreatedEvent is an integration event that is triggered when a new Peribahasa is created.
    /// </summary>
    internal class PeribahasaCreatedEvent : IIntegrationEvent
    {
        /// <summary>
        /// PeribahasaCreatedEvent constructor that initializes the event with a PeribahasaCreatedDomainEvent and a correlation ID.
        /// </summary>
        /// <param name="peribahasaCreatedDomainEvent"></param>
        /// <param name="correlationId"></param>
        internal PeribahasaCreatedEvent(PeribahasaCreatedDomainEvent peribahasaCreatedDomainEvent, Guid correlationId)
        {
            PeribahasaId = peribahasaCreatedDomainEvent.Peribahasa.Id;
           
            CorrelationId = correlationId;
        }

        /// <summary>
        /// PeribahasaCreatedEvent constructor for deserialization purposes.
        /// </summary>
        /// <param name="peribahasaId"></param>
        /// <param name="correlationId"></param>
        [JsonConstructor]
        private PeribahasaCreatedEvent(Guid peribahasaId, Guid correlationId)
        {
            PeribahasaId = peribahasaId;
            CorrelationId = correlationId;
        }

        public Guid PeribahasaId { get; private set; }
        public Guid CorrelationId { get; private set; }
    }
}
