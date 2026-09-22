using MultiTech.Platform.Application.Abstractions.Messaging;

namespace MultiTech.Platform.Application.Features.Authentication.Login;

/// <summary>
/// Authenticates a user with email and password.
/// </summary>
/// <param name="Email">The email address.</param>
/// <param name="Password">The plain-text password.</param>
public sealed record LoginCommand(
    string Email,
    string Password)
    : ICommand<LoginResult>;

