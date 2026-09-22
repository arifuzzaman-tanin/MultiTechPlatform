using Microsoft.Extensions.Logging;
using MultiTech.Platform.Application.Abstractions.Authentication;
using MultiTech.Platform.Application.Abstractions.Clock;
using MultiTech.Platform.Application.Abstractions.Messaging;
using MultiTech.Platform.Application.Abstractions.Persistence;
using MultiTech.Platform.Application.Common.Results;
using MultiTech.Platform.Domain.Users;

namespace MultiTech.Platform.Application.Features.Authentication.Logout;

/// <summary>
/// Handles refresh-token logout.
/// </summary>
public sealed class LogoutCommandHandler : ICommandHandler<LogoutCommand>
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IApplicationDbContext _dbContext;
    private readonly ITokenHasher _tokenHasher;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ILogger<LogoutCommandHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="LogoutCommandHandler"/> class.
    /// </summary>
    public LogoutCommandHandler(
        IRefreshTokenRepository refreshTokenRepository,
        IApplicationDbContext dbContext,
        ITokenHasher tokenHasher,
        IDateTimeProvider dateTimeProvider,
        ILogger<LogoutCommandHandler> logger)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _dbContext = dbContext;
        _tokenHasher = tokenHasher;
        _dateTimeProvider = dateTimeProvider;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<Result> Handle(
        LogoutCommand command,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(command.RefreshToken))
        {
            return Result.Success();
        }

        string refreshTokenHash = _tokenHasher.Hash(command.RefreshToken);
        RefreshToken? refreshToken = await _refreshTokenRepository.GetByHashAsync(
            refreshTokenHash,
            cancellationToken);

        if (refreshToken is not null && !refreshToken.IsRevoked)
        {
            refreshToken.Revoke(_dateTimeProvider.UtcNow);
            await _dbContext.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Logout completed for UserId {UserId}.", refreshToken.UserId);
        }

        return Result.Success();
    }
}

