using MultiTech.Platform.Application.Abstractions.Messaging;

namespace MultiTech.Platform.Application.Features.Authentication.Logout;

/// <summary>
/// Logs out a refresh-token session.
/// </summary>
/// <param name="RefreshToken">The raw refresh token, when present.</param>
public sealed record LogoutCommand(string? RefreshToken) : ICommand;

