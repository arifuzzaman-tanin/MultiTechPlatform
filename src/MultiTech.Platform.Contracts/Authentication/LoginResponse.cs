namespace MultiTech.Platform.Contracts.Authentication;

/// <summary>
/// Represents a successful login or refresh response.
/// </summary>
/// <param name="AccessToken">The JWT access token.</param>
/// <param name="AccessTokenExpiresAtUtc">The access token expiration timestamp.</param>
/// <param name="User">The authenticated user summary.</param>
public sealed record LoginResponse(
    string AccessToken,
    DateTimeOffset AccessTokenExpiresAtUtc,
    UserSummaryResponse User);

