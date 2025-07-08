namespace Codecaine.PeribahasaVector.Presentation.WebApi.DTOs.Peribahasas
{
    public record PeribahasaDto
    (
       string Teks,
       string Maksud,
       string TeksTranslation,
       string MaksudTranslation,
       string Context,
       string Source
    );
}
