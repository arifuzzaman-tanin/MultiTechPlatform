using Microsoft.EntityFrameworkCore;
using MultiTech.Platform.Infrastructure.Persistence;

namespace MultiTech.Platform.Api.DependencyInjection;

/// <summary>
/// Provides host extensions for applying database migrations during application startup.
/// </summary>
public static class DatabaseMigrationExtensions
{
    private const string ApplyMigrationsOnStartupConfigurationKey = "Database:ApplyMigrationsOnStartup";
    private const int MaxMigrationAttempts = 12;
    private static readonly TimeSpan MigrationRetryDelay = TimeSpan.FromSeconds(5);

    /// <summary>
    /// Applies pending Entity Framework Core migrations when startup migrations are enabled.
    /// </summary>
    /// <param name="host">The application host.</param>
    /// <param name="cancellationToken">Token used to cancel the asynchronous operation.</param>
    /// <returns>The application host.</returns>
    public static async Task<IHost> ApplyDatabaseMigrationsAsync(
        this IHost host,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(host);

        using IServiceScope scope = host.Services.CreateScope();
        IConfiguration configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();

        if (!configuration.GetValue<bool>(ApplyMigrationsOnStartupConfigurationKey))
        {
            return host;
        }

        ILogger<PlatformDbContext> logger = scope.ServiceProvider.GetRequiredService<ILogger<PlatformDbContext>>();
        PlatformDbContext dbContext = scope.ServiceProvider.GetRequiredService<PlatformDbContext>();

        for (int attempt = 1; attempt <= MaxMigrationAttempts; attempt++)
        {
            try
            {
                await dbContext.Database.MigrateAsync(cancellationToken);
                logger.LogInformation("Applied database migrations successfully.");

                return host;
            }
            catch (Exception exception) when (attempt < MaxMigrationAttempts && !cancellationToken.IsCancellationRequested)
            {
                logger.LogWarning(
                    exception,
                    "Database migration attempt {Attempt} of {MaxAttempts} failed. Retrying in {DelaySeconds} seconds.",
                    attempt,
                    MaxMigrationAttempts,
                    MigrationRetryDelay.TotalSeconds);

                await Task.Delay(MigrationRetryDelay, cancellationToken);
            }
        }

        return host;
    }
}
