using Codecaine.Common.Abstractions;
using Codecaine.Common.Domain.Events;
using Codecaine.Common.Messaging;
using Codecaine.PeribahasaVector.Domain.Events;

namespace Codecaine.PeribahasaVector.Application.UseCases.Peribahasas.Events.PeribahasaCreated
{
    /// <summary>
    /// publishIntegrationEventOnPeribahasaCreatedDomainEventHandler is a domain event handler that publishes an integration event when a Peribahasa is created.
    /// </summary>
    internal class PublishIntegrationEventOnPeribahasaCreatedDomainEventHandler : IDomainEventHandler<PeribahasaCreatedDomainEvent>
    {
        private readonly IMessagePublisher _publisher;
        private readonly ICorrelationIdGenerator _correlationIdGenerator;

        public PublishIntegrationEventOnPeribahasaCreatedDomainEventHandler(IMessagePublisher publisher, ICorrelationIdGenerator correlationIdGenerator)
        {
            _publisher = publisher;
            _correlationIdGenerator = correlationIdGenerator;
        }
        public Task Handle(PeribahasaCreatedDomainEvent notification, CancellationToken cancellationToken)
        {
            var correlationId = _correlationIdGenerator.Get();

            _publisher.PublishIntegrationEventAsync(new PeribahasaCreatedEvent(notification, correlationId));

            return Task.CompletedTask;
        }
    }
}
