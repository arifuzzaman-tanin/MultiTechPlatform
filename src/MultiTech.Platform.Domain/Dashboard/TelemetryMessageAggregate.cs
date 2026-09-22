using MultiTech.Platform.Domain.Common;

namespace MultiTech.Platform.Domain.Dashboard;

/// <summary>
/// Represents aggregated telemetry message volume for a time bucket.
/// </summary>
public sealed class TelemetryMessageAggregate : Entity
{
    private TelemetryMessageAggregate()
    {
    }

    private TelemetryMessageAggregate(
        Guid id,
        Guid siteId,
        DateTimeOffset bucketStartUtc,
        DateTimeOffset bucketEndUtc,
        long messageCount,
        DateTimeOffset createdAtUtc)
    {
        Id = id;
        SiteId = siteId;
        BucketStartUtc = bucketStartUtc;
        BucketEndUtc = bucketEndUtc;
        MessageCount = messageCount;
        CreatedAtUtc = createdAtUtc;
    }

    /// <summary>
    /// Gets the related site identifier.
    /// </summary>
    public Guid SiteId { get; private set; }

    /// <summary>
    /// Gets the UTC start of the aggregate bucket.
    /// </summary>
    public DateTimeOffset BucketStartUtc { get; private set; }

    /// <summary>
    /// Gets the UTC end of the aggregate bucket.
    /// </summary>
    public DateTimeOffset BucketEndUtc { get; private set; }

    /// <summary>
    /// Gets the number of messages observed in the bucket.
    /// </summary>
    public long MessageCount { get; private set; }

    /// <summary>
    /// Gets the UTC timestamp when the aggregate was created.
    /// </summary>
    public DateTimeOffset CreatedAtUtc { get; private set; }

    /// <summary>
    /// Creates a telemetry message aggregate.
    /// </summary>
    /// <param name="id">The stable aggregate identifier.</param>
    /// <param name="siteId">The related site identifier.</param>
    /// <param name="bucketStartUtc">The UTC bucket start.</param>
    /// <param name="bucketEndUtc">The UTC bucket end.</param>
    /// <param name="messageCount">The observed message count.</param>
    /// <param name="createdAtUtc">The UTC creation timestamp.</param>
    /// <returns>The created aggregate.</returns>
    public static TelemetryMessageAggregate Create(
        Guid id,
        Guid siteId,
        DateTimeOffset bucketStartUtc,
        DateTimeOffset bucketEndUtc,
        long messageCount,
        DateTimeOffset createdAtUtc)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Telemetry aggregate identifier is required.", nameof(id));
        }

        if (siteId == Guid.Empty)
        {
            throw new ArgumentException("Site identifier is required.", nameof(siteId));
        }

        if (bucketEndUtc <= bucketStartUtc)
        {
            throw new ArgumentException("Bucket end must be after bucket start.", nameof(bucketEndUtc));
        }

        if (messageCount < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(messageCount), "Message count cannot be negative.");
        }

        return new TelemetryMessageAggregate(
            id,
            siteId,
            bucketStartUtc,
            bucketEndUtc,
            messageCount,
            createdAtUtc);
    }
}
