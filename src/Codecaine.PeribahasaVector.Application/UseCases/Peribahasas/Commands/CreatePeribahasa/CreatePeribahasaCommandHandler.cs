using Codecaine.Common.CQRS.Base;
using Codecaine.Common.Persistence.Dapper.Interfaces;
using Codecaine.Common.Primitives.Result;
using Codecaine.PeribahasaVector.Domain.Entities;
using Codecaine.PeribahasaVector.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Codecaine.PeribahasaVector.Application.UseCases.Peribahasas.Commands.CreatePeribahasa
{
    public sealed class CreatePeribahasaCommandHandler: CommandHandler<CreatePeribahasaCommand, Result<CreatePeribahasaCommandResponse>>
    {
        private readonly IPeribahasaRepository _repository;
        private readonly ILogger<CreatePeribahasaCommandHandler> _logger;
        private readonly IDapperUnitOfWork _unitOfWork;

        public CreatePeribahasaCommandHandler(IPeribahasaRepository repository, ILogger<CreatePeribahasaCommandHandler> logger, IDapperUnitOfWork unitOfWork):base(logger)
        {
            _repository = repository;
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public override Task<Result<CreatePeribahasaCommandResponse>> Handle(CreatePeribahasaCommand request, CancellationToken cancellationToken)
        => HandleSafelyAsync(async () =>
        {
            _logger.LogInformation("CreateDocumentCommandHandler: {Content}", request.Teks);

           
            var peribahasa = Peribahasa.Create(request.Teks, request.Maksud, request.TeksTranslation, request.MaksudTranslation, request.Context, request.Source);
            await _unitOfWork.StartTransactionAsync(Guid.NewGuid());
           
            await _repository.Insert(peribahasa);

            // Commit the transaction
            await _unitOfWork.CommitAsync(peribahasa);
            // Return the response with the created peribasa ID
            return Result<CreatePeribahasaCommandResponse>.Success(new CreatePeribahasaCommandResponse(peribahasa.Id));
        });
    }
}
