namespace MultiTech.Platform.Application.Abstractions.Clock;

/// <summary>
/// Provides the current UTC time.
/// </summary>
public interface IDateTimeProvider
{
    /// <summary>
    /// Gets the current UTC timestamp.
    /// </summary>
    DateTimeOffset UtcNow { get; }
}

