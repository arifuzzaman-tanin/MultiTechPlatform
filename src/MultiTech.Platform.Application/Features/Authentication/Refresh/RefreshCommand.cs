using MultiTech.Platform.Application.Abstractions.Messaging;
using MultiTech.Platform.Application.Features.Authentication.Login;

namespace MultiTech.Platform.Application.Features.Authentication.Refresh;

/// <summary>
/// Refreshes an access token and rotates the refresh token.
/// </summary>
/// <param name="RefreshToken">The raw refresh token.</param>
public sealed record RefreshCommand(string RefreshToken) : ICommand<LoginResult>;

