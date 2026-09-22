using MultiTech.Platform.Application.Abstractions.Authentication;
using MultiTech.Platform.Application.Abstractions.Messaging;
using MultiTech.Platform.Application.Abstractions.Persistence;
using MultiTech.Platform.Application.Common.Results;
using MultiTech.Platform.Domain.Users;

namespace MultiTech.Platform.Application.Features.Authentication.GetCurrentUser;

/// <summary>
/// Handles current-user lookup.
/// </summary>
public sealed class GetCurrentUserQueryHandler
    : IQueryHandler<GetCurrentUserQuery, AuthenticatedUserDto>
{
    private readonly ICurrentUser _currentUser;
    private readonly IUserRepository _userRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetCurrentUserQueryHandler"/> class.
    /// </summary>
    public GetCurrentUserQueryHandler(
        ICurrentUser currentUser,
        IUserRepository userRepository)
    {
        _currentUser = currentUser;
        _userRepository = userRepository;
    }

    /// <inheritdoc />
    public async Task<Result<AuthenticatedUserDto>> Handle(
        GetCurrentUserQuery query,
        CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAuthenticated || _currentUser.UserId is not { } userId)
        {
            return Result<AuthenticatedUserDto>.Failure(AuthErrors.InvalidCredentials);
        }

        User? user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user is null)
        {
            return Result<AuthenticatedUserDto>.Failure(AuthErrors.CurrentUserNotFound);
        }

        return Result<AuthenticatedUserDto>.Success(new AuthenticatedUserDto(
            user.Id,
            user.Name,
            user.Email,
            user.CompanyName));
    }
}

