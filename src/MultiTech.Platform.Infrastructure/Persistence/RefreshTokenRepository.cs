using Microsoft.EntityFrameworkCore;
using MultiTech.Platform.Application.Abstractions.Persistence;
using MultiTech.Platform.Domain.Users;

namespace MultiTech.Platform.Infrastructure.Persistence;

/// <summary>
/// EF Core refresh-token repository.
/// </summary>
public sealed class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly PlatformDbContext _dbContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="RefreshTokenRepository"/> class.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    public RefreshTokenRepository(PlatformDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <inheritdoc />
    public Task<RefreshToken?> GetByHashAsync(
        string tokenHash,
        CancellationToken cancellationToken) =>
        _dbContext.RefreshTokens.FirstOrDefaultAsync(
            refreshToken => refreshToken.TokenHash == tokenHash,
            cancellationToken);

    /// <inheritdoc />
    public void Add(RefreshToken refreshToken)
    {
        _dbContext.RefreshTokens.Add(refreshToken);
    }
}

