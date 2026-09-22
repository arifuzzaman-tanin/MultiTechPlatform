using Microsoft.Extensions.Logging;
using MultiTech.Platform.Application.Abstractions.Authentication;
using MultiTech.Platform.Application.Abstractions.Clock;
using MultiTech.Platform.Application.Abstractions.Messaging;
using MultiTech.Platform.Application.Abstractions.Persistence;
using MultiTech.Platform.Application.Common.Results;
using MultiTech.Platform.Domain.Users;

namespace MultiTech.Platform.Application.Features.Authentication.Login;

/// <summary>
/// Handles user login.
/// </summary>
public sealed class LoginCommandHandler : ICommandHandler<LoginCommand, LoginResult>
{
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IApplicationDbContext _dbContext;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IAccessTokenProvider _accessTokenProvider;
    private readonly IRefreshTokenGenerator _refreshTokenGenerator;
    private readonly ITokenHasher _tokenHasher;
    private readonly IRefreshTokenLifetimeProvider _refreshTokenLifetimeProvider;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ILogger<LoginCommandHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="LoginCommandHandler"/> class.
    /// </summary>
    public LoginCommandHandler(
        IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IApplicationDbContext dbContext,
        IPasswordHasher passwordHasher,
        IAccessTokenProvider accessTokenProvider,
        IRefreshTokenGenerator refreshTokenGenerator,
        ITokenHasher tokenHasher,
        IRefreshTokenLifetimeProvider refreshTokenLifetimeProvider,
        IDateTimeProvider dateTimeProvider,
        ILogger<LoginCommandHandler> logger)
    {
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
        _accessTokenProvider = accessTokenProvider;
        _refreshTokenGenerator = refreshTokenGenerator;
        _tokenHasher = tokenHasher;
        _refreshTokenLifetimeProvider = refreshTokenLifetimeProvider;
        _dateTimeProvider = dateTimeProvider;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<Result<LoginResult>> Handle(
        LoginCommand command,
        CancellationToken cancellationToken)
    {
        Error? validationError = AuthValidation.ValidateLogin(command.Email, command.Password);
        if (validationError is not null)
        {
            return Result<LoginResult>.Failure(validationError);
        }

        string normalizedEmail = AuthValidation.NormalizeEmail(command.Email);
        User? user = await _userRepository.GetByEmailAsync(normalizedEmail, cancellationToken);

        if (user is null || !user.IsActive)
        {
            _logger.LogWarning("Login failed for normalized email {Email}.", normalizedEmail);
            return Result<LoginResult>.Failure(AuthErrors.InvalidCredentials);
        }

        if (!_passwordHasher.Verify(user, user.PasswordHash, command.Password))
        {
            _logger.LogWarning("Login failed for UserId {UserId}.", user.Id);
            return Result<LoginResult>.Failure(AuthErrors.InvalidCredentials);
        }

        LoginResult loginResult = IssueTokens(user);
        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Login succeeded for UserId {UserId}.", user.Id);

        return Result<LoginResult>.Success(loginResult);
    }

    private LoginResult IssueTokens(User user)
    {
        AccessTokenResult accessToken = _accessTokenProvider.Create(user);
        string refreshTokenValue = _refreshTokenGenerator.Generate();
        string refreshTokenHash = _tokenHasher.Hash(refreshTokenValue);
        DateTimeOffset now = _dateTimeProvider.UtcNow;
        DateTimeOffset refreshTokenExpiresAtUtc = now.Add(_refreshTokenLifetimeProvider.RefreshTokenLifetime);

        RefreshToken refreshToken = RefreshToken.Create(
            user.Id,
            refreshTokenHash,
            now,
            refreshTokenExpiresAtUtc);

        _refreshTokenRepository.Add(refreshToken);

        return new LoginResult(
            accessToken.Token,
            accessToken.ExpiresAtUtc,
            refreshTokenValue,
            refreshTokenExpiresAtUtc,
            new AuthenticatedUserDto(
                user.Id,
                user.Name,
                user.Email,
                user.CompanyName));
    }
}
