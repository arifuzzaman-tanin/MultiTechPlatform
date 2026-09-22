using MultiTech.Platform.Domain.Common;

namespace MultiTech.Platform.Domain.Dashboard;

/// <summary>
/// Represents a managed device shown in dashboard summaries.
/// </summary>
public sealed class Device : Entity
{
    private Device()
    {
    }

    private Device(
        Guid id,
        Guid siteId,
        string name,
        string serialNumber,
        DeviceType deviceType,
        DeviceStatus status,
        DateTimeOffset lastSeenAtUtc,
        bool isManaged,
        DateTimeOffset createdAtUtc,
        DateTimeOffset updatedAtUtc)
    {
        Id = id;
        SiteId = siteId;
        Name = name;
        SerialNumber = serialNumber;
        DeviceType = deviceType;
        Status = status;
        LastSeenAtUtc = lastSeenAtUtc;
        IsManaged = isManaged;
        CreatedAtUtc = createdAtUtc;
        UpdatedAtUtc = updatedAtUtc;
    }

    /// <summary>
    /// Gets the site that owns the device.
    /// </summary>
    public Guid SiteId { get; private set; }

    /// <summary>
    /// Gets the device display name.
    /// </summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the stable device serial number.
    /// </summary>
    public string SerialNumber { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the device category.
    /// </summary>
    public DeviceType DeviceType { get; private set; }

    /// <summary>
    /// Gets the current device status.
    /// </summary>
    public DeviceStatus Status { get; private set; }

    /// <summary>
    /// Gets the UTC timestamp when the device was last observed.
    /// </summary>
    public DateTimeOffset LastSeenAtUtc { get; private set; }

    /// <summary>
    /// Gets a value indicating whether the device is managed by the platform.
    /// </summary>
    public bool IsManaged { get; private set; }

    /// <summary>
    /// Gets the UTC timestamp when the device was created.
    /// </summary>
    public DateTimeOffset CreatedAtUtc { get; private set; }

    /// <summary>
    /// Gets the UTC timestamp when the device was last updated.
    /// </summary>
    public DateTimeOffset UpdatedAtUtc { get; private set; }

    /// <summary>
    /// Creates a dashboard device.
    /// </summary>
    /// <param name="id">The stable device identifier.</param>
    /// <param name="siteId">The owning site identifier.</param>
    /// <param name="name">The device display name.</param>
    /// <param name="serialNumber">The stable serial number.</param>
    /// <param name="deviceType">The device category.</param>
    /// <param name="status">The current device status.</param>
    /// <param name="lastSeenAtUtc">The UTC last-seen timestamp.</param>
    /// <param name="isManaged">Whether the platform manages the device.</param>
    /// <param name="createdAtUtc">The UTC creation timestamp.</param>
    /// <param name="updatedAtUtc">The UTC update timestamp.</param>
    /// <returns>The created device.</returns>
    public static Device Create(
        Guid id,
        Guid siteId,
        string name,
        string serialNumber,
        DeviceType deviceType,
        DeviceStatus status,
        DateTimeOffset lastSeenAtUtc,
        bool isManaged,
        DateTimeOffset createdAtUtc,
        DateTimeOffset updatedAtUtc)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Device identifier is required.", nameof(id));
        }

        if (siteId == Guid.Empty)
        {
            throw new ArgumentException("Site identifier is required.", nameof(siteId));
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Device name is required.", nameof(name));
        }

        if (string.IsNullOrWhiteSpace(serialNumber))
        {
            throw new ArgumentException("Serial number is required.", nameof(serialNumber));
        }

        return new Device(
            id,
            siteId,
            name.Trim(),
            serialNumber.Trim().ToUpperInvariant(),
            deviceType,
            status,
            lastSeenAtUtc,
            isManaged,
            createdAtUtc,
            updatedAtUtc);
    }
}
