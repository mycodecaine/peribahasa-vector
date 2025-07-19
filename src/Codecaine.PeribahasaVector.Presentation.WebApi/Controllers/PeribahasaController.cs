using Asp.Versioning;
using Codecaine.Common.Errors;
using Codecaine.Common.Primitives.Maybe;
using Codecaine.Common.Primitives.Result;
using Codecaine.PeribahasaVector.Application.UseCases.Peribahasas.Commands.CreatePeribahasa;
using Codecaine.PeribahasaVector.Application.UseCases.Peribahasas.Queries.SearchPeribahasaByVector;
using Codecaine.PeribahasaVector.Application.ViewModels;
using Codecaine.PeribahasaVector.Presentation.WebApi.DTOs.Peribahasas;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Codecaine.PeribahasaVector.Presentation.WebApi.Controllers
{
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class PeribahasaController : BaseController
    {
        public PeribahasaController(IMediator mediator) : base(mediator)
        {
        }

        [HttpPost()]
        [ProducesResponseType(typeof(CreatePeribahasaCommandResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] PeribahasaDto request) =>
         await Result.Create(request, GeneralErrors.UnProcessableRequest)
             .Map(request => new CreatePeribahasaCommand(request.Teks, request.Maksud, request.TeksTranslation,
                 request.MaksudTranslation, request.Context, request.Source))
             .Bind(command => Mediator.Send(command))
             .Match(Ok, BadRequest);

        [HttpGet("search/{content}")]
        [ProducesResponseType(typeof(List<PeribahasaViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Search(string content) =>
        await Maybe<SearchPeribahasaByVectorQuery>
            .From(new SearchPeribahasaByVectorQuery(content))
            .Bind(query => Mediator.Send(query))
            .Match(Ok, NotFound);
    }
}
