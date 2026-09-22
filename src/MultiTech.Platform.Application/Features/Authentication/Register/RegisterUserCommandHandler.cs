using Microsoft.Extensions.Logging;
using MultiTech.Platform.Application.Abstractions.Authentication;
using MultiTech.Platform.Application.Abstractions.Messaging;
using MultiTech.Platform.Application.Abstractions.Persistence;
using MultiTech.Platform.Application.Common.Results;
using MultiTech.Platform.Domain.Users;

namespace MultiTech.Platform.Application.Features.Authentication.Register;

/// <summary>
/// Handles user registration.
/// </summary>
public sealed class RegisterUserCommandHandler
    : ICommandHandler<RegisterUserCommand, RegisterUserResult>
{
    private readonly IUserRepository _userRepository;
    private readonly IApplicationDbContext _dbContext;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ILogger<RegisterUserCommandHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="RegisterUserCommandHandler"/> class.
    /// </summary>
    public RegisterUserCommandHandler(
        IUserRepository userRepository,
        IApplicationDbContext dbContext,
        IPasswordHasher passwordHasher,
        ILogger<RegisterUserCommandHandler> logger)
    {
        _userRepository = userRepository;
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<Result<RegisterUserResult>> Handle(
        RegisterUserCommand command,
        CancellationToken cancellationToken)
    {
        Error? validationError = AuthValidation.ValidateRegistration(
            command.Name,
            command.Email,
            command.Company,
            command.Password);

        if (validationError is not null)
        {
            return Result<RegisterUserResult>.Failure(validationError);
        }

        string normalizedEmail = AuthValidation.NormalizeEmail(command.Email);
        if (await _userRepository.ExistsByEmailAsync(normalizedEmail, cancellationToken))
        {
            return Result<RegisterUserResult>.Failure(AuthErrors.EmailAlreadyExists);
        }

        string trimmedName = command.Name.Trim();
        string trimmedCompany = command.Company.Trim();
        User user = User.Create(trimmedName, normalizedEmail, trimmedCompany, "pending");
        string passwordHash = _passwordHasher.Hash(user, command.Password);
        user.SetPasswordHash(passwordHash);

        _userRepository.Add(user);

        try
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DuplicateEmailException)
        {
            return Result<RegisterUserResult>.Failure(AuthErrors.EmailAlreadyExists);
        }

        _logger.LogInformation(
            "Registration succeeded for UserId {UserId}.",
            user.Id);

        return Result<RegisterUserResult>.Success(new RegisterUserResult(
            user.Id,
            user.Name,
            user.Email,
            user.CompanyName));
    }
}
