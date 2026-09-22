using MultiTech.Platform.Application.Common.Results;

namespace MultiTech.Platform.Application.Features.Authentication;

/// <summary>
/// Contains authentication error definitions.
/// </summary>
public static class AuthErrors
{
    /// <summary>
    /// Gets the duplicate email error.
    /// </summary>
    public static readonly Error EmailAlreadyExists = Error.Conflict(
        "Auth.EmailAlreadyExists",
        "An account with this email already exists.");

    /// <summary>
    /// Gets the invalid credentials error.
    /// </summary>
    public static readonly Error InvalidCredentials = Error.Unauthorized(
        "Auth.InvalidCredentials",
        "Invalid email or password.");

    /// <summary>
    /// Gets the invalid refresh token error.
    /// </summary>
    public static readonly Error InvalidRefreshToken = Error.Unauthorized(
        "Auth.InvalidRefreshToken",
        "The refresh token is invalid or expired.");

    /// <summary>
    /// Gets the authenticated user not found error.
    /// </summary>
    public static readonly Error CurrentUserNotFound = Error.NotFound(
        "Auth.CurrentUserNotFound",
        "The authenticated user was not found.");
}

