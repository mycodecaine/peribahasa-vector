using Asp.Versioning;
using Codecaine.Common.Errors;
using Codecaine.Common.Primitives.Result;
using Codecaine.PeribahasaVector.Application.UseCases.Peribahasas.Commands.CreatePeribahasa;
using Codecaine.PeribahasaVector.Presentation.WebApi.DTOs.Peribahasas;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Codecaine.PeribahasaVector.Presentation.WebApi.Controllers
{
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class PeribahasaController : BaseController
    {
        public  PeribahasaController(IMediator mediator) : base(mediator)
        {
        }

        [HttpPost()]
        [ProducesResponseType(typeof(CreatePeribahasaCommandResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] PeribahasaDto request) =>
         await Result.Create(request, GeneralErrors.UnProcessableRequest)
             .Map(request => new CreatePeribahasaCommand(request.Teks, request.Maksud, request.TeksTranslation, 
                 request.MaksudTranslation, request.Context,request.Source))
             .Bind(command => Mediator.Send(command))
             .Match(Ok, BadRequest);
    }
}
