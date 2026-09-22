using MultiTech.Platform.Application.Abstractions.Messaging;

namespace MultiTech.Platform.Application.Features.Authentication.Register;

/// <summary>
/// Registers a new user.
/// </summary>
/// <param name="Name">The display name.</param>
/// <param name="Email">The email address.</param>
/// <param name="Company">The company name.</param>
/// <param name="Password">The plain-text password.</param>
public sealed record RegisterUserCommand(
    string Name,
    string Email,
    string Company,
    string Password)
    : ICommand<RegisterUserResult>;

