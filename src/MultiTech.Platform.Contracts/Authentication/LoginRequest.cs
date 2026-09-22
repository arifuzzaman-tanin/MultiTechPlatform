namespace MultiTech.Platform.Contracts.Authentication;

/// <summary>
/// Represents a login request.
/// </summary>
/// <param name="Email">The user's email address.</param>
/// <param name="Password">The user's password.</param>
public sealed record LoginRequest(
    string Email,
    string Password);

