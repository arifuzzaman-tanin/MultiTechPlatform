using MultiTech.Platform.Api.Http;
using MultiTech.Platform.Application.Abstractions.Messaging;
using MultiTech.Platform.Application.Features.Authentication;
using MultiTech.Platform.Application.Features.Authentication.GetCurrentUser;
using MultiTech.Platform.Application.Features.Authentication.Login;
using MultiTech.Platform.Application.Features.Authentication.Logout;
using MultiTech.Platform.Application.Features.Authentication.Refresh;
using MultiTech.Platform.Application.Features.Authentication.Register;
using MultiTech.Platform.Contracts.Authentication;
using ApplicationResult = MultiTech.Platform.Application.Common.Results.Result;
using RegisterUserApplicationResult = MultiTech.Platform.Application.Common.Results.Result<MultiTech.Platform.Application.Features.Authentication.Register.RegisterUserResult>;
using LoginApplicationResult = MultiTech.Platform.Application.Common.Results.Result<MultiTech.Platform.Application.Features.Authentication.Login.LoginResult>;
using AuthenticatedUserApplicationResult = MultiTech.Platform.Application.Common.Results.Result<MultiTech.Platform.Application.Features.Authentication.AuthenticatedUserDto>;

namespace MultiTech.Platform.Api.Endpoints.Auth;

/// <summary>
/// Maps authentication endpoints.
/// </summary>
public static class AuthEndpoints
{
    private const string RefreshTokenCookieName = "__Host-multitech-refresh";
    private const string AuthRateLimitPolicyName = "auth";

    /// <summary>
    /// Maps authentication routes.
    /// </summary>
    /// <param name="app">The endpoint route builder.</param>
    /// <returns>The configured endpoint route builder.</returns>
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        RouteGroupBuilder group = app
            .MapGroup("/api/v1/auth")
            .WithTags("Authentication");

        group.MapPost("/register", RegisterAsync)
            .RequireRateLimiting(AuthRateLimitPolicyName);

        group.MapPost("/login", LoginAsync)
            .RequireRateLimiting(AuthRateLimitPolicyName);

        group.MapPost("/refresh", RefreshAsync)
            .RequireRateLimiting(AuthRateLimitPolicyName);

        group.MapPost("/logout", LogoutAsync);

        group.MapGet("/me", MeAsync)
            .RequireAuthorization();

        return app;
    }

    private static async Task<IResult> RegisterAsync(
        RegisterRequest request,
        ICommandHandler<RegisterUserCommand, RegisterUserResult> handler,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        RegisterUserApplicationResult result = await handler.Handle(
            new RegisterUserCommand(
                request.Name,
                request.Email,
                request.Company,
                request.Password),
            cancellationToken);

        if (result.IsFailure || result.Value is null)
        {
            return HttpResults.Problem(result.Error!, httpContext);
        }

        RegisterResponse response = new(
            result.Value.Id,
            result.Value.Name,
            result.Value.Email,
            result.Value.Company);

        return Results.Created("/api/v1/auth/me", response);
    }

    private static async Task<IResult> LoginAsync(
        LoginRequest request,
        ICommandHandler<LoginCommand, LoginResult> handler,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        LoginApplicationResult result = await handler.Handle(
            new LoginCommand(request.Email, request.Password),
            cancellationToken);

        if (result.IsFailure || result.Value is null)
        {
            return HttpResults.Problem(result.Error!, httpContext);
        }

        AppendRefreshTokenCookie(httpContext.Response, result.Value);

        return Results.Ok(ToLoginResponse(result.Value));
    }

    private static async Task<IResult> RefreshAsync(
        ICommandHandler<RefreshCommand, LoginResult> handler,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        string? refreshToken = httpContext.Request.Cookies[RefreshTokenCookieName];

        LoginApplicationResult result = await handler.Handle(
            new RefreshCommand(refreshToken ?? string.Empty),
            cancellationToken);

        if (result.IsFailure || result.Value is null)
        {
            ClearRefreshTokenCookie(httpContext.Response);
            return HttpResults.Problem(result.Error!, httpContext);
        }

        AppendRefreshTokenCookie(httpContext.Response, result.Value);

        return Results.Ok(ToLoginResponse(result.Value));
    }

    private static async Task<IResult> LogoutAsync(
        ICommandHandler<LogoutCommand> handler,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        string? refreshToken = httpContext.Request.Cookies[RefreshTokenCookieName];
        ApplicationResult result = await handler.Handle(new LogoutCommand(refreshToken), cancellationToken);

        ClearRefreshTokenCookie(httpContext.Response);

        if (result.IsFailure)
        {
            return HttpResults.Problem(result.Error!, httpContext);
        }

        return Results.NoContent();
    }

    private static async Task<IResult> MeAsync(
        IQueryHandler<GetCurrentUserQuery, AuthenticatedUserDto> handler,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        AuthenticatedUserApplicationResult result = await handler.Handle(
            new GetCurrentUserQuery(),
            cancellationToken);

        if (result.IsFailure || result.Value is null)
        {
            return HttpResults.Problem(result.Error!, httpContext);
        }

        return Results.Ok(new UserSummaryResponse(
            result.Value.Id,
            result.Value.Name,
            result.Value.Email,
            result.Value.Company));
    }

    private static LoginResponse ToLoginResponse(LoginResult result) =>
        new(
            result.AccessToken,
            result.AccessTokenExpiresAtUtc,
            new UserSummaryResponse(
                result.User.Id,
                result.User.Name,
                result.User.Email,
                result.User.Company));

    private static void AppendRefreshTokenCookie(HttpResponse response, LoginResult result)
    {
        response.Cookies.Append(
            RefreshTokenCookieName,
            result.RefreshToken,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Path = "/",
                Expires = result.RefreshTokenExpiresAtUtc
            });
    }

    private static void ClearRefreshTokenCookie(HttpResponse response)
    {
        response.Cookies.Delete(
            RefreshTokenCookieName,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Path = "/"
            });
    }
}
