using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MultiTech.Platform.Domain.Dashboard;

namespace MultiTech.Platform.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configures the dashboard device entity.
/// </summary>
public sealed class DeviceConfiguration : IEntityTypeConfiguration<Device>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Device> builder)
    {
        builder.ToTable("Devices", "dashboard");

        builder.HasKey(device => device.Id);

        builder.Property(device => device.SiteId)
            .IsRequired();

        builder.Property(device => device.Name)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(device => device.SerialNumber)
            .HasMaxLength(80)
            .IsRequired();

        builder.HasIndex(device => device.SerialNumber)
            .IsUnique();

        builder.Property(device => device.DeviceType)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(device => device.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(device => device.LastSeenAtUtc)
            .IsRequired();

        builder.Property(device => device.IsManaged)
            .IsRequired();

        builder.Property(device => device.CreatedAtUtc)
            .IsRequired();

        builder.Property(device => device.UpdatedAtUtc)
            .IsRequired();

        builder.HasOne<Site>()
            .WithMany()
            .HasForeignKey(device => device.SiteId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(device => device.SiteId);
        builder.HasIndex(device => device.DeviceType);
        builder.HasIndex(device => device.Status);
        builder.HasIndex(device => device.IsManaged);
        builder.HasIndex(device => device.LastSeenAtUtc);
    }
}
