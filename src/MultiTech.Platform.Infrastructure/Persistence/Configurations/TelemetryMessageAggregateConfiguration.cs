using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MultiTech.Platform.Domain.Dashboard;

namespace MultiTech.Platform.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configures the telemetry message aggregate entity.
/// </summary>
public sealed class TelemetryMessageAggregateConfiguration
    : IEntityTypeConfiguration<TelemetryMessageAggregate>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<TelemetryMessageAggregate> builder)
    {
        builder.ToTable("TelemetryMessageAggregates", "dashboard");

        builder.HasKey(aggregate => aggregate.Id);

        builder.Property(aggregate => aggregate.SiteId)
            .IsRequired();

        builder.Property(aggregate => aggregate.BucketStartUtc)
            .IsRequired();

        builder.Property(aggregate => aggregate.BucketEndUtc)
            .IsRequired();

        builder.Property(aggregate => aggregate.MessageCount)
            .IsRequired();

        builder.Property(aggregate => aggregate.CreatedAtUtc)
            .IsRequired();

        builder.HasOne<Site>()
            .WithMany()
            .HasForeignKey(aggregate => aggregate.SiteId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(aggregate => aggregate.SiteId);
        builder.HasIndex(aggregate => new { aggregate.BucketStartUtc, aggregate.BucketEndUtc });
    }
}
