namespace MultiTech.Platform.Application.Abstractions.Authentication;

/// <summary>
/// Represents a created access token.
/// </summary>
/// <param name="Token">The token value.</param>
/// <param name="ExpiresAtUtc">The expiration timestamp.</param>
public sealed record AccessTokenResult(
    string Token,
    DateTimeOffset ExpiresAtUtc);

