using System.Net;
using System.Net.Http.Json;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace MultiTech.Platform.FunctionalTests;

public sealed class DashboardOverviewGraphQlFunctionalTests
{
    [Fact]
    public async Task DashboardOverviewQuery_ValidRequest_ReturnsExpectedFields()
    {
        await using DashboardApiFactory factory = new();
        using HttpClient client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
            BaseAddress = new Uri("https://localhost")
        });

        HttpResponseMessage response = await client.PostAsJsonAsync(
            "/graphql",
            new
            {
                query = """
                    query DashboardOverview {
                      dashboardOverview {
                        managedDevices
                        onlineDevices
                        gateways
                        sensors
                        criticalAlerts
                        connectivityHealthPercent
                        messagesLast24Hours
                        sites
                        lastUpdatedUtc
                      }
                    }
                    """
            });

        string body = await response.Content.ReadAsStringAsync();
        Assert.True(
            response.StatusCode == HttpStatusCode.OK,
            $"Expected 200 OK but received {(int)response.StatusCode}: {body}");

        JsonNode? json = JsonNode.Parse(body);
        JsonNode? overview = json?["data"]?["dashboardOverview"];

        Assert.NotNull(overview);
        Assert.NotNull(overview["managedDevices"]);
        Assert.NotNull(overview["onlineDevices"]);
        Assert.NotNull(overview["gateways"]);
        Assert.NotNull(overview["sensors"]);
        Assert.NotNull(overview["criticalAlerts"]);
        Assert.NotNull(overview["connectivityHealthPercent"]);
        Assert.NotNull(overview["messagesLast24Hours"]);
        Assert.NotNull(overview["sites"]);
        Assert.NotNull(overview["lastUpdatedUtc"]);
    }

    private sealed class DashboardApiFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureAppConfiguration(configurationBuilder =>
            {
                configurationBuilder.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["ConnectionStrings:Database"] = "DataSource=:memory:",
                    ["UseInMemoryDatabaseForTests"] = "true",
                    ["Jwt:Issuer"] = "MultiTech.Platform.Tests",
                    ["Jwt:Audience"] = "MultiTech.Platform.FunctionalTests",
                    ["Jwt:SigningKey"] = "FunctionalTestSigningKeyChangeOutsideSourceControl12345",
                    ["Jwt:AccessTokenLifetimeMinutes"] = "15",
                    ["Jwt:RefreshTokenLifetimeDays"] = "7"
                });
            });
        }
    }
}
