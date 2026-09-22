namespace MultiTech.Platform.Application.Features.Authentication.Login;

/// <summary>
/// Represents a successful login result.
/// </summary>
/// <param name="AccessToken">The JWT access token.</param>
/// <param name="AccessTokenExpiresAtUtc">The access token expiration timestamp.</param>
/// <param name="RefreshToken">The raw refresh token for the transport layer.</param>
/// <param name="RefreshTokenExpiresAtUtc">The refresh token expiration timestamp.</param>
/// <param name="User">The authenticated user summary.</param>
public sealed record LoginResult(
    string AccessToken,
    DateTimeOffset AccessTokenExpiresAtUtc,
    string RefreshToken,
    DateTimeOffset RefreshTokenExpiresAtUtc,
    AuthenticatedUserDto User);

