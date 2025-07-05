using Codecaine.Common.Primitives.Errors;

namespace Codecaine.Common.Exceptions
{
    /// <summary>
    /// Represents errors that occur when a business rule or domain constraint is violated.
    /// This exception should be thrown from the Domain Layer to indicate invalid operations
    /// according to the domain rules.
    /// </summary>
    public class DomainException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DomainException"/> class with a specified <see cref="Error"/>.
        /// </summary>
        /// <param name="error">The domain-specific error associated with the exception.</param>
        public DomainException(Error error)  
            : base(error.Message)
            => Error = error;

        /// <summary>
        /// Gets the associated <see cref="Error"/> that describes the domain violation.
        /// </summary>
        public Error Error { get; }
    }
}
