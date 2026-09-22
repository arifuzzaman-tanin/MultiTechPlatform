using Microsoft.AspNetCore.RateLimiting;

namespace MultiTech.Platform.Api.DependencyInjection;

/// <summary>
/// Registers presentation-layer services.
/// </summary>
public static class PresentationServiceCollectionExtensions
{
    private const string AuthRateLimitPolicyName = "auth";

    /// <summary>
    /// Adds presentation-layer dependencies to the service collection.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <returns>The configured service collection.</returns>
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddOpenApi();
        services.AddHealthChecks();
        services.AddProblemDetails();
        services.AddRateLimiter(options =>
        {
            options.AddFixedWindowLimiter(AuthRateLimitPolicyName, limiterOptions =>
            {
                limiterOptions.PermitLimit = 10;
                limiterOptions.Window = TimeSpan.FromMinutes(1);
                limiterOptions.QueueLimit = 0;
            });
        });

        return services;
    }
}
