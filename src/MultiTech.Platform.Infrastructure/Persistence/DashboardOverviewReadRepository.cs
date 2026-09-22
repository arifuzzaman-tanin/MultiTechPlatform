using Microsoft.EntityFrameworkCore;
using MultiTech.Platform.Application.Abstractions.Persistence;
using MultiTech.Platform.Application.Features.Dashboard.GetOverview;
using MultiTech.Platform.Domain.Dashboard;

namespace MultiTech.Platform.Infrastructure.Persistence;

/// <summary>
/// EF Core read repository for dashboard overview aggregate data.
/// </summary>
public sealed class DashboardOverviewReadRepository : IDashboardOverviewReadRepository
{
    private readonly PlatformDbContext _dbContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="DashboardOverviewReadRepository"/> class.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    public DashboardOverviewReadRepository(PlatformDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <inheritdoc />
    public async Task<DashboardOverviewDto> GetOverviewAsync(
        DateTimeOffset nowUtc,
        CancellationToken cancellationToken)
    {
        DateTimeOffset messagesWindowStartUtc = nowUtc.AddHours(-24);

        DashboardDeviceSummary deviceSummary = await GetDeviceSummaryAsync(cancellationToken);
        DashboardAlertSummary alertSummary = await GetAlertSummaryAsync(cancellationToken);

        long messagesLast24Hours = await _dbContext.TelemetryMessageAggregates
            .AsNoTracking()
            .Where(aggregate => aggregate.BucketStartUtc >= messagesWindowStartUtc)
            .SumAsync(aggregate => aggregate.MessageCount, cancellationToken);

        int activeSites = await _dbContext.Sites
            .AsNoTracking()
            .CountAsync(site => site.IsActive, cancellationToken);

        return new DashboardOverviewDto
        {
            ManagedDevices = deviceSummary.ManagedDevices,
            OnlineDevices = deviceSummary.OnlineDevices,
            Gateways = deviceSummary.Gateways,
            Sensors = deviceSummary.Sensors,
            CriticalAlerts = alertSummary.CriticalAlerts,
            ConnectivityHealthPercent = CalculateConnectivityHealth(
                deviceSummary.OnlineDevices,
                deviceSummary.ManagedDevices),
            MessagesLast24Hours = messagesLast24Hours,
            Sites = activeSites,
            LastUpdatedUtc = nowUtc
        };
    }

    private async Task<DashboardDeviceSummary> GetDeviceSummaryAsync(CancellationToken cancellationToken)
    {
        DashboardDeviceSummary? summary = await _dbContext.Devices
            .AsNoTracking()
            .Where(device => device.IsManaged)
            .GroupBy(_ => 1)
            .Select(group => new DashboardDeviceSummary(
                group.Count(),
                group.Count(device => device.Status == DeviceStatus.Online),
                group.Count(device => device.DeviceType == DeviceType.Gateway),
                group.Count(device => device.DeviceType == DeviceType.Sensor)))
            .SingleOrDefaultAsync(cancellationToken);

        return summary ?? DashboardDeviceSummary.Empty;
    }

    private async Task<DashboardAlertSummary> GetAlertSummaryAsync(CancellationToken cancellationToken)
    {
        DashboardAlertSummary? summary = await _dbContext.Alerts
            .AsNoTracking()
            .Where(alert => alert.Status == AlertStatus.Open)
            .GroupBy(_ => 1)
            .Select(group => new DashboardAlertSummary(
                group.Count(alert => alert.Severity == AlertSeverity.Critical)))
            .SingleOrDefaultAsync(cancellationToken);

        return summary ?? DashboardAlertSummary.Empty;
    }

    private static decimal CalculateConnectivityHealth(
        int onlineDevices,
        int managedDevices)
    {
        if (managedDevices == 0)
        {
            return 0m;
        }

        return Math.Round(onlineDevices * 100m / managedDevices, 2, MidpointRounding.AwayFromZero);
    }

    private sealed record DashboardDeviceSummary(
        int ManagedDevices,
        int OnlineDevices,
        int Gateways,
        int Sensors)
    {
        public static DashboardDeviceSummary Empty { get; } = new(0, 0, 0, 0);
    }

    private sealed record DashboardAlertSummary(int CriticalAlerts)
    {
        public static DashboardAlertSummary Empty { get; } = new(0);
    }
}
