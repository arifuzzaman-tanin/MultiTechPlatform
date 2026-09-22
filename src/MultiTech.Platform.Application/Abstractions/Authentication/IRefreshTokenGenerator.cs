namespace MultiTech.Platform.Application.Abstractions.Authentication;

/// <summary>
/// Generates secure refresh tokens.
/// </summary>
public interface IRefreshTokenGenerator
{
    /// <summary>
    /// Generates a refresh token.
    /// </summary>
    /// <returns>The generated token.</returns>
    string Generate();
}

