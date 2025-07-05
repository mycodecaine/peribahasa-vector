namespace Codecaine.Common.Abstractions
{
    /// <summary>
    /// Represents the current user in the application.
    /// </summary>
    public interface IRequestContext
    {
        Guid UserId { get; }
        string UserName { get; }
    }
}
