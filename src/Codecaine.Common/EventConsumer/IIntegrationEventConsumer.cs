using Codecaine.Common.CQRS.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codecaine.Common.EventConsumer
{
    /// <summary>
    /// Defines a contract for consuming integration events.
    /// </summary>
    public interface IIntegrationEventConsumer
    {
        /// <summary>
        /// Consumes the incoming the specified integration event.
        /// </summary>
        void Consume(IIntegrationEvent integrationEvent);
    }
}
