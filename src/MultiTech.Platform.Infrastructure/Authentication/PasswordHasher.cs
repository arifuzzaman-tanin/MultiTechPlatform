using Microsoft.AspNetCore.Identity;
using MultiTech.Platform.Application.Abstractions.Authentication;
using MultiTech.Platform.Domain.Users;

namespace MultiTech.Platform.Infrastructure.Authentication;

/// <summary>
/// Hashes passwords with ASP.NET Core Identity's password hasher.
/// </summary>
public sealed class PasswordHasher : IPasswordHasher
{
    private readonly PasswordHasher<User> _passwordHasher = new();

    /// <inheritdoc />
    public string Hash(User user, string password) =>
        _passwordHasher.HashPassword(user, password);

    /// <inheritdoc />
    public bool Verify(User user, string passwordHash, string providedPassword) =>
        _passwordHasher.VerifyHashedPassword(user, passwordHash, providedPassword)
        is PasswordVerificationResult.Success or PasswordVerificationResult.SuccessRehashNeeded;
}

