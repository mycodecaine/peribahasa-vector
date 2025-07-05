using Codecaine.Common.Primitives.Errors;

namespace Codecaine.Common.Exceptions
{
    /// <summary>
    /// Represents errors that occur during application execution that are not expected to be recovered from.
    /// This exception should be thrown when an unrecoverable error happens within the Application Layer.
    /// </summary>
    public class ApplicationLayerException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationLayerException"/> class with a specified <see cref="Error"/>.
        /// </summary>
        /// <param name="error">The application-specific error associated with the exception.</param>
        public ApplicationLayerException(ApplicationError error)
            : base(error.Message)
            => Error = error;

        /// <summary>
        /// Gets the associated <see cref="Error"/> that describes the current exception.
        /// </summary>
        public Error Error { get; }
    }

}
