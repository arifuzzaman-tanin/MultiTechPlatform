using MultiTech.Platform.Application.Abstractions.Clock;
using MultiTech.Platform.Application.Abstractions.Messaging;
using MultiTech.Platform.Application.Abstractions.Persistence;
using MultiTech.Platform.Application.Common.Results;

namespace MultiTech.Platform.Application.Features.Dashboard.GetOverview;

/// <summary>
/// Handles dashboard overview summary queries.
/// </summary>
public sealed class GetDashboardOverviewQueryHandler
    : IQueryHandler<GetDashboardOverviewQuery, DashboardOverviewDto>
{
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IDashboardOverviewReadRepository _dashboardOverviewReadRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetDashboardOverviewQueryHandler"/> class.
    /// </summary>
    /// <param name="dateTimeProvider">The UTC time provider.</param>
    /// <param name="dashboardOverviewReadRepository">The dashboard overview read repository.</param>
    public GetDashboardOverviewQueryHandler(
        IDateTimeProvider dateTimeProvider,
        IDashboardOverviewReadRepository dashboardOverviewReadRepository)
    {
        _dateTimeProvider = dateTimeProvider;
        _dashboardOverviewReadRepository = dashboardOverviewReadRepository;
    }

    /// <inheritdoc />
    public async Task<Result<DashboardOverviewDto>> Handle(
        GetDashboardOverviewQuery query,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(query);

        DashboardOverviewDto overview = await _dashboardOverviewReadRepository.GetOverviewAsync(
            _dateTimeProvider.UtcNow,
            cancellationToken);

        return Result<DashboardOverviewDto>.Success(overview);
    }
}
