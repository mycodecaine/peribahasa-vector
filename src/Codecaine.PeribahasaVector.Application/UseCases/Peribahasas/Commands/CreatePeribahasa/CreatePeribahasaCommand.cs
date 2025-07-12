using Codecaine.Common.CQRS.Commands;
using Codecaine.Common.Primitives.Result;

namespace Codecaine.PeribahasaVector.Application.UseCases.Peribahasas.Commands.CreatePeribahasa
{
    /// <summary>
    /// CreatePeribahasaCommand is a command for creating a new Peribahasa.
    /// </summary>
    /// <param name="Teks"></param>
    /// <param name="Maksud"></param>
    /// <param name="TeksTranslation"></param>
    /// <param name="MaksudTranslation"></param>
    /// <param name="Context"></param>
    /// <param name="Source"></param>
    public record CreatePeribahasaCommand
    (
       string Teks,
       string Maksud,
       string TeksTranslation,
       string MaksudTranslation,
       string Context,
       string Source
    ): ICommand<Result<CreatePeribahasaCommandResponse>>;
}
