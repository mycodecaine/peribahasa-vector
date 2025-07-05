using Codecaine.Common.Primitives.Errors;

namespace Codecaine.Common.Exceptions
{
  

    public class NotFoundException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InfrastructureException"/> class with a specified <see cref="Error"/>.
        /// </summary>
        /// <param name="error">The infrastructure-specific error associated with the exception.</param>
        public NotFoundException(Error error)
            : base(error.Message)
            => Error = error;

        /// <summary>
        /// Gets the associated <see cref="Error"/> that describes the infrastructure failure.
        /// </summary>
        public Error Error { get; }
    }
}
