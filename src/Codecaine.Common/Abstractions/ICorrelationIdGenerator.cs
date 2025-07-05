namespace Codecaine.Common.Abstractions
{
    /// <summary>
    /// Generate Correlation Id
    /// Notes : https://chatgpt.com/share/6799684c-2d80-8007-947a-23e1736519ac
    /// </summary>
    public interface ICorrelationIdGenerator
    {
        /// <summary>
        /// Gets the current correlation ID.
        /// </summary>
        /// <returns>The current correlation ID.</returns>
        Guid Get();

        /// <summary>
        /// Sets the correlation ID.
        /// </summary>
        /// <param name="correlationId">The correlation ID to set.</param>
        Guid Set();
    }
}
