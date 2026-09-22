namespace MultiTech.Platform.Contracts.Authentication;

/// <summary>
/// Represents a registration request.
/// </summary>
/// <param name="Name">The user's display name.</param>
/// <param name="Email">The user's email address.</param>
/// <param name="Company">The user's company name.</param>
/// <param name="Password">The user's password.</param>
public sealed record RegisterRequest(
    string Name,
    string Email,
    string Company,
    string Password);

