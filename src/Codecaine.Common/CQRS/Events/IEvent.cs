using MediatR;

namespace Codecaine.Common.CQRS.Events
{
    /// <summary>
    /// Represents the event interface.
    /// </summary>
    public interface IEvent : INotification
    {
    }
}
