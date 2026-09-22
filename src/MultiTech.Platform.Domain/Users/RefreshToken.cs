using MultiTech.Platform.Domain.Common;

namespace MultiTech.Platform.Domain.Users;

/// <summary>
/// Represents a hashed refresh token issued to a user.
/// </summary>
public sealed class RefreshToken : Entity
{
    private RefreshToken()
    {
    }

    private RefreshToken(
        Guid id,
        Guid userId,
        string tokenHash,
        DateTimeOffset createdAtUtc,
        DateTimeOffset expiresAtUtc)
    {
        Id = id;
        UserId = userId;
        TokenHash = tokenHash;
        CreatedAtUtc = createdAtUtc;
        ExpiresAtUtc = expiresAtUtc;
    }

    /// <summary>
    /// Gets the user identifier that owns the token.
    /// </summary>
    public Guid UserId { get; private set; }

    /// <summary>
    /// Gets the stored token hash.
    /// </summary>
    public string TokenHash { get; private set; } = string.Empty;

    /// <summary>
    /// Gets when the token was created.
    /// </summary>
    public DateTimeOffset CreatedAtUtc { get; private set; }

    /// <summary>
    /// Gets when the token expires.
    /// </summary>
    public DateTimeOffset ExpiresAtUtc { get; private set; }

    /// <summary>
    /// Gets when the token was revoked.
    /// </summary>
    public DateTimeOffset? RevokedAtUtc { get; private set; }

    /// <summary>
    /// Gets the token that replaced this token during rotation.
    /// </summary>
    public Guid? ReplacedByTokenId { get; private set; }

    /// <summary>
    /// Gets a value indicating whether the token has been revoked.
    /// </summary>
    public bool IsRevoked => RevokedAtUtc.HasValue;

    /// <summary>
    /// Creates a refresh token.
    /// </summary>
    /// <param name="userId">The owning user identifier.</param>
    /// <param name="tokenHash">The stored token hash.</param>
    /// <param name="createdAtUtc">The creation timestamp.</param>
    /// <param name="expiresAtUtc">The expiration timestamp.</param>
    /// <returns>The created refresh token.</returns>
    public static RefreshToken Create(
        Guid userId,
        string tokenHash,
        DateTimeOffset createdAtUtc,
        DateTimeOffset expiresAtUtc)
    {
        if (userId == Guid.Empty)
        {
            throw new ArgumentException("User identifier is required.", nameof(userId));
        }

        if (string.IsNullOrWhiteSpace(tokenHash))
        {
            throw new ArgumentException("Token hash is required.", nameof(tokenHash));
        }

        if (expiresAtUtc <= createdAtUtc)
        {
            throw new ArgumentException("Expiration must be after creation.", nameof(expiresAtUtc));
        }

        return new RefreshToken(
            Guid.NewGuid(),
            userId,
            tokenHash,
            createdAtUtc,
            expiresAtUtc);
    }

    /// <summary>
    /// Determines whether the token is expired at the supplied timestamp.
    /// </summary>
    /// <param name="utcNow">The current UTC timestamp.</param>
    /// <returns><see langword="true"/> when expired; otherwise <see langword="false"/>.</returns>
    public bool IsExpired(DateTimeOffset utcNow) => utcNow >= ExpiresAtUtc;

    /// <summary>
    /// Revokes the token.
    /// </summary>
    /// <param name="revokedAtUtc">The revocation timestamp.</param>
    /// <param name="replacedByTokenId">The replacement token identifier, when rotation occurred.</param>
    public void Revoke(DateTimeOffset revokedAtUtc, Guid? replacedByTokenId = null)
    {
        if (IsRevoked)
        {
            return;
        }

        RevokedAtUtc = revokedAtUtc;
        ReplacedByTokenId = replacedByTokenId;
    }
}

