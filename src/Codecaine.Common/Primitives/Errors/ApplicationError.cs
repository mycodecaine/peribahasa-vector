namespace Codecaine.Common.Primitives.Errors
{


    public class ApplicationError : Error
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceApplicationError"/> class.
        /// </summary>
        /// <param name="code">The error code.</param>
        /// <param name="message">The error message.</param>
        public ApplicationError(string code, Exception message) : base($"ApplicationError.{code}", message.ToJsonString())
        {
        }
    }
}
