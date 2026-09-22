namespace MultiTech.Platform.Domain.Dashboard;

/// <summary>
/// Defines the operational status of a managed device.
/// </summary>
public enum DeviceStatus
{
    /// <summary>
    /// Indicates that the device is currently online.
    /// </summary>
    Online = 1,

    /// <summary>
    /// Indicates that the device is currently offline.
    /// </summary>
    Offline = 2,

    /// <summary>
    /// Indicates that the device is reachable but reporting degraded health.
    /// </summary>
    Degraded = 3,

    /// <summary>
    /// Indicates that the device is in planned maintenance.
    /// </summary>
    Maintenance = 4
}
