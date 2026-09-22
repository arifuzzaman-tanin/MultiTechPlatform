using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MultiTech.Platform.Domain.Dashboard;

namespace MultiTech.Platform.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configures the dashboard site entity.
/// </summary>
public sealed class SiteConfiguration : IEntityTypeConfiguration<Site>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Site> builder)
    {
        builder.ToTable("Sites", "dashboard");

        builder.HasKey(site => site.Id);

        builder.Property(site => site.Name)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(site => site.Code)
            .HasMaxLength(32)
            .IsRequired();

        builder.HasIndex(site => site.Code)
            .IsUnique();

        builder.Property(site => site.Region)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(site => site.IsActive)
            .IsRequired();

        builder.HasIndex(site => site.IsActive);

        builder.Property(site => site.CreatedAtUtc)
            .IsRequired();

        builder.Property(site => site.UpdatedAtUtc)
            .IsRequired();
    }
}
