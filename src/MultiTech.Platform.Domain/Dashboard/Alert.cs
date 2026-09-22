using MultiTech.Platform.Domain.Common;

namespace MultiTech.Platform.Domain.Dashboard;

/// <summary>
/// Represents an operational alert included in dashboard summaries.
/// </summary>
public sealed class Alert : Entity
{
    private Alert()
    {
    }

    private Alert(
        Guid id,
        Guid deviceId,
        Guid siteId,
        string title,
        AlertSeverity severity,
        AlertStatus status,
        DateTimeOffset createdAtUtc,
        DateTimeOffset? resolvedAtUtc)
    {
        Id = id;
        DeviceId = deviceId;
        SiteId = siteId;
        Title = title;
        Severity = severity;
        Status = status;
        CreatedAtUtc = createdAtUtc;
        ResolvedAtUtc = resolvedAtUtc;
    }

    /// <summary>
    /// Gets the related device identifier.
    /// </summary>
    public Guid DeviceId { get; private set; }

    /// <summary>
    /// Gets the related site identifier.
    /// </summary>
    public Guid SiteId { get; private set; }

    /// <summary>
    /// Gets the alert title.
    /// </summary>
    public string Title { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the alert severity.
    /// </summary>
    public AlertSeverity Severity { get; private set; }

    /// <summary>
    /// Gets the alert lifecycle status.
    /// </summary>
    public AlertStatus Status { get; private set; }

    /// <summary>
    /// Gets the UTC timestamp when the alert was created.
    /// </summary>
    public DateTimeOffset CreatedAtUtc { get; private set; }

    /// <summary>
    /// Gets the UTC timestamp when the alert was resolved.
    /// </summary>
    public DateTimeOffset? ResolvedAtUtc { get; private set; }

    /// <summary>
    /// Creates a dashboard alert.
    /// </summary>
    /// <param name="id">The stable alert identifier.</param>
    /// <param name="deviceId">The related device identifier.</param>
    /// <param name="siteId">The related site identifier.</param>
    /// <param name="title">The alert title.</param>
    /// <param name="severity">The alert severity.</param>
    /// <param name="status">The alert lifecycle status.</param>
    /// <param name="createdAtUtc">The UTC creation timestamp.</param>
    /// <param name="resolvedAtUtc">The optional UTC resolution timestamp.</param>
    /// <returns>The created alert.</returns>
    public static Alert Create(
        Guid id,
        Guid deviceId,
        Guid siteId,
        string title,
        AlertSeverity severity,
        AlertStatus status,
        DateTimeOffset createdAtUtc,
        DateTimeOffset? resolvedAtUtc = null)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Alert identifier is required.", nameof(id));
        }

        if (deviceId == Guid.Empty)
        {
            throw new ArgumentException("Device identifier is required.", nameof(deviceId));
        }

        if (siteId == Guid.Empty)
        {
            throw new ArgumentException("Site identifier is required.", nameof(siteId));
        }

        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Alert title is required.", nameof(title));
        }

        return new Alert(
            id,
            deviceId,
            siteId,
            title.Trim(),
            severity,
            status,
            createdAtUtc,
            resolvedAtUtc);
    }
}
