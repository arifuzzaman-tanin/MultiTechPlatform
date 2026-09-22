using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MultiTech.Platform.Application.Abstractions.Authentication;
using MultiTech.Platform.Application.Abstractions.Clock;
using MultiTech.Platform.Application.Abstractions.Persistence;
using MultiTech.Platform.Infrastructure.Authentication;
using MultiTech.Platform.Infrastructure.Persistence;
using MultiTech.Platform.Infrastructure.Persistence.Seed;
using MultiTech.Platform.Infrastructure.Services;

namespace MultiTech.Platform.Infrastructure.DependencyInjection;

/// <summary>
/// Registers infrastructure-layer services.
/// </summary>
public static class InfrastructureServiceCollectionExtensions
{
    /// <summary>
    /// Adds infrastructure-layer dependencies to the service collection.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <param name="configuration">The application configuration.</param>
    /// <returns>The configured service collection.</returns>
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        services.AddOptions<JwtOptions>()
            .Bind(configuration.GetSection(JwtOptions.SectionName))
            .ValidateOnStart();

        services.AddSingleton<IValidateOptions<JwtOptions>, JwtOptionsValidator>();

        services.AddDbContext<PlatformDbContext>(options =>
        {
            if (configuration.GetValue<bool>("UseInMemoryDatabaseForTests"))
            {
                options.UseInMemoryDatabase("MultiTechPlatformTests");
                return;
            }

            options.UseSqlServer(configuration.GetConnectionString("Database"));
        });

        services.AddScoped<IApplicationDbContext>(serviceProvider =>
            serviceProvider.GetRequiredService<PlatformDbContext>());
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IDashboardOverviewReadRepository, DashboardOverviewReadRepository>();
        services.AddScoped<DashboardOverviewSeeder>();

        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IAccessTokenProvider, JwtAccessTokenProvider>();
        services.AddSingleton<IRefreshTokenGenerator, RefreshTokenGenerator>();
        services.AddSingleton<ITokenHasher, TokenHasher>();
        services.AddSingleton<IRefreshTokenLifetimeProvider, JwtRefreshTokenLifetimeProvider>();
        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
        services.AddScoped<ICurrentUser, CurrentUser>();
        services.AddHttpContextAccessor();

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer();

        services
            .AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
            .Configure<IOptions<JwtOptions>>((options, jwtOptionsAccessor) =>
            {
                JwtOptions jwtOptions = jwtOptionsAccessor.Value;
                SymmetricSecurityKey signingKey = new(Encoding.UTF8.GetBytes(jwtOptions.SigningKey))
                {
                    KeyId = "multitech-platform-signing-key"
                };

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtOptions.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwtOptions.Audience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = signingKey,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromMinutes(1)
                };
            });

        services.AddAuthorization();

        return services;
    }
}
