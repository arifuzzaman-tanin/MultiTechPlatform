using Microsoft.Extensions.Options;

namespace MultiTech.Platform.Infrastructure.Authentication;

/// <summary>
/// Validates JWT options at startup.
/// </summary>
public sealed class JwtOptionsValidator : IValidateOptions<JwtOptions>
{
    /// <inheritdoc />
    public ValidateOptionsResult Validate(string? name, JwtOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.Issuer))
        {
            return ValidateOptionsResult.Fail("Jwt:Issuer is required.");
        }

        if (string.IsNullOrWhiteSpace(options.Audience))
        {
            return ValidateOptionsResult.Fail("Jwt:Audience is required.");
        }

        if (string.IsNullOrWhiteSpace(options.SigningKey) || options.SigningKey.Length < 32)
        {
            return ValidateOptionsResult.Fail("Jwt:SigningKey must be at least 32 characters.");
        }

        if (options.AccessTokenLifetimeMinutes <= 0)
        {
            return ValidateOptionsResult.Fail("Jwt:AccessTokenLifetimeMinutes must be positive.");
        }

        if (options.RefreshTokenLifetimeDays <= 0)
        {
            return ValidateOptionsResult.Fail("Jwt:RefreshTokenLifetimeDays must be positive.");
        }

        return ValidateOptionsResult.Success;
    }
}

