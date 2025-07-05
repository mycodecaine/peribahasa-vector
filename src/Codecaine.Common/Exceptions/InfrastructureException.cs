using Codecaine.Common.Primitives.Errors;

namespace Codecaine.Common.Exceptions
{
    /// <summary>
    /// Represents errors that occur in the Infrastructure Layer, such as issues with external systems,
    /// databases, file systems, or network communication.
    /// This exception should be thrown when an unexpected infrastructure-related failure happens.
    /// </summary>
    public class InfrastructureException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InfrastructureException"/> class with a specified <see cref="Error"/>.
        /// </summary>
        /// <param name="error">The infrastructure-specific error associated with the exception.</param>
        public InfrastructureException(InfrastructureError error)
            : base(error.Message)
            => Error = error;

        /// <summary>
        /// Gets the associated <see cref="Error"/> that describes the infrastructure failure.
        /// </summary>
        public Error Error { get; }
    }
}
