using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MultiTech.Platform.Infrastructure.Persistence;

#nullable disable

namespace MultiTech.Platform.Infrastructure.Persistence.Migrations
{
    [DbContext(typeof(PlatformDbContext))]
    partial class PlatformDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "10.0.12")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("MultiTech.Platform.Domain.Users.RefreshToken", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTimeOffset>("CreatedAtUtc")
                        .HasColumnType("datetimeoffset");

                    b.Property<DateTimeOffset>("ExpiresAtUtc")
                        .HasColumnType("datetimeoffset");

                    b.Property<Guid?>("ReplacedByTokenId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTimeOffset?>("RevokedAtUtc")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("TokenHash")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("ExpiresAtUtc");

                    b.HasIndex("TokenHash")
                        .IsUnique();

                    b.HasIndex("UserId");

                    b.ToTable("RefreshTokens", "auth");
                });

            modelBuilder.Entity("MultiTech.Platform.Domain.Users.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("CompanyName")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(320)
                        .HasColumnType("nvarchar(320)");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(150)
                        .HasColumnType("nvarchar(150)");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("Email")
                        .IsUnique()
                        .HasDatabaseName("IX_Users_Email");

                    b.ToTable("Users", "auth");
                });

            modelBuilder.Entity("MultiTech.Platform.Domain.Users.RefreshToken", b =>
                {
                    b.HasOne("MultiTech.Platform.Domain.Users.User", null)
                        .WithMany("RefreshTokens")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("MultiTech.Platform.Domain.Users.User", b =>
                {
                    b.Navigation("RefreshTokens");
                });
#pragma warning restore 612, 618
        }
    }
}

