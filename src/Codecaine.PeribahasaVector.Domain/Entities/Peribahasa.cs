using Codecaine.Common.Domain;
using Codecaine.Common.Domain.Interfaces;
using Codecaine.PeribahasaVector.Domain.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codecaine.PeribahasaVector.Domain.Entities
{
    /// <summary>
    /// https://chatgpt.com/share/686928c2-f4d0-8007-8552-aa1bedc812df
    /// </summary>
    public class Peribahasa : AggregateVectorRoot, IAuditableEntity, ISoftDeletableEntity
    {
        /// <summary>
        /// Teks peribahasa 
        /// </summary>
        public string Teks { get; private set; }

        /// <summary>
        /// Maksud peribahasa
        /// </summary>
        public string Maksud { get; private set; }

        /// <summary>
        /// Translation in english
        /// </summary>
        public string Translation { get; private set; }

        /// <summary>
        /// Peribahasa context
        /// </summary>
        public string Context { get; private set; }

        /// <summary>
        /// Peribahasa source : Dewan Bahasa dan Pustaka, Sejarah Melayu, etc.
        /// </summary>
        public string Source { get; private set; }

        public DateTime CreatedOnUtc { get; private set; }

        public DateTime? ModifiedOnUtc { get; private set; }

        public Guid? CreatedBy { get; private set; }

        public Guid? ModifiedBy { get; private set; }

        public DateTime? DeletedOnUtc { get; private set; }

        public bool Deleted { get; private set; }

        public Peribahasa(string teks, string maksud, string translation, string context, string source) : base(Guid.NewGuid())

        {
            Teks = teks;
            Maksud = maksud;
            Translation = translation;
            Context = context;
            Source = source;
            Content = $"{teks} {maksud} {translation} {context} {source}";
        }

        public static Peribahasa Create(string teks, string maksud, string translation, string context, string source)
        {
            var peribahasa = new Peribahasa(teks, maksud, translation, context, source);

            peribahasa.AddDomainEvent(new PeribahasaCreatedDomainEvent(peribahasa));

            return peribahasa;
            
        }

    }
}
