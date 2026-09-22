using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MultiTech.Platform.Domain.Users;

namespace MultiTech.Platform.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configures the user entity.
/// </summary>
public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users", "auth");

        builder.HasKey(user => user.Id);

        builder.Property(user => user.Name)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(user => user.Email)
            .HasMaxLength(320)
            .IsRequired();

        builder.HasIndex(user => user.Email)
            .IsUnique()
            .HasDatabaseName("IX_Users_Email");

        builder.Property(user => user.CompanyName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(user => user.PasswordHash)
            .IsRequired();

        builder.Property(user => user.IsActive)
            .IsRequired();

        builder.HasMany(user => user.RefreshTokens)
            .WithOne()
            .HasForeignKey(refreshToken => refreshToken.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(user => user.RefreshTokens)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}

