namespace MultiTech.Platform.Contracts.Authentication;

/// <summary>
/// Represents a safe user summary.
/// </summary>
/// <param name="Id">The user identifier.</param>
/// <param name="Name">The user's display name.</param>
/// <param name="Email">The user's email address.</param>
/// <param name="Company">The user's company name.</param>
public sealed record UserSummaryResponse(
    Guid Id,
    string Name,
    string Email,
    string Company);

