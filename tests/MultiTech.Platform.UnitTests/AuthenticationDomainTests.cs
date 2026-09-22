using MultiTech.Platform.Domain.Users;

namespace MultiTech.Platform.UnitTests;

public sealed class AuthenticationDomainTests
{
    [Fact]
    public void UserCreate_ValidValues_CreatesActiveUser()
    {
        User user = User.Create(
            "John Smith",
            "john.smith@example.com",
            "Acme Manufacturing",
            "hash");

        Assert.NotEqual(Guid.Empty, user.Id);
        Assert.True(user.IsActive);
        Assert.Equal("john.smith@example.com", user.Email);
        Assert.Equal("Acme Manufacturing", user.CompanyName);
        Assert.Equal("hash", user.PasswordHash);
    }

    [Fact]
    public void RefreshTokenCreate_ValidValues_StoresHashOnly()
    {
        DateTimeOffset now = DateTimeOffset.UtcNow;

        RefreshToken refreshToken = RefreshToken.Create(
            Guid.NewGuid(),
            "hashed-token",
            now,
            now.AddDays(7));

        Assert.Equal("hashed-token", refreshToken.TokenHash);
        Assert.False(refreshToken.IsRevoked);
        Assert.False(refreshToken.IsExpired(now));
    }

    [Fact]
    public void RefreshTokenRevoke_ActiveToken_MarksRevoked()
    {
        DateTimeOffset now = DateTimeOffset.UtcNow;
        RefreshToken refreshToken = RefreshToken.Create(
            Guid.NewGuid(),
            "hashed-token",
            now,
            now.AddDays(7));

        refreshToken.Revoke(now.AddMinutes(1));

        Assert.True(refreshToken.IsRevoked);
        Assert.Equal(now.AddMinutes(1), refreshToken.RevokedAtUtc);
    }
}

