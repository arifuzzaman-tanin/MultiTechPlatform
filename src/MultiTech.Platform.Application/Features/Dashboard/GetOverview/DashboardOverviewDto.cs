namespace MultiTech.Platform.Application.Features.Dashboard.GetOverview;

/// <summary>
/// Represents the summary metrics required by the dashboard overview cards.
/// </summary>
public sealed record DashboardOverviewDto
{
    /// <summary>
    /// Gets the number of managed devices.
    /// </summary>
    public int ManagedDevices { get; init; }

    /// <summary>
    /// Gets the number of online managed devices.
    /// </summary>
    public int OnlineDevices { get; init; }

    /// <summary>
    /// Gets the number of managed gateways.
    /// </summary>
    public int Gateways { get; init; }

    /// <summary>
    /// Gets the number of managed sensors.
    /// </summary>
    public int Sensors { get; init; }

    /// <summary>
    /// Gets the number of open critical alerts.
    /// </summary>
    public int CriticalAlerts { get; init; }

    /// <summary>
    /// Gets the managed-device connectivity health percentage.
    /// </summary>
    public decimal ConnectivityHealthPercent { get; init; }

    /// <summary>
    /// Gets the number of telemetry messages received in the last 24 hours.
    /// </summary>
    public long MessagesLast24Hours { get; init; }

    /// <summary>
    /// Gets the number of active sites.
    /// </summary>
    public int Sites { get; init; }

    /// <summary>
    /// Gets the UTC timestamp used to calculate the overview.
    /// </summary>
    public DateTimeOffset LastUpdatedUtc { get; init; }
}
