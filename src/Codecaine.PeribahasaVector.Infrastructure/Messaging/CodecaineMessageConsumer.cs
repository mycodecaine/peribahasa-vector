using Codecaine.Common.Messaging.MassTransit;
using Microsoft.Extensions.Logging;

namespace Codecaine.PeribahasaVector.Infrastructure.Messaging
{
    public class CodecaineMessageConsumer : MessageQueueConsumer
    {
        public CodecaineMessageConsumer(ILogger<CodecaineMessageConsumer> logger, IServiceProvider serviceProvider) : base(logger, serviceProvider)
        {
        }
    }
}
