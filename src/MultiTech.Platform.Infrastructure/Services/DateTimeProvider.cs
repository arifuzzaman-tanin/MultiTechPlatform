using MultiTech.Platform.Application.Abstractions.Clock;

namespace MultiTech.Platform.Infrastructure.Services;

/// <summary>
/// Provides system UTC time.
/// </summary>
public sealed class DateTimeProvider : IDateTimeProvider
{
    /// <inheritdoc />
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}

