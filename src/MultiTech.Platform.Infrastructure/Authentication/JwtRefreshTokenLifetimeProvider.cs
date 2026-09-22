using Microsoft.Extensions.Options;
using MultiTech.Platform.Application.Abstractions.Authentication;

namespace MultiTech.Platform.Infrastructure.Authentication;

/// <summary>
/// Provides refresh-token lifetime from JWT options.
/// </summary>
public sealed class JwtRefreshTokenLifetimeProvider : IRefreshTokenLifetimeProvider
{
    private readonly JwtOptions _jwtOptions;

    /// <summary>
    /// Initializes a new instance of the <see cref="JwtRefreshTokenLifetimeProvider"/> class.
    /// </summary>
    /// <param name="jwtOptions">The JWT options.</param>
    public JwtRefreshTokenLifetimeProvider(IOptions<JwtOptions> jwtOptions)
    {
        _jwtOptions = jwtOptions.Value;
    }

    /// <inheritdoc />
    public TimeSpan RefreshTokenLifetime => TimeSpan.FromDays(_jwtOptions.RefreshTokenLifetimeDays);
}

