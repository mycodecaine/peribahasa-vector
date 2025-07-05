using Codecaine.Common.Abstractions;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codecaine.Common.CQRS.Events
{
    /// <summary>
    /// Represents the marker interface for an integration event.
    /// </summary>
    public interface IIntegrationEvent : INotification, ICorrelation
    {
    }

}
