using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codecaine.Common.CQRS.Events
{
    /// <summary>
    /// Interface for handling integration events.
    /// </summary>
    /// <typeparam name="TIntegrationEvent">The type of the integration event.</typeparam>
    public interface IIntegrationEventHandler<in TIntegrationEvent> : INotificationHandler<TIntegrationEvent>
        where TIntegrationEvent : IIntegrationEvent
    {
    }
}
