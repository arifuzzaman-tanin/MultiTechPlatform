namespace MultiTech.Platform.Application.Abstractions.Authentication;

/// <summary>
/// Provides details about the authenticated request user.
/// </summary>
public interface ICurrentUser
{
    /// <summary>
    /// Gets a value indicating whether the request is authenticated.
    /// </summary>
    bool IsAuthenticated { get; }

    /// <summary>
    /// Gets the authenticated user's identifier.
    /// </summary>
    Guid? UserId { get; }

    /// <summary>
    /// Gets the authenticated user's email address.
    /// </summary>
    string? Email { get; }
}

