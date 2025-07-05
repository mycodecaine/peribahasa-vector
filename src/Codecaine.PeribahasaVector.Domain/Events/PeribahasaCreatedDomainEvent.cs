using Codecaine.Common.Domain.Events;
using Codecaine.PeribahasaVector.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
