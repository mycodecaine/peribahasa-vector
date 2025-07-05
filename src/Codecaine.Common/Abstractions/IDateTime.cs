namespace Codecaine.Common.Abstractions
{
    /// <summary>
    /// Represents a service for getting the current date and time.
    /// </summary>
    public interface IDateTime
    {
        DateTime UtcNow { get; }
        DateTime Now { get; }
        DateTime LocalTime { get; }

        public string TimeZone { get; }
    }
}
