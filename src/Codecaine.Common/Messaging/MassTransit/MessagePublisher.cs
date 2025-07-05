using Codecaine.Common.Abstractions;
using Codecaine.Common.CQRS.Events;
using MassTransit;
using MassTransit.Transports;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Diagnostics;

namespace Codecaine.Common.Messaging.MassTransit
{
    /// <summary>  
    /// Provides functionality to publish messages and integration events using MassTransit.  
    /// </summary>  
    public class MessagePublisher : IMessagePublisher
    {
        private readonly IPublishEndpoint _publishEndPoint;
        private readonly ILogger<MessagePublisher> _logger;
        private readonly ICorrelationIdGenerator _correlationIdGenerator;

        /// <summary>  
        /// Initializes a new instance of the <see cref="MessagePublisher"/> class.  
        /// </summary>  
        /// <param name="publishEndPoint">The MassTransit publish endpoint.</param>  
        /// <param name="logger">The logger instance for logging operations.</param>  
        public MessagePublisher(IPublishEndpoint publishEndPoint, ILogger<MessagePublisher> logger, ICorrelationIdGenerator correlationIdGenerator)
        {
            _publishEndPoint = publishEndPoint;
            _logger = logger;
            _correlationIdGenerator = correlationIdGenerator;
        }

        /// <summary>  
        /// Publishes an integration event to the default queue.  
        /// </summary>  
        /// <typeparam name="T">The type of the integration event.</typeparam>  
        /// <param name="message">The integration event to publish.</param>  
        /// <returns>A task representing the asynchronous operation.</returns>  
        public Task PublishIntegrationEventAsync<T>(T message) where T : IIntegrationEvent
        {
            _logger.LogInformation("Publishing integration event of type {EventType}", typeof(T).Name);
            return PublishIntegrationEventAsync(message, "codecaine-message");
        }

        /// <summary>  
        /// Publishes an integration event to a specified queue.  
        /// </summary>  
        /// <typeparam name="T">The type of the integration event.</typeparam>  
        /// <param name="message">The integration event to publish.</param>  
        /// <param name="queue">The name of the queue to publish the event to.</param>  
        /// <returns>A task representing the asynchronous operation.</returns>  
        public async Task PublishIntegrationEventAsync<T>(T message, string queue) where T : IIntegrationEvent
        {
            _logger.LogInformation("Publishing integration event of type {EventType} to queue {Queue}", typeof(T).Name, queue);

            string payload = JsonConvert.SerializeObject(message, typeof(IIntegrationEvent), new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            });

            var wrapper = new MessageWrapper(payload);

            await _publishEndPoint.Publish(wrapper, context =>
            {
                var activity = Activity.Current;
                if (activity != null)
                {
                    context.Headers.Set("traceparent", activity.Id);
                    // Also include baggage or correlationId if needed
                    context.Headers.Set("correlation_id", _correlationIdGenerator.Get());
                    context.CorrelationId = _correlationIdGenerator.Get();

                }
            });
        }

        /// <summary>  
        /// Publishes a message to the default queue.  
        /// </summary>  
        /// <typeparam name="T">The type of the message.</typeparam>  
        /// <param name="message">The message to publish.</param>  
        /// <returns>A task representing the asynchronous operation.</returns>  
        public Task PublishMessageAsync<T>(T message) where T : class
        {
            _logger.LogInformation("Publishing message of type {MessageType}", typeof(T).Name);
            return PublishMessageAsync(message, "codecaine-message");
        }

        /// <summary>  
        /// Publishes a message to a specified queue.  
        /// </summary>  
        /// <typeparam name="T">The type of the message.</typeparam>  
        /// <param name="message">The message to publish.</param>  
        /// <param name="queue">The name of the queue to publish the message to.</param>  
        /// <returns>A task representing the asynchronous operation.</returns>  
        public Task PublishMessageAsync<T>(T message, string queue) where T : class
        {
            _logger.LogInformation("Publishing message of type {MessageType} to queue {Queue}", typeof(T).Name, queue);

            string payload = JsonConvert.SerializeObject(message, typeof(T), new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            });

            var wrapper = new MessageWrapper(payload);

            return _publishEndPoint.Publish(wrapper, context =>
            {
                var activity = Activity.Current;
                if (activity != null)
                {
                    context.Headers.Set("traceparent", activity.Id);
                    // Also include baggage or correlationId if needed
                    context.Headers.Set("correlation_id", _correlationIdGenerator.Get());
                    context.CorrelationId = _correlationIdGenerator.Get();

                }
            }); 

        }
    }
}
