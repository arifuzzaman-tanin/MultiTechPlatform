using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using MultiTech.Platform.Contracts.Authentication;

namespace MultiTech.Platform.FunctionalTests;

public sealed class AuthenticationFunctionalTests
{
    [Fact]
    public async Task AuthenticationFlow_ValidRequests_CompletesEndToEnd()
    {
        await using AuthenticationApiFactory factory = new();
        using HttpClient client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
            BaseAddress = new Uri("https://localhost")
        });

        HttpResponseMessage registerResponse = await client.PostAsJsonAsync(
            "/api/v1/auth/register",
            new RegisterRequest(
                " John Smith ",
                "John.Smith@example.com",
                " Acme Manufacturing ",
                "StrongPassword123!"));

        string registerBody = await registerResponse.Content.ReadAsStringAsync();
        Assert.True(
            registerResponse.StatusCode == HttpStatusCode.Created,
            $"Expected 201 Created but received {(int)registerResponse.StatusCode}: {registerBody}");

        RegisterResponse? registeredUser = await registerResponse.Content
            .ReadFromJsonAsync<RegisterResponse>();

        Assert.NotNull(registeredUser);
        Assert.Equal("John Smith", registeredUser.Name);
        Assert.Equal("john.smith@example.com", registeredUser.Email);
        Assert.Equal("Acme Manufacturing", registeredUser.Company);

        HttpResponseMessage duplicateResponse = await client.PostAsJsonAsync(
            "/api/v1/auth/register",
            new RegisterRequest(
                "John Smith",
                "john.smith@example.com",
                "Acme Manufacturing",
                "StrongPassword123!"));

        Assert.Equal(HttpStatusCode.Conflict, duplicateResponse.StatusCode);

        HttpResponseMessage loginResponse = await client.PostAsJsonAsync(
            "/api/v1/auth/login",
            new LoginRequest("john.smith@example.com", "StrongPassword123!"));

        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);
        LoginResponse? login = await loginResponse.Content.ReadFromJsonAsync<LoginResponse>();

        Assert.NotNull(login);
        Assert.False(string.IsNullOrWhiteSpace(login.AccessToken));
        Assert.Equal(registeredUser.Id, login.User.Id);
        Assert.Contains(
            loginResponse.Headers.GetValues("Set-Cookie"),
            value => value.Contains("__Host-multitech-refresh", StringComparison.Ordinal));

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer",
            login.AccessToken);

        HttpResponseMessage meResponse = await client.GetAsync("/api/v1/auth/me");
        string meBody = await meResponse.Content.ReadAsStringAsync();
        string authenticateHeader = string.Join(
            ", ",
            meResponse.Headers.WwwAuthenticate.Select(value => value.ToString()));
        Assert.True(
            meResponse.StatusCode == HttpStatusCode.OK,
            $"Expected 200 OK but received {(int)meResponse.StatusCode}. WWW-Authenticate: {authenticateHeader}. Body: {meBody}");

        UserSummaryResponse? currentUser = await meResponse.Content
            .ReadFromJsonAsync<UserSummaryResponse>();

        Assert.NotNull(currentUser);
        Assert.Equal(registeredUser.Id, currentUser.Id);

        HttpResponseMessage refreshResponse = await client.PostAsync(
            "/api/v1/auth/refresh",
            content: null);

        Assert.Equal(HttpStatusCode.OK, refreshResponse.StatusCode);
        LoginResponse? refreshedLogin = await refreshResponse.Content.ReadFromJsonAsync<LoginResponse>();

        Assert.NotNull(refreshedLogin);
        Assert.False(string.IsNullOrWhiteSpace(refreshedLogin.AccessToken));

        HttpResponseMessage logoutResponse = await client.PostAsync(
            "/api/v1/auth/logout",
            content: null);

        Assert.Equal(HttpStatusCode.NoContent, logoutResponse.StatusCode);
    }

    private sealed class AuthenticationApiFactory : WebApplicationFactory<Program>
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
