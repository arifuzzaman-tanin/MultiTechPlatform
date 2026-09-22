using MultiTech.Platform.Domain.Users;

namespace MultiTech.Platform.Application.Abstractions.Authentication;

/// <summary>
/// Creates JWT access tokens for authenticated users.
/// </summary>
public interface IAccessTokenProvider
{
    /// <summary>
    /// Creates an access token.
    /// </summary>
    /// <param name="user">The authenticated user.</param>
    /// <returns>The created access token.</returns>
    AccessTokenResult Create(User user);
}

