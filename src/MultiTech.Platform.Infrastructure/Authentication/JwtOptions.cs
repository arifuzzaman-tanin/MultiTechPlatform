namespace MultiTech.Platform.Infrastructure.Authentication;

/// <summary>
/// Defines JWT authentication settings.
/// </summary>
public sealed class JwtOptions
{
    /// <summary>
    /// Gets the configuration section name.
    /// </summary>
    public const string SectionName = "Jwt";

    /// <summary>
    /// Gets or initializes the token issuer.
    /// </summary>
    public required string Issuer { get; init; }

    /// <summary>
    /// Gets or initializes the token audience.
    /// </summary>
    public required string Audience { get; init; }

    /// <summary>
    /// Gets or initializes the signing key.
    /// </summary>
    public required string SigningKey { get; init; }

    /// <summary>
    /// Gets or initializes the access-token lifetime in minutes.
    /// </summary>
    public int AccessTokenLifetimeMinutes { get; init; } = 15;

    /// <summary>
    /// Gets or initializes the refresh-token lifetime in days.
    /// </summary>
    public int RefreshTokenLifetimeDays { get; init; } = 7;
}

