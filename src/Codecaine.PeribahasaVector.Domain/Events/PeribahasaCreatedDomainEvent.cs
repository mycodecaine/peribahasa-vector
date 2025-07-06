using Codecaine.Common.Domain.Events;
using Codecaine.PeribahasaVector.Domain.Entities;

namespace Codecaine.PeribahasaVector.Domain.Events
{
    /// <summary>
    /// PeribahasaCreatedDomainEvent is triggered when a new Peribahasa is created.
    /// </summary>
    public sealed class PeribahasaCreatedDomainEvent : IDomainEvent
    {

        public PeribahasaCreatedDomainEvent(Peribahasa peribahasa) => Peribahasa = peribahasa;
        public Peribahasa Peribahasa { get; }
    }

}
