using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MultiTech.Platform.Domain.Dashboard;

namespace MultiTech.Platform.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configures the dashboard alert entity.
/// </summary>
public sealed class AlertConfiguration : IEntityTypeConfiguration<Alert>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Alert> builder)
    {
        builder.ToTable("Alerts", "dashboard");

        builder.HasKey(alert => alert.Id);

        builder.Property(alert => alert.DeviceId)
            .IsRequired();

        builder.Property(alert => alert.SiteId)
            .IsRequired();

        builder.Property(alert => alert.Title)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(alert => alert.Severity)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(alert => alert.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(alert => alert.CreatedAtUtc)
            .IsRequired();

        builder.HasOne<Device>()
            .WithMany()
            .HasForeignKey(alert => alert.DeviceId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Site>()
            .WithMany()
            .HasForeignKey(alert => alert.SiteId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(alert => alert.DeviceId);
        builder.HasIndex(alert => alert.SiteId);
        builder.HasIndex(alert => alert.Severity);
        builder.HasIndex(alert => alert.Status);
        builder.HasIndex(alert => alert.CreatedAtUtc);
    }
}
