﻿namespace Codecaine.Common.Abstractions
{
    /// <summary>
    /// Generate Correlation Id
    /// Notes https://chatgpt.com/share/6799684c-2d80-8007-947a-23e1736519ac
    /// </summary>
    public interface ICorrelation
    {
        Guid CorrelationId { get; }
    }
}
