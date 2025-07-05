using Codecaine.Common.Abstractions;

namespace Codecaine.Common.Domain.Events
{
    /// <summary>
    /// Represents a base class for domain event handlers.
    /// </summary>
    /// <typeparam name="TDomainEvent">The domain event type.</typeparam>
    public abstract class BaseDomainEventHandler<TDomainEvent> : IDomainEventHandler<TDomainEvent> where TDomainEvent : IDomainEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseDomainEventHandler{TDomainEvent}"/> class.
        /// </summary>
        /// <param name="correlationIdGenerator">The correlation ID generator.</param>
        protected BaseDomainEventHandler(ICorrelationIdGenerator correlationIdGenerator)
        {
            CorrelationId = correlationIdGenerator.Get().ToString();
        }

        /// <summary>
        /// Handles the specified domain event.
        /// </summary>
        /// <param name="notification">The domain event notification.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public abstract Task Handle(TDomainEvent notification, CancellationToken cancellationToken);

        /// <summary>
        /// Gets the correlation ID.
        /// </summary>
        public string CorrelationId { get; }
    }
}
