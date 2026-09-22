using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MultiTech.Platform.Domain.Dashboard;

namespace MultiTech.Platform.Infrastructure.Persistence.Seed;

/// <summary>
/// Seeds deterministic dashboard overview data for local development.
/// </summary>
public sealed class DashboardOverviewSeeder
{
    private static readonly DateTimeOffset SeedCreatedAtUtc = new(2026, 9, 22, 6, 0, 0, TimeSpan.Zero);
    private readonly PlatformDbContext _dbContext;
    private readonly ILogger<DashboardOverviewSeeder> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="DashboardOverviewSeeder"/> class.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    /// <param name="logger">The structured logger.</param>
    public DashboardOverviewSeeder(
        PlatformDbContext dbContext,
        ILogger<DashboardOverviewSeeder> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    /// <summary>
    /// Seeds dashboard overview data when it is not already present.
    /// </summary>
    /// <param name="cancellationToken">Token used to cancel the asynchronous operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task SeedAsync(CancellationToken cancellationToken)
    {
        bool dashboardDataExists = await _dbContext.Sites
            .AnyAsync(site => site.Code == DashboardSeedCodes.RiversidePlant, cancellationToken);

        if (dashboardDataExists)
        {
            return;
        }

        IReadOnlyList<Site> sites = CreateSites();
        IReadOnlyList<Device> devices = CreateDevices(sites);
        IReadOnlyList<Alert> alerts = CreateAlerts(devices);
        IReadOnlyList<TelemetryMessageAggregate> telemetryAggregates = CreateTelemetryAggregates(sites);

        await _dbContext.Sites.AddRangeAsync(sites, cancellationToken);
        await _dbContext.Devices.AddRangeAsync(devices, cancellationToken);
        await _dbContext.Alerts.AddRangeAsync(alerts, cancellationToken);
        await _dbContext.TelemetryMessageAggregates.AddRangeAsync(telemetryAggregates, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Seeded dashboard overview data with {SiteCount} sites, {DeviceCount} devices, {AlertCount} alerts, and {TelemetryAggregateCount} telemetry aggregates.",
            sites.Count,
            devices.Count,
            alerts.Count,
            telemetryAggregates.Count);
    }

    private static IReadOnlyList<Site> CreateSites() =>
    [
        CreateSite(1, "Riverside Plant", DashboardSeedCodes.RiversidePlant, "Midwest"),
        CreateSite(2, "Detroit Plant", "DET", "Midwest"),
        CreateSite(3, "Toronto Distribution Center", "TOR", "Canada East"),
        CreateSite(4, "Austin Assembly", "AUS", "South"),
        CreateSite(5, "Chicago Cold Storage", "CHI", "Midwest"),
        CreateSite(6, "Vancouver Utilities", "VAN", "Canada West"),
        CreateSite(7, "Northwind Manufacturing", "NOR", "Northeast"),
        CreateSite(8, "Summit Energy Site", "SUM", "Mountain"),
        CreateSite(9, "Phoenix Logistics", "PHX", "Southwest"),
        CreateSite(10, "Portland Fabrication", "PDX", "Pacific")
    ];

    private static IReadOnlyList<Device> CreateDevices(IReadOnlyList<Site> sites)
    {
        List<Device> devices = new(capacity: 248);

        for (int index = 0; index < 24; index++)
        {
            Site site = sites[index % sites.Count];
            devices.Add(CreateDevice(
                index + 1,
                site.Id,
                $"GW-{site.Code}-{index + 1:000}",
                DeviceType.Gateway,
                GetStatus(index),
                SeedCreatedAtUtc.AddMinutes(-index)));
        }

        for (int index = 0; index < 224; index++)
        {
            Site site = sites[index % sites.Count];
            devices.Add(CreateDevice(
                index + 25,
                site.Id,
                $"SNS-{GetSensorPrefix(index)}-{site.Code}-{index + 1:000}",
                DeviceType.Sensor,
                GetStatus(index + 24),
                SeedCreatedAtUtc.AddMinutes(-(index % 120))));
        }

        return devices;
    }

    private static IReadOnlyList<Alert> CreateAlerts(IReadOnlyList<Device> devices)
    {
        List<Alert> alerts = new(capacity: 18);

        for (int index = 0; index < 9; index++)
        {
            Device device = devices[index * 11];
            alerts.Add(CreateAlert(
                index + 1,
                device,
                $"Critical connectivity loss on {device.Name}",
                AlertSeverity.Critical,
                AlertStatus.Open,
                SeedCreatedAtUtc.AddHours(-index - 1)));
        }

        for (int index = 0; index < 5; index++)
        {
            Device device = devices[index * 7 + 3];
            alerts.Add(CreateAlert(
                index + 10,
                device,
                $"Warning threshold exceeded on {device.Name}",
                AlertSeverity.Warning,
                AlertStatus.Open,
                SeedCreatedAtUtc.AddHours(-index - 2)));
        }

        for (int index = 0; index < 4; index++)
        {
            Device device = devices[index * 5 + 2];
            alerts.Add(CreateAlert(
                index + 15,
                device,
                $"Resolved telemetry backlog on {device.Name}",
                AlertSeverity.Critical,
                AlertStatus.Resolved,
                SeedCreatedAtUtc.AddDays(-index - 2),
                SeedCreatedAtUtc.AddDays(-index - 1)));
        }

        return alerts;
    }

    private static IReadOnlyList<TelemetryMessageAggregate> CreateTelemetryAggregates(IReadOnlyList<Site> sites)
    {
        List<TelemetryMessageAggregate> aggregates = new(capacity: 250);
        long[] hourlyMessageCounts =
        [
            142_000, 138_500, 133_900, 129_800, 126_700, 124_500,
            132_400, 145_600, 158_200, 171_500, 183_100, 190_800,
            196_700, 201_400, 198_900, 192_300, 185_600, 179_200,
            172_800, 166_400, 159_900, 153_700, 149_300, 148_100
        ];

        for (int hourIndex = 0; hourIndex < hourlyMessageCounts.Length; hourIndex++)
        {
            DateTimeOffset bucketStartUtc = SeedCreatedAtUtc.AddHours(hourIndex - 24);
            DateTimeOffset bucketEndUtc = bucketStartUtc.AddHours(1);
            long baseCount = hourlyMessageCounts[hourIndex] / sites.Count;

            for (int siteIndex = 0; siteIndex < sites.Count; siteIndex++)
            {
                aggregates.Add(TelemetryMessageAggregate.Create(
                    CreateGuid(7000 + (hourIndex * sites.Count) + siteIndex),
                    sites[siteIndex].Id,
                    bucketStartUtc,
                    bucketEndUtc,
                    baseCount + (siteIndex * 137),
                    SeedCreatedAtUtc));
            }
        }

        foreach (Site site in sites)
        {
            aggregates.Add(TelemetryMessageAggregate.Create(
                CreateGuid(9000 + aggregates.Count),
                site.Id,
                SeedCreatedAtUtc.AddDays(-3),
                SeedCreatedAtUtc.AddDays(-3).AddHours(1),
                42_000,
                SeedCreatedAtUtc));
        }

        return aggregates;
    }

    private static Site CreateSite(
        int id,
        string name,
        string code,
        string region) =>
        Site.Create(
            CreateGuid(id),
            name,
            code,
            region,
            isActive: true,
            SeedCreatedAtUtc.AddMonths(-12),
            SeedCreatedAtUtc);

    private static Device CreateDevice(
        int id,
        Guid siteId,
        string name,
        DeviceType deviceType,
        DeviceStatus status,
        DateTimeOffset lastSeenAtUtc) =>
        Device.Create(
            CreateGuid(1000 + id),
            siteId,
            name,
            $"MT-{id:000000}",
            deviceType,
            status,
            lastSeenAtUtc,
            isManaged: true,
            SeedCreatedAtUtc.AddMonths(-6),
            SeedCreatedAtUtc);

    private static Alert CreateAlert(
        int id,
        Device device,
        string title,
        AlertSeverity severity,
        AlertStatus status,
        DateTimeOffset createdAtUtc,
        DateTimeOffset? resolvedAtUtc = null) =>
        Alert.Create(
            CreateGuid(5000 + id),
            device.Id,
            device.SiteId,
            title,
            severity,
            status,
            createdAtUtc,
            resolvedAtUtc);

    private static DeviceStatus GetStatus(int index)
    {
        if (index < 232)
        {
            return DeviceStatus.Online;
        }

        return index < 240
            ? DeviceStatus.Degraded
            : DeviceStatus.Offline;
    }

    private static string GetSensorPrefix(int index) =>
        (index % 3) switch
        {
            0 => "TEMP",
            1 => "VIB",
            _ => "PWR"
        };

    private static Guid CreateGuid(int id) =>
        Guid.Parse($"00000000-0000-0000-0000-{id:000000000000}");

    private static class DashboardSeedCodes
    {
        public const string RiversidePlant = "RIV";
    }
}
