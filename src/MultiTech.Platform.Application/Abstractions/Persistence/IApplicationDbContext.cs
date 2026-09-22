namespace MultiTech.Platform.Application.Abstractions.Persistence;

/// <summary>
/// Represents the application database context contract.
/// </summary>
public interface IApplicationDbContext
{
    /// <summary>
    /// Saves pending changes.
    /// </summary>
    /// <param name="cancellationToken">Token used to cancel the asynchronous operation.</param>
    /// <returns>The number of affected records.</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
