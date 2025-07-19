using AutoMapper;
using Codecaine.Common.CQRS.Base;
using Codecaine.Common.Primitives.Maybe;
using Codecaine.PeribahasaVector.Application.ViewModels;
using Codecaine.PeribahasaVector.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Codecaine.PeribahasaVector.Application.UseCases.Peribahasas.Queries.SearchPeribahasaByVector
{
    public class SearchPeribahasaByVectorQueryHandler:QueryHandler<SearchPeribahasaByVectorQuery, Maybe<List<PeribahasaViewModel>>>
    {
        private readonly IPeribahasaRepository _repository;
        private readonly ILogger<SearchPeribahasaByVectorQueryHandler> _logger;
        private readonly IMapper _mapper;

        public SearchPeribahasaByVectorQueryHandler(ILogger<SearchPeribahasaByVectorQueryHandler> logger, IPeribahasaRepository peribahasaRepository, IMapper mapper) : base(logger)
        {
            _repository = peribahasaRepository ?? throw new ArgumentNullException(nameof(peribahasaRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async override Task<Maybe<List<PeribahasaViewModel>>> Handle(SearchPeribahasaByVectorQuery request, CancellationToken cancellationToken)
        {
            var result = await _repository.SearchEntityByVectorAsync(request.Content);
            if (!result.Any())
            {
                _logger.LogWarning("Content not found");
                return Maybe<List<PeribahasaViewModel>>.None;
            }

            var peribahasas = result.Select(p=>p.entity).ToList();

            //var viewModels = peribahasas.Select(p => new PeribahasaViewModel
            //(

            //    Teks: p.Teks,
            //    Maksud: p.Maksud,
            //    TeksTranslation: p.TeksTranslation,
            //    MaksudTranslation: p.MaksudTranslation,
            //    Context: p.Context,
            //    Source: p.Source
            //)).ToList();

            var viewModels = _mapper.Map<List<PeribahasaViewModel>>(peribahasas);


            return viewModels;
        }
    }
}
