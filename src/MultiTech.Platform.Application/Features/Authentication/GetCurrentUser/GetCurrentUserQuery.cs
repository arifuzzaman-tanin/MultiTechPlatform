using MultiTech.Platform.Application.Abstractions.Messaging;

namespace MultiTech.Platform.Application.Features.Authentication.GetCurrentUser;

/// <summary>
/// Gets the current authenticated user.
/// </summary>
public sealed record GetCurrentUserQuery : IQuery<AuthenticatedUserDto>;

