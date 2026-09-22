using MultiTech.Platform.Domain.Users;

namespace MultiTech.Platform.Application.Abstractions.Persistence;

/// <summary>
/// Persists and retrieves refresh tokens.
/// </summary>
public interface IRefreshTokenRepository
{
    /// <summary>
    /// Gets a refresh token by hash.
    /// </summary>
    /// <param name="tokenHash">The token hash.</param>
    /// <param name="cancellationToken">Token used to cancel the asynchronous operation.</param>
    /// <returns>The matching refresh token, or <see langword="null"/>.</returns>
    Task<RefreshToken?> GetByHashAsync(
        string tokenHash,
        CancellationToken cancellationToken);

    /// <summary>
    /// Adds a refresh token.
    /// </summary>
    /// <param name="refreshToken">The refresh token to add.</param>
    void Add(RefreshToken refreshToken);
}

