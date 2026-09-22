using MultiTech.Platform.Domain.Common;

namespace MultiTech.Platform.Domain.Users;

/// <summary>
/// Represents a registered platform user.
/// </summary>
public sealed class User : Entity
{
    private readonly List<RefreshToken> _refreshTokens = [];

    private User()
    {
    }

    private User(
        Guid id,
        string name,
        string email,
        string companyName,
        string passwordHash)
    {
        Id = id;
        Name = name;
        Email = email;
        CompanyName = companyName;
        PasswordHash = passwordHash;
        IsActive = true;
    }

    /// <summary>
    /// Gets the user's display name.
    /// </summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the normalized email address used to identify the user.
    /// </summary>
    public string Email { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the company name supplied during registration.
    /// </summary>
    public string CompanyName { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the hashed password.
    /// </summary>
    public string PasswordHash { get; private set; } = string.Empty;

    /// <summary>
    /// Gets a value indicating whether the user can authenticate.
    /// </summary>
    public bool IsActive { get; private set; }

    /// <summary>
    /// Gets the refresh tokens issued for the user.
    /// </summary>
    public IReadOnlyCollection<RefreshToken> RefreshTokens => _refreshTokens.AsReadOnly();

    /// <summary>
    /// Creates an active user.
    /// </summary>
    /// <param name="name">The trimmed display name.</param>
    /// <param name="email">The normalized email address.</param>
    /// <param name="companyName">The trimmed company name.</param>
    /// <param name="passwordHash">The password hash.</param>
    /// <returns>The created user.</returns>
    public static User Create(
        string name,
        string email,
        string companyName,
        string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Name is required.", nameof(name));
        }

        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException("Email is required.", nameof(email));
        }

        if (string.IsNullOrWhiteSpace(companyName))
        {
            throw new ArgumentException("Company name is required.", nameof(companyName));
        }

        if (string.IsNullOrWhiteSpace(passwordHash))
        {
            throw new ArgumentException("Password hash is required.", nameof(passwordHash));
        }

        return new User(
            Guid.NewGuid(),
            name,
            email,
            companyName,
            passwordHash);
    }

    /// <summary>
    /// Adds an issued refresh token to the user.
    /// </summary>
    /// <param name="refreshToken">The refresh token to add.</param>
    public void AddRefreshToken(RefreshToken refreshToken)
    {
        ArgumentNullException.ThrowIfNull(refreshToken);

        _refreshTokens.Add(refreshToken);
    }

    /// <summary>
    /// Updates the password hash.
    /// </summary>
    /// <param name="passwordHash">The new password hash.</param>
    public void SetPasswordHash(string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(passwordHash))
        {
            throw new ArgumentException("Password hash is required.", nameof(passwordHash));
        }

        PasswordHash = passwordHash;
    }
}
