using Codecaine.Common.CQRS.Queries;
using Codecaine.PeribahasaVector.Application.ViewModels;

namespace Codecaine.PeribahasaVector.Application.UseCases.Peribahasas.Queries.StreamPeribahasas
{
    /// <summary>
    /// Query for streaming Peribahasas in real-time.
    /// </summary>
    /// <param name="SearchTerm">Optional search term to filter results</param>
    /// <param name="MaxResults">Maximum number of results to stream</param>
    public record StreamPeribahasasQuery(string? SearchTerm = null, int MaxResults = 100) 
        : IQuery<IAsyncEnumerable<PeribahasaViewModel>>;
}
