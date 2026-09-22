using MultiTech.Platform.Api.DependencyInjection;
using MultiTech.Platform.Api.Endpoints.Auth;
using MultiTech.Platform.Application.DependencyInjection;
using MultiTech.Platform.Infrastructure.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration)
    .AddPresentation();

var app = builder.Build();

await app.ApplyDatabaseMigrationsAsync();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "MultiTech Platform API v1");
    });
}

app.UseHttpsRedirection();
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();

app.MapHealthChecks("/health");
app.MapAuthEndpoints();

app.Run();

/// <summary>
/// Exposes the entry point to functional tests.
/// </summary>
public partial class Program;
