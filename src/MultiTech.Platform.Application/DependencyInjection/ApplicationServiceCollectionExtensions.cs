using Microsoft.Extensions.DependencyInjection;
using MultiTech.Platform.Application.Abstractions.Messaging;
using MultiTech.Platform.Application.Features.Authentication.GetCurrentUser;
using MultiTech.Platform.Application.Features.Authentication.Login;
using MultiTech.Platform.Application.Features.Authentication.Logout;
using MultiTech.Platform.Application.Features.Authentication.Refresh;
using MultiTech.Platform.Application.Features.Authentication.Register;

namespace MultiTech.Platform.Application.DependencyInjection;

/// <summary>
/// Registers application-layer services.
/// </summary>
public static class ApplicationServiceCollectionExtensions
{
    /// <summary>
    /// Adds application-layer dependencies to the service collection.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <returns>The configured service collection.</returns>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddScoped<ICommandHandler<RegisterUserCommand, RegisterUserResult>, RegisterUserCommandHandler>();
        services.AddScoped<ICommandHandler<LoginCommand, LoginResult>, LoginCommandHandler>();
        services.AddScoped<ICommandHandler<RefreshCommand, LoginResult>, RefreshCommandHandler>();
        services.AddScoped<ICommandHandler<LogoutCommand>, LogoutCommandHandler>();
        services.AddScoped<IQueryHandler<GetCurrentUserQuery, Features.Authentication.AuthenticatedUserDto>, GetCurrentUserQueryHandler>();

        return services;
    }
}
