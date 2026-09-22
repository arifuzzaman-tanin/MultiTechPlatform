namespace MultiTech.Platform.Api.DependencyInjection;

/// <summary>
/// Registers presentation-layer services.
/// </summary>
public static class PresentationServiceCollectionExtensions
{
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

        return services;
    }
}
