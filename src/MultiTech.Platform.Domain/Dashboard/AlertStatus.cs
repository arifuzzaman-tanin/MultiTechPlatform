namespace MultiTech.Platform.Domain.Dashboard;

/// <summary>
/// Defines the lifecycle status of a dashboard alert.
/// </summary>
public enum AlertStatus
{
    /// <summary>
    /// Indicates that the alert is open.
    /// </summary>
    Open = 1,

    /// <summary>
    /// Indicates that the alert has been acknowledged.
    /// </summary>
    Acknowledged = 2,

    /// <summary>
    /// Indicates that the alert has been resolved.
    /// </summary>
    Resolved = 3
}
