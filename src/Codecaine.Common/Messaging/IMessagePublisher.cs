using Codecaine.Common.CQRS.Events;

namespace Codecaine.Common.Messaging
{
    public interface IMessagePublisher
    {
        Task PublishIntegrationEventAsync<T>(T message) where T : IIntegrationEvent;
        Task PublishIntegrationEventAsync<T>(T message, string queue) where T : IIntegrationEvent;
        Task PublishMessageAsync<T>(T message) where T : class;
        Task PublishMessageAsync<T>(T message, string queue) where T : class;
    }
}
