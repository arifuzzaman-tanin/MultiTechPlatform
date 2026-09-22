using MultiTech.Platform.Application.Features.Dashboard.GetOverview;

namespace MultiTech.Platform.Application.Abstractions.Persistence;

/// <summary>
/// Reads aggregate data required by the dashboard overview.
/// </summary>
public interface IDashboardOverviewReadRepository
{
    /// <summary>
    /// Retrieves dashboard overview aggregate metrics.
    /// </summary>
    /// <param name="nowUtc">The UTC timestamp used as the aggregation reference point.</param>
    /// <param name="cancellationToken">Token used to cancel the asynchronous operation.</param>
    /// <returns>The dashboard overview metrics.</returns>
    Task<DashboardOverviewDto> GetOverviewAsync(
        DateTimeOffset nowUtc,
        CancellationToken cancellationToken);
}
