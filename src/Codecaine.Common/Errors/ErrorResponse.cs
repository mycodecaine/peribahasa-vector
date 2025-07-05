using Codecaine.Common.Primitives.Errors;

namespace Codecaine.Common.Errors
{
    public class ErrorResponse
    {
        public ErrorResponse(IReadOnlyCollection<Error> errors) => Errors = errors;

        /// <summary>
        /// Gets the errors.
        /// </summary>
        public IReadOnlyCollection<Error> Errors { get; }
    }
}
