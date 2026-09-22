using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MultiTech.Platform.Application.Abstractions.Authentication;
using MultiTech.Platform.Application.Abstractions.Clock;
using MultiTech.Platform.Domain.Users;

namespace MultiTech.Platform.Infrastructure.Authentication;

/// <summary>
/// Creates signed JWT access tokens.
/// </summary>
public sealed class JwtAccessTokenProvider : IAccessTokenProvider
{
    private readonly JwtOptions _jwtOptions;
    private readonly IDateTimeProvider _dateTimeProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="JwtAccessTokenProvider"/> class.
    /// </summary>
    public JwtAccessTokenProvider(
        IOptions<JwtOptions> jwtOptions,
        IDateTimeProvider dateTimeProvider)
    {
        _jwtOptions = jwtOptions.Value;
        _dateTimeProvider = dateTimeProvider;
    }

    /// <inheritdoc />
    public AccessTokenResult Create(User user)
    {
        DateTimeOffset expiresAtUtc = _dateTimeProvider.UtcNow.AddMinutes(
            _jwtOptions.AccessTokenLifetimeMinutes);

        Claim[] claims =
        [
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(JwtRegisteredClaimNames.Name, user.Name),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        ];

        SymmetricSecurityKey securityKey = new(Encoding.UTF8.GetBytes(_jwtOptions.SigningKey))
        {
            KeyId = "multitech-platform-signing-key"
        };
        SigningCredentials signingCredentials = new(
            securityKey,
            SecurityAlgorithms.HmacSha256);

        JwtSecurityToken token = new(
            _jwtOptions.Issuer,
            _jwtOptions.Audience,
            claims,
            expires: expiresAtUtc.UtcDateTime,
            signingCredentials: signingCredentials);

        string tokenValue = new JwtSecurityTokenHandler().WriteToken(token);

        return new AccessTokenResult(tokenValue, expiresAtUtc);
    }
}
