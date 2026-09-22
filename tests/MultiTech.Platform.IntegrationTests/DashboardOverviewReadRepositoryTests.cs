using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using MultiTech.Platform.Infrastructure.Persistence;
using MultiTech.Platform.Infrastructure.Persistence.Seed;

namespace MultiTech.Platform.IntegrationTests;

public sealed class DashboardOverviewReadRepositoryTests
{
    [Fact]
    public async Task GetOverviewAsync_SeededDashboardData_ReturnsExpectedSummary()
    {
        await using PlatformDbContext dbContext = CreateDbContext();
        DashboardOverviewSeeder seeder = new(
            dbContext,
            NullLogger<DashboardOverviewSeeder>.Instance);
        await seeder.SeedAsync(CancellationToken.None);

        DashboardOverviewReadRepository repository = new(dbContext);

        var overview = await repository.GetOverviewAsync(
            new DateTimeOffset(2026, 9, 22, 6, 0, 0, TimeSpan.Zero),
            CancellationToken.None);

        Assert.Equal(248, overview.ManagedDevices);
        Assert.Equal(232, overview.OnlineDevices);
        Assert.Equal(24, overview.Gateways);
        Assert.Equal(224, overview.Sensors);
        Assert.Equal(9, overview.CriticalAlerts);
        Assert.Equal(93.55m, overview.ConnectivityHealthPercent);
        Assert.Equal(10, overview.Sites);
        Assert.Equal(4_029_260, overview.MessagesLast24Hours);
    }

    [Fact]
    public async Task GetOverviewAsync_NoManagedDevices_ReturnsZeroConnectivityHealth()
    {
        await using PlatformDbContext dbContext = CreateDbContext();
        DashboardOverviewReadRepository repository = new(dbContext);

        var overview = await repository.GetOverviewAsync(
            new DateTimeOffset(2026, 9, 22, 6, 0, 0, TimeSpan.Zero),
            CancellationToken.None);

        Assert.Equal(0, overview.ManagedDevices);
        Assert.Equal(0m, overview.ConnectivityHealthPercent);
        Assert.Equal(0, overview.MessagesLast24Hours);
    }

    private static PlatformDbContext CreateDbContext()
    {
        DbContextOptions<PlatformDbContext> options = new DbContextOptionsBuilder<PlatformDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new PlatformDbContext(options);
    }
}
