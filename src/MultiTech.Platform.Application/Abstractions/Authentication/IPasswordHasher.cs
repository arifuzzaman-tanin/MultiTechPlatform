using MultiTech.Platform.Domain.Users;

namespace MultiTech.Platform.Application.Abstractions.Authentication;

/// <summary>
/// Hashes and verifies user passwords.
/// </summary>
public interface IPasswordHasher
{
    /// <summary>
    /// Hashes a password for the supplied user.
    /// </summary>
    /// <param name="user">The user.</param>
    /// <param name="password">The plain-text password.</param>
    /// <returns>The password hash.</returns>
    string Hash(User user, string password);

    /// <summary>
    /// Verifies a provided password against a stored hash.
    /// </summary>
    /// <param name="user">The user.</param>
    /// <param name="passwordHash">The stored password hash.</param>
    /// <param name="providedPassword">The provided password.</param>
    /// <returns><see langword="true"/> when the password is valid.</returns>
    bool Verify(User user, string passwordHash, string providedPassword);
}

