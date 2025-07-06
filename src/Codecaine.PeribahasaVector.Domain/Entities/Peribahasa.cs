using Codecaine.Common.Domain;
using Codecaine.Common.Domain.Interfaces;
using Codecaine.Common.Primitives.Ensure;
using Codecaine.PeribahasaVector.Domain.Events;

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
        public string TeksTranslation { get; private set; }

        /// <summary>
        /// Maksud translation in english
        /// </summary>
        public string MaksudTranslation { get; private set; }

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

        public Peribahasa(string teks, string maksud, string translation,string maksudTranslation, string context, string source) : base(Guid.NewGuid())

        {
            Ensure.NotEmpty(teks, "The teks is required.", nameof(Teks));
            Ensure.NotEmpty(maksud, "The maksud is required.", nameof(Maksud));            
            Ensure.NotEmpty(translation, "The translation is required.", nameof(TeksTranslation));
            Ensure.NotEmpty(maksudTranslation, "The maksudTranslation is required.", nameof(MaksudTranslation));
            Ensure.NotEmpty(context, "The Context is required.", nameof(Context));
            Ensure.NotEmpty(source, "The Source is required.", nameof(Source));

            Teks = teks;
            Maksud = maksud;
            TeksTranslation = translation;
            MaksudTranslation = maksudTranslation;
            Context = context;
            Source = source;
            Content = $"{teks} {maksud} {translation} {maksudTranslation} {context} {source}";

        }

        public static Peribahasa Create(string teks, string maksud, string translation, string maksudTranslation, string context, string source)
        {
            var peribahasa = new Peribahasa(teks, maksud, translation,maksudTranslation, context, source);

            peribahasa.AddDomainEvent(new PeribahasaCreatedDomainEvent(peribahasa));

            return peribahasa;
            
        }

    }
}
