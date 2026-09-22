using MultiTech.Platform.Application.Abstractions.Messaging;

namespace MultiTech.Platform.Application.Features.Dashboard.GetOverview;

/// <summary>
/// Queries the dashboard overview summary metrics.
/// </summary>
public sealed record GetDashboardOverviewQuery : IQuery<DashboardOverviewDto>;
