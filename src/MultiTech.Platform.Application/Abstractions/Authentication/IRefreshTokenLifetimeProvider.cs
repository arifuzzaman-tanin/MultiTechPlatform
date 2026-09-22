namespace MultiTech.Platform.Application.Abstractions.Authentication;

/// <summary>
/// Provides refresh-token lifetime settings to application use cases.
/// </summary>
public interface IRefreshTokenLifetimeProvider
{
    /// <summary>
    /// Gets the refresh-token lifetime.
    /// </summary>
    TimeSpan RefreshTokenLifetime { get; }
}

