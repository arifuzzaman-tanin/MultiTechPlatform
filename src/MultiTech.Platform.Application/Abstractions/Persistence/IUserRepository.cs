using MultiTech.Platform.Domain.Users;

namespace MultiTech.Platform.Application.Abstractions.Persistence;

/// <summary>
/// Persists and retrieves users.
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// Gets a user by normalized email.
    /// </summary>
    /// <param name="normalizedEmail">The normalized email address.</param>
    /// <param name="cancellationToken">Token used to cancel the asynchronous operation.</param>
    /// <returns>The matching user, or <see langword="null"/>.</returns>
    Task<User?> GetByEmailAsync(
        string normalizedEmail,
        CancellationToken cancellationToken);

    /// <summary>
    /// Gets a user by identifier.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="cancellationToken">Token used to cancel the asynchronous operation.</param>
    /// <returns>The matching user, or <see langword="null"/>.</returns>
    Task<User?> GetByIdAsync(
        Guid userId,
        CancellationToken cancellationToken);

    /// <summary>
    /// Determines whether a user exists by normalized email.
    /// </summary>
    /// <param name="normalizedEmail">The normalized email address.</param>
    /// <param name="cancellationToken">Token used to cancel the asynchronous operation.</param>
    /// <returns><see langword="true"/> when a user exists.</returns>
    Task<bool> ExistsByEmailAsync(
        string normalizedEmail,
        CancellationToken cancellationToken);

    /// <summary>
    /// Adds a user.
    /// </summary>
    /// <param name="user">The user to add.</param>
    void Add(User user);
}

