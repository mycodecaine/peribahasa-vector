namespace Codecaine.PeribahasaVector.Application.UseCases.Peribahasas.Commands.CreatePeribahasa
{
    /// <summary>
    /// CreatePeribahasaCommandResponse is the response model for the CreatePeribahasa command.
    /// Represents the result of creating a new Peribahasa.
    /// </summary>
    /// <param name="Id"></param>
    public record CreatePeribahasaCommandResponse
    (
        Guid Id
    );
}
