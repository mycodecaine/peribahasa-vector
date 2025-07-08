using Codecaine.Common.Errors;
using Codecaine.Common.Primitives.Errors;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Codecaine.PeribahasaVector.Presentation.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseController : ControllerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseController"/> class.
        /// </summary>
        /// <param name="mediator">The mediator instance.</param>
        protected BaseController(IMediator mediator) => Mediator = mediator;

        /// <summary>
        /// Gets the mediator instance.
        /// </summary>
        protected IMediator Mediator { get; }

        /// <summary>
        /// Returns a BadRequest response with the specified error.
        /// </summary>
        /// <param name="error">The error to include in the response.</param>
        /// <returns>A BadRequest response.</returns>
        protected IActionResult BadRequest(Error error) => BadRequest(new ErrorResponse(new[] { error }));

        /// <summary>
        /// Returns an Ok (200) response with the specified value.
        /// </summary>
        /// <param name="value">The value to include in the response.</param>
        /// <returns>An Ok response.</returns>
        protected new IActionResult Ok(object value) => base.Ok(value);

        /// <summary>
        /// Returns an Ok (200) response with the specified boolean value.
        /// </summary>
        /// <param name="value">The boolean value to include in the response.</param>
        /// <returns>An Ok response.</returns>
        protected IActionResult Ok(bool value) => base.Ok(value);

        /// <summary>
        /// Returns a NotFound (404) response.
        /// </summary>
        /// <returns>A NotFound response.</returns>
        protected new IActionResult NotFound() => base.NotFound();



        /// <summary>
        /// Returns a Created (201) response with the specified result.
        /// </summary>
        /// <param name="result">The result to include in the response.</param>
        /// <returns>A Created response.</returns>
        protected IActionResult Created(object result) => base.Created(nameof(result), result);
    }
}
