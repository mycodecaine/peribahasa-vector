using AutoMapper;
using Codecaine.Common.CQRS.Base;
using Codecaine.PeribahasaVector.Application.ViewModels;
using Codecaine.PeribahasaVector.Domain.Repositories;
using Microsoft.Extensions.Logging;
using System.Runtime.CompilerServices;

namespace Codecaine.PeribahasaVector.Application.UseCases.Peribahasas.Queries.StreamPeribahasas
{
    /// <summary>
    /// Handler for streaming Peribahasas query.
    /// Returns results as an async stream for real-time processing.
    /// </summary>
    public class StreamPeribahasasQueryHandler : QueryHandler<StreamPeribahasasQuery, IAsyncEnumerable<PeribahasaViewModel>>
    {
        private readonly IPeribahasaRepository _repository;
        private readonly ILogger<StreamPeribahasasQueryHandler> _logger;
        private readonly IMapper _mapper;

        public StreamPeribahasasQueryHandler(
            ILogger<StreamPeribahasasQueryHandler> logger,
            IPeribahasaRepository repository,
 IMapper mapper) : base(logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
_logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public override Task<IAsyncEnumerable<PeribahasaViewModel>> Handle(
            StreamPeribahasasQuery request,
         CancellationToken cancellationToken)
        {
            _logger.LogInformation("Streaming Peribahasas with SearchTerm: {SearchTerm}, MaxResults: {MaxResults}",
   request.SearchTerm, request.MaxResults);

        var stream = StreamPeribahasasAsync(request, cancellationToken);
            return Task.FromResult(stream);
        }

      private async IAsyncEnumerable<PeribahasaViewModel> StreamPeribahasasAsync(
   StreamPeribahasasQuery request,
      [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            IEnumerable<Domain.Entities.Peribahasa> peribahasas;

      // If search term is provided, use vector search
            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
    {
                var searchResults = await _repository.SearchEntityByVectorAsync(
               request.SearchTerm,
           request.MaxResults);

    peribahasas = searchResults.Select(r => r.entity);
            }
   else
  {
         // Otherwise get paged results
      var (items, _) = await _repository.GetPagedAsync(
        page: 1,
     pageSize: request.MaxResults,
         cancellationToken: cancellationToken);

     peribahasas = items;
            }

// Stream each result individually
       foreach (var peribahasa in peribahasas)
         {
          if (cancellationToken.IsCancellationRequested)
         {
          _logger.LogWarning("Streaming cancelled");
                    yield break;
    }

                var viewModel = _mapper.Map<PeribahasaViewModel>(peribahasa);

         // Optional: Add small delay to simulate real-time streaming
      await Task.Delay(10, cancellationToken);
   
         yield return viewModel;
       }

    _logger.LogInformation("Streaming completed");
   }
    }
}
