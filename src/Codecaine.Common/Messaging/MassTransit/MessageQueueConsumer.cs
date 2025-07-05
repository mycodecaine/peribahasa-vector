using Codecaine.Common.CQRS.Events;
using Codecaine.Common.EventConsumer;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Codecaine.Common.Messaging.MassTransit
{
    public abstract class MessageQueueConsumer : IConsumer<MessageWrapper>
    {
        private readonly ILogger<MessageQueueConsumer> _logger;
        private readonly IServiceProvider _serviceProvider;

        protected MessageQueueConsumer(ILogger<MessageQueueConsumer> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        public Task Consume(ConsumeContext<MessageWrapper> context)
        {
            _logger.LogInformation("MessageQueueConsumer-Message: {Message}", context.Message.Message);
            _logger.LogInformation("MessageQueueConsumer-CorrelationId: {CorrelationId}", context.CorrelationId);


            var integrationEvent = JsonConvert.DeserializeObject<IIntegrationEvent>(context.Message.Message, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            });

            using IServiceScope scope = _serviceProvider.CreateScope();

            var integrationEventConsumer = scope.ServiceProvider.GetRequiredService<IIntegrationEventConsumer>();
            if (integrationEvent != null)
            {
                integrationEventConsumer.Consume(integrationEvent);
            }

            return Task.CompletedTask;
        }
    }
}
