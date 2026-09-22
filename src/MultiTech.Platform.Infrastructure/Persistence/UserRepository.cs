using Microsoft.EntityFrameworkCore;
using MultiTech.Platform.Application.Abstractions.Persistence;
using MultiTech.Platform.Domain.Users;

namespace MultiTech.Platform.Infrastructure.Persistence;

/// <summary>
/// EF Core user repository.
/// </summary>
public sealed class UserRepository : IUserRepository
{
    private readonly PlatformDbContext _dbContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserRepository"/> class.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    public UserRepository(PlatformDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <inheritdoc />
    public Task<User?> GetByEmailAsync(
        string normalizedEmail,
        CancellationToken cancellationToken) =>
        _dbContext.Users.FirstOrDefaultAsync(
            user => user.Email == normalizedEmail,
            cancellationToken);

    /// <inheritdoc />
    public Task<User?> GetByIdAsync(
        Guid userId,
        CancellationToken cancellationToken) =>
        _dbContext.Users.FirstOrDefaultAsync(
            user => user.Id == userId,
            cancellationToken);

    /// <inheritdoc />
    public Task<bool> ExistsByEmailAsync(
        string normalizedEmail,
        CancellationToken cancellationToken) =>
        _dbContext.Users.AnyAsync(
            user => user.Email == normalizedEmail,
            cancellationToken);

    /// <inheritdoc />
    public void Add(User user)
    {
        _dbContext.Users.Add(user);
    }
}

