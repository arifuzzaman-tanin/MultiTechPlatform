using MultiTech.Platform.Domain.Common;

namespace MultiTech.Platform.Domain.Dashboard;

/// <summary>
/// Represents an operational site shown in dashboard summaries.
/// </summary>
public sealed class Site : Entity
{
    private Site()
    {
    }

    private Site(
        Guid id,
        string name,
        string code,
        string region,
        bool isActive,
        DateTimeOffset createdAtUtc,
        DateTimeOffset updatedAtUtc)
    {
        Id = id;
        Name = name;
        Code = code;
        Region = region;
        IsActive = isActive;
        CreatedAtUtc = createdAtUtc;
        UpdatedAtUtc = updatedAtUtc;
    }

    /// <summary>
    /// Gets the site name.
    /// </summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the stable site code.
    /// </summary>
    public string Code { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the region where the site operates.
    /// </summary>
    public string Region { get; private set; } = string.Empty;

    /// <summary>
    /// Gets a value indicating whether the site is active.
    /// </summary>
    public bool IsActive { get; private set; }

    /// <summary>
    /// Gets the UTC timestamp when the site was created.
    /// </summary>
    public DateTimeOffset CreatedAtUtc { get; private set; }

    /// <summary>
    /// Gets the UTC timestamp when the site was last updated.
    /// </summary>
    public DateTimeOffset UpdatedAtUtc { get; private set; }

    /// <summary>
    /// Creates a dashboard site.
    /// </summary>
    /// <param name="id">The stable site identifier.</param>
    /// <param name="name">The site name.</param>
    /// <param name="code">The stable site code.</param>
    /// <param name="region">The site region.</param>
    /// <param name="isActive">Whether the site is active.</param>
    /// <param name="createdAtUtc">The UTC creation timestamp.</param>
    /// <param name="updatedAtUtc">The UTC update timestamp.</param>
    /// <returns>The created site.</returns>
    public static Site Create(
        Guid id,
        string name,
        string code,
        string region,
        bool isActive,
        DateTimeOffset createdAtUtc,
        DateTimeOffset updatedAtUtc)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Site identifier is required.", nameof(id));
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Site name is required.", nameof(name));
        }

        if (string.IsNullOrWhiteSpace(code))
        {
            throw new ArgumentException("Site code is required.", nameof(code));
        }

        if (string.IsNullOrWhiteSpace(region))
        {
            throw new ArgumentException("Site region is required.", nameof(region));
        }

        return new Site(
            id,
            name.Trim(),
            code.Trim().ToUpperInvariant(),
            region.Trim(),
            isActive,
            createdAtUtc,
            updatedAtUtc);
    }
}
