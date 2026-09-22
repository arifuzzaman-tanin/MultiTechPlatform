using Microsoft.EntityFrameworkCore;
using MultiTech.Platform.Application.Abstractions.Persistence;
using MultiTech.Platform.Domain.Users;

namespace MultiTech.Platform.Infrastructure.Persistence;

/// <summary>
/// Represents the EF Core database context for the platform.
/// </summary>
public sealed class PlatformDbContext(DbContextOptions<PlatformDbContext> options)
    : DbContext(options), IApplicationDbContext
{
    /// <inheritdoc />
    public DbSet<User> Users => Set<User>();

    /// <inheritdoc />
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    /// <inheritdoc />
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await base.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception) when (IsUniqueEmailViolation(exception))
        {
            throw new DuplicateEmailException();
        }
    }

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PlatformDbContext).Assembly);
    }

    private static bool IsUniqueEmailViolation(DbUpdateException exception)
    {
        string exceptionText = exception.ToString();

        return exception.Entries.Any(entry => entry.Entity is User)
            && (exceptionText.Contains("IX_Users_Email", StringComparison.Ordinal)
                || exceptionText.Contains("Users.Email", StringComparison.Ordinal)
                || exceptionText.Contains("auth.Users.Email", StringComparison.Ordinal));
    }
}
