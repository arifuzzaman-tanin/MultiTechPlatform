using Microsoft.Extensions.Logging;
using MultiTech.Platform.Application.Abstractions.Authentication;
using MultiTech.Platform.Application.Abstractions.Clock;
using MultiTech.Platform.Application.Abstractions.Messaging;
using MultiTech.Platform.Application.Abstractions.Persistence;
using MultiTech.Platform.Application.Common.Results;
using MultiTech.Platform.Application.Features.Authentication.Login;
using MultiTech.Platform.Domain.Users;

namespace MultiTech.Platform.Application.Features.Authentication.Refresh;

/// <summary>
/// Handles refresh-token rotation.
/// </summary>
public sealed class RefreshCommandHandler : ICommandHandler<RefreshCommand, LoginResult>
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IUserRepository _userRepository;
    private readonly IApplicationDbContext _dbContext;
    private readonly IAccessTokenProvider _accessTokenProvider;
    private readonly IRefreshTokenGenerator _refreshTokenGenerator;
    private readonly ITokenHasher _tokenHasher;
    private readonly IRefreshTokenLifetimeProvider _refreshTokenLifetimeProvider;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ILogger<RefreshCommandHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="RefreshCommandHandler"/> class.
    /// </summary>
    public RefreshCommandHandler(
        IRefreshTokenRepository refreshTokenRepository,
        IUserRepository userRepository,
        IApplicationDbContext dbContext,
        IAccessTokenProvider accessTokenProvider,
        IRefreshTokenGenerator refreshTokenGenerator,
        ITokenHasher tokenHasher,
        IRefreshTokenLifetimeProvider refreshTokenLifetimeProvider,
        IDateTimeProvider dateTimeProvider,
        ILogger<RefreshCommandHandler> logger)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _userRepository = userRepository;
        _dbContext = dbContext;
        _accessTokenProvider = accessTokenProvider;
        _refreshTokenGenerator = refreshTokenGenerator;
        _tokenHasher = tokenHasher;
        _refreshTokenLifetimeProvider = refreshTokenLifetimeProvider;
        _dateTimeProvider = dateTimeProvider;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<Result<LoginResult>> Handle(
        RefreshCommand command,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(command.RefreshToken))
        {
            return Result<LoginResult>.Failure(AuthErrors.InvalidRefreshToken);
        }

        string refreshTokenHash = _tokenHasher.Hash(command.RefreshToken);
        RefreshToken? existingToken = await _refreshTokenRepository.GetByHashAsync(
            refreshTokenHash,
            cancellationToken);

        DateTimeOffset now = _dateTimeProvider.UtcNow;
        if (existingToken is null || existingToken.IsRevoked || existingToken.IsExpired(now))
        {
            _logger.LogWarning("Invalid refresh token was rejected.");
            return Result<LoginResult>.Failure(AuthErrors.InvalidRefreshToken);
        }

        User? user = await _userRepository.GetByIdAsync(existingToken.UserId, cancellationToken);
        if (user is null || !user.IsActive)
        {
            return Result<LoginResult>.Failure(AuthErrors.InvalidRefreshToken);
        }

        string replacementTokenValue = _refreshTokenGenerator.Generate();
        RefreshToken replacementToken = RefreshToken.Create(
            user.Id,
            _tokenHasher.Hash(replacementTokenValue),
            now,
            now.Add(_refreshTokenLifetimeProvider.RefreshTokenLifetime));

        existingToken.Revoke(now, replacementToken.Id);
        _refreshTokenRepository.Add(replacementToken);

        AccessTokenResult accessToken = _accessTokenProvider.Create(user);
        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Refresh token rotated for UserId {UserId}.", user.Id);

        return Result<LoginResult>.Success(new LoginResult(
            accessToken.Token,
            accessToken.ExpiresAtUtc,
            replacementTokenValue,
            replacementToken.ExpiresAtUtc,
            new AuthenticatedUserDto(
                user.Id,
                user.Name,
                user.Email,
                user.CompanyName)));
    }
}

