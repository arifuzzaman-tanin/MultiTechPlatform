namespace MultiTech.Platform.Application.Features.Authentication;

/// <summary>
/// Represents a safe authenticated user summary.
/// </summary>
/// <param name="Id">The user identifier.</param>
/// <param name="Name">The display name.</param>
/// <param name="Email">The email address.</param>
/// <param name="Company">The company name.</param>
public sealed record AuthenticatedUserDto(
    Guid Id,
    string Name,
    string Email,
    string Company);

