# MultiTech.Platform — User Registration & Login REST API Implementation Guide

> Use this file as the implementation prompt/specification for the AI coding agent.
>
> Scope: backend only. Do not modify the existing React application yet.

## 1. Objective

Implement a production-oriented **User Registration and Login feature using REST API** in the existing `MultiTech.Platform` backend.

The registration form in the existing React application contains exactly these fields:

```text
name
email
company
password
```

The backend must support:

```text
POST /api/v1/auth/register
POST /api/v1/auth/login
POST /api/v1/auth/refresh
POST /api/v1/auth/logout
GET  /api/v1/auth/me
```

The implementation must follow the existing `MultiTech.Platform` architecture and demonstrate strong senior-level .NET practices.

Do not implement frontend integration in this task.

## 2. Important First Step for the AI Agent

Before changing any code:

1. Read the existing solution.
2. Read the existing backend architecture documentation.
3. Inspect all current projects, folders, abstractions, conventions, and package references.
4. Reuse existing Result pattern, command/query abstractions, dependency injection patterns, exception handling, EF Core setup, validation patterns, logging, configuration options, date/time abstraction, and current-user abstraction if present.
5. Do not create duplicate abstractions.
6. Do not rename existing architectural projects unless required.
7. Keep all current architecture rules intact.
8. Run the existing tests before implementation so regressions can be identified.

Do not redesign the complete application. This task adds authentication into the architecture already created.

## 3. Architectural Requirements

Continue to follow:

```text
API / Presentation
        ↓
Application
        ↓
Domain
        ↓
Infrastructure implementations
```

Dependency direction:

```text
Api
 ├── Application
 ├── Contracts
 └── Infrastructure registration

Application
 ├── Domain
 └── Contracts

Infrastructure
 ├── Application
 └── Domain

Domain
 └── no infrastructure dependencies
```

Authentication logic must not live directly inside REST endpoints.

Expected request flow:

```text
REST Endpoint
     ↓
Request Contract
     ↓
Command
     ↓
Validation
     ↓
Command Handler
     ↓
Domain / Application abstractions
     ↓
Infrastructure
     ↓
SQL Server
```

## 4. Registration Fields

Public request:

```json
{
  "name": "John Smith",
  "email": "john.smith@example.com",
  "company": "Acme Manufacturing",
  "password": "StrongPassword123!"
}
```

Required fields:

```text
Name
Email
Company
Password
```

Do not add frontend-required fields such as `firstName`, `lastName`, `username`, `confirmPassword`, `phone`, `address`, or `role`.

If React uses `confirmPassword`, validate it in the UI but never persist it.

## 5. Company Field Design

Do not introduce full multi-tenancy merely because registration contains `company`.

If an Organization/Company aggregate already exists, reuse it and link the user according to the existing model.

If no such aggregate exists, persist `CompanyName` on the user/profile for now.

Do not add tenant engines, workspaces, subscription systems, billing, or organization authorization as part of this task.

## 6. User Domain Model

Reuse the existing `User` entity if present. Otherwise create one under Domain.

Suggested shape, adapted to existing base/auditing conventions:

```csharp
public sealed class User : Entity
{
    private User() { }

    public Guid Id { get; private set; }
    public string Name { get; private set; } = null!;
    public string Email { get; private set; } = null!;
    public string CompanyName { get; private set; } = null!;
    public string PasswordHash { get; private set; } = null!;
    public bool IsActive { get; private set; }
}
```

Important rules:

- no public setters unless existing conventions already require them
- protect invariants
- normalize email consistently
- never expose `PasswordHash`
- never expose refresh-token hashes
- reuse auditing base classes/interceptors if already implemented

## 7. Password Hashing

Never store passwords as plain text.

Never implement custom password cryptography.

Use ASP.NET Core's password hashing abstraction:

```text
IPasswordHasher<User>
```

or wrap it behind an application abstraction:

```csharp
public interface IPasswordHasher
{
    string Hash(User user, string password);

    bool Verify(
        User user,
        string passwordHash,
        string providedPassword);
}
```

Infrastructure should use `Microsoft.AspNetCore.Identity.PasswordHasher<User>`.

Do not use MD5, SHA1, raw SHA256 of passwords, Base64, reversible encryption, or custom salt logic.

## 8. Password Validation

Backend requirements:

```text
Required
Minimum 8 characters
Maximum 128 characters
```

Optional basic composition requirements may be added only if consistent with the project requirements.

Backend validation remains required even if React performs validation.

## 9. Email Rules

Email must be:

```text
required
trimmed
valid format
normalized consistently
unique
```

Use consistent normalization, for example:

```csharp
var normalizedEmail = email.Trim().ToLowerInvariant();
```

Create a unique database index on the normalized email field. Do not depend only on an application-level duplicate check because concurrent requests can race.

## 10. Name and Company Validation

Name:

```text
Required
Trim whitespace
2–150 characters
```

Do not split the value into first/last name. Preserve international names and avoid restrictive regexes.

Company:

```text
Required
Trim whitespace
2–200 characters
```

## 11. Registration Contract

Create a public request contract:

```csharp
public sealed record RegisterRequest(
    string Name,
    string Email,
    string Company,
    string Password);
```

Response:

```csharp
public sealed record RegisterResponse(
    Guid Id,
    string Name,
    string Email,
    string Company);
```

Never return `Password`, `PasswordHash`, refresh-token hashes, or internal persistence fields.

## 12. Registration Command

Create:

```csharp
public sealed record RegisterUserCommand(
    string Name,
    string Email,
    string Company,
    string Password)
    : ICommand<RegisterUserResult>;
```

Suggested result:

```csharp
public sealed record RegisterUserResult(
    Guid Id,
    string Name,
    string Email,
    string Company);
```

The command must be transport-independent and must not reference `HttpContext`, REST endpoint classes, or GraphQL.

## 13. Registration Validator

Use FluentValidation or the validation mechanism already established in the solution.

```csharp
public sealed class RegisterUserCommandValidator
    : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MinimumLength(2)
            .MaximumLength(150);

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(320);

        RuleFor(x => x.Company)
            .NotEmpty()
            .MinimumLength(2)
            .MaximumLength(200);

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(8)
            .MaximumLength(128);
    }
}
```

Reuse existing validator extensions where available.

## 14. Registration Handler

Required flow:

```text
Normalize input
     ↓
Check existing email
     ↓
Create user
     ↓
Hash password
     ↓
Persist user
     ↓
Save transaction
     ↓
Return safe result
```

Duplicate email error:

```text
Code: Auth.EmailAlreadyExists
HTTP: 409 Conflict
```

Do not expose SQL or EF exceptions. Still translate a database unique constraint failure into the same `409` because concurrent registrations may both pass the initial existence check.

## 15. Register REST Endpoint

Create:

```text
POST /api/v1/auth/register
```

Endpoint responsibilities only:

```text
Read RegisterRequest
Map request → RegisterUserCommand
Dispatch command
Map Result → HTTP response
```

No business logic in the endpoint.

Success:

```http
HTTP/1.1 201 Created
```

```json
{
  "id": "guid",
  "name": "John Smith",
  "email": "john.smith@example.com",
  "company": "Acme Manufacturing"
}
```

Only use a `Location` header if a corresponding user resource endpoint really exists.

## 16. Login Request and Response

Login request:

```csharp
public sealed record LoginRequest(
    string Email,
    string Password);
```

Suggested response:

```csharp
public sealed record LoginResponse(
    string AccessToken,
    DateTimeOffset AccessTokenExpiresAtUtc,
    UserSummaryResponse User);
```

```csharp
public sealed record UserSummaryResponse(
    Guid Id,
    string Name,
    string Email,
    string Company);
```

## 17. Login Command

Create:

```csharp
public sealed record LoginCommand(
    string Email,
    string Password)
    : ICommand<LoginResult>;
```

Suggested internal result:

```csharp
public sealed record LoginResult(
    string AccessToken,
    DateTimeOffset AccessTokenExpiresAtUtc,
    string RefreshToken,
    DateTimeOffset RefreshTokenExpiresAtUtc,
    AuthenticatedUserDto User);
```

Transport/cookie decisions remain outside the Application layer.

## 18. Login Validation

Validate:

```text
Email required
Email valid
Password required
Password <= 128 characters
```

Do not distinguish between "email not found" and "wrong password" in public responses.

Use one generic error:

```text
Invalid email or password.
```

## 19. Login Handler Flow

Implement:

```text
Normalize email
      ↓
Find user
      ↓
Missing user → InvalidCredentials
      ↓
Inactive user → reject
      ↓
Verify password hash
      ↓
Invalid password → InvalidCredentials
      ↓
Generate JWT access token
      ↓
Generate cryptographically secure refresh token
      ↓
Hash refresh token
      ↓
Persist refresh-token record
      ↓
Return authentication result
```

Never log passwords or tokens.

## 20. Invalid Credentials Error

Use one application error, for example:

```csharp
public static readonly Error InvalidCredentials =
    Error.Unauthorized(
        "Auth.InvalidCredentials",
        "Invalid email or password.");
```

Map to:

```http
401 Unauthorized
```

## 21. JWT Access Token

Create/reuse an abstraction such as:

```csharp
public interface IAccessTokenProvider
{
    AccessTokenResult Create(User user);
}
```

Suggested result:

```csharp
public sealed record AccessTokenResult(
    string Token,
    DateTimeOffset ExpiresAtUtc);
```

JWT should contain only necessary claims, such as:

```text
sub   → UserId
email → normalized email
name  → display name
jti   → token identifier
```

Roles/permissions may be included if the authorization architecture already uses claims.

Never include password hashes, refresh tokens, signing keys, or sensitive internal data.

## 22. JWT Options

Create strongly typed options if not already present:

```csharp
public sealed class JwtOptions
{
    public const string SectionName = "Jwt";

    public required string Issuer { get; init; }
    public required string Audience { get; init; }
    public required string SigningKey { get; init; }
    public int AccessTokenLifetimeMinutes { get; init; } = 15;
    public int RefreshTokenLifetimeDays { get; init; } = 7;
}
```

Validate options on startup and fail fast for missing required values.

Do not commit real signing keys.

Use:

```text
Local development → user secrets / ignored environment variables
Docker → environment variables / ignored .env
Production → secret store such as Azure Key Vault
```

## 23. JWT Validation

Configure JWT bearer authentication to validate:

```text
issuer
audience
signature
lifetime
```

Do not disable lifetime validation.

Use HTTPS in production and a deliberate small clock-skew policy.

## 24. Refresh Token Entity

Create/reuse a refresh-token entity.

Suggested model:

```csharp
public sealed class RefreshToken : Entity
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public string TokenHash { get; private set; } = null!;
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public DateTimeOffset ExpiresAtUtc { get; private set; }
    public DateTimeOffset? RevokedAtUtc { get; private set; }
    public Guid? ReplacedByTokenId { get; private set; }

    public bool IsRevoked => RevokedAtUtc.HasValue;

    public bool IsExpired(DateTimeOffset utcNow) =>
        utcNow >= ExpiresAtUtc;
}
```

Adapt to existing domain/auditing conventions.

## 25. Refresh Token Generation

Refresh tokens must be cryptographically secure, random, high-entropy, and unpredictable.

Use `RandomNumberGenerator` or an equivalent cryptographically secure generator.

Do not use `Guid.NewGuid()`, `Random`, timestamps, or deterministic combinations as session secrets.

Persist only a hash of the refresh token.

## 26. Token Hashing

Create/reuse:

```csharp
public interface ITokenHasher
{
    string Hash(string token);
}
```

For already-random high-entropy refresh tokens, a cryptographic hash such as SHA-256 is suitable for token lookup/storage.

Important: this does **not** mean SHA-256 should be used for user passwords.

## 27. Refresh Token Persistence

Use table/schema consistent with the existing project, preferably:

```text
auth.RefreshTokens
```

Recommended fields:

```text
Id
UserId
TokenHash
CreatedAtUtc
ExpiresAtUtc
RevokedAtUtc
ReplacedByTokenId
```

Indexes:

```text
TokenHash UNIQUE
UserId
ExpiresAtUtc
```

Relationship:

```text
User 1 ---- many RefreshTokens
```

## 28. Browser Refresh Token Strategy

Design the API so the React app can later use a secure refresh-token cookie.

Preferred browser strategy:

```text
Access token  → short-lived; frontend auth layer/memory
Refresh token → HttpOnly cookie
```

Production cookie should be configured centrally with appropriate:

```text
HttpOnly
Secure
SameSite
Path
Expiration
```

The exact `SameSite` value must depend on whether frontend/backend are same-site or cross-site.

Do not hard-code an unsafe cross-site policy without deployment context.

If an HttpOnly cookie is used, do not also send the refresh token in JSON unless explicitly necessary.

## 29. Login REST Endpoint

Create:

```text
POST /api/v1/auth/login
```

Flow:

```text
LoginRequest
   ↓
LoginCommand
   ↓
Handler
   ↓
LoginResult
   ↓
Set refresh-token cookie
   ↓
Return access token + safe user summary
```

Example response:

```json
{
  "accessToken": "<jwt>",
  "accessTokenExpiresAtUtc": "2026-09-22T04:15:00Z",
  "user": {
    "id": "guid",
    "name": "John Smith",
    "email": "john.smith@example.com",
    "company": "Acme Manufacturing"
  }
}
```

## 30. Refresh Endpoint

Create:

```text
POST /api/v1/auth/refresh
```

Flow:

```text
Read refresh token from cookie
        ↓
Hash supplied token
        ↓
Find stored record
        ↓
Validate exists / active / unexpired / user active
        ↓
Revoke old token
        ↓
Generate replacement refresh token
        ↓
Persist replacement hash
        ↓
Generate new JWT
        ↓
Set replacement cookie
        ↓
Return new access token
```

Use refresh-token rotation. Do not reuse the same refresh token indefinitely.

## 31. Refresh Token Reuse

If a token already revoked/replaced is presented again, reject it.

Where practical, implement basic reuse detection and optionally revoke the active token family/session.

Keep this understandable. Do not build an OAuth authorization server.

## 32. Logout Endpoint

Create:

```text
POST /api/v1/auth/logout
```

Requirements:

```text
read refresh token
find stored active token if present
revoke token
clear refresh cookie
return success
```

Logout should be idempotent where practical.

Preferred success response:

```http
204 No Content
```

Allow refresh-token-based logout even if the short-lived access token has expired, if consistent with the selected endpoint design.

## 33. Current User Endpoint

Create:

```text
GET /api/v1/auth/me
```

Requires bearer authentication.

Return:

```json
{
  "id": "guid",
  "name": "John Smith",
  "email": "john.smith@example.com",
  "company": "Acme Manufacturing"
}
```

Use the current-user abstraction. Do not manually parse JWT claims in multiple handlers.

## 34. Current User Abstraction

Reuse/create:

```csharp
public interface ICurrentUser
{
    bool IsAuthenticated { get; }
    Guid? UserId { get; }
    string? Email { get; }
}
```

Implementation may use `IHttpContextAccessor`, but Application and Domain must not depend on `HttpContext`.

## 35. Auth Endpoint Group

Create a cohesive endpoint group, adapted to project conventions:

```csharp
public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(
        this IEndpointRouteBuilder app)
    {
        var group = app
            .MapGroup("/api/v1/auth")
            .WithTags("Authentication");

        group.MapPost("/register", RegisterAsync);
        group.MapPost("/login", LoginAsync);
        group.MapPost("/refresh", RefreshAsync);
        group.MapPost("/logout", LogoutAsync);
        group.MapGet("/me", MeAsync)
            .RequireAuthorization();

        return app;
    }
}
```

Do not create duplicate routes such as `/api/login` and `/api/users/login`.

## 36. Error Mapping

Use the existing Result-to-HTTP mapping.

Expected mappings:

```text
Validation                → 400
Invalid credentials       → 401
Authentication required   → 401
Forbidden                 → 403
Authenticated resource missing → 404
Email already exists      → 409
Unexpected error          → 500
```

Login must never return `404 Email not found`.

## 37. Problem Details

REST errors should follow the project's Problem Details convention.

Example duplicate email:

```json
{
  "title": "Conflict",
  "status": 409,
  "code": "Auth.EmailAlreadyExists",
  "detail": "An account with this email already exists.",
  "traceId": "..."
}
```

Never expose raw exceptions, stack traces, SQL statements, or connection strings.

## 38. EF Core User Configuration

Adapt to the existing persistence model:

```csharp
public sealed class UserConfiguration
    : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users", "auth");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(x => x.Email)
            .HasMaxLength(320)
            .IsRequired();

        builder.HasIndex(x => x.Email)
            .IsUnique();

        builder.Property(x => x.CompanyName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.PasswordHash)
            .IsRequired();

        builder.Property(x => x.IsActive)
            .IsRequired();
    }
}
```

If the model keeps original and normalized email separately, make the unique index target `NormalizedEmail` instead.

## 39. Migration

Create an EF Core migration for authentication persistence changes.

Suggested migration name:

```text
AddUserAuthentication
```

Apply it against the Docker SQL Server instance and verify it works.

Do not manually rewrite generated migration code unless necessary and understood.

## 40. Persistence Abstractions

Do not add a generic repository solely for this feature.

If the current architecture uses repositories, create a focused abstraction such as:

```csharp
public interface IUserRepository
{
    Task<User?> GetByEmailAsync(
        string normalizedEmail,
        CancellationToken cancellationToken);

    Task<User?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken);

    Task<bool> ExistsByEmailAsync(
        string normalizedEmail,
        CancellationToken cancellationToken);

    void Add(User user);
}
```

Use a focused refresh-token repository only if consistent with current persistence conventions.

## 41. Unit of Work / Transactions

Reuse an existing `IUnitOfWork` if the solution already has one.

Do not add it just to claim a pattern; EF Core `DbContext` already provides unit-of-work behavior.

Registration must persist consistently.

Refresh rotation must make:

```text
revoke old token
+
create replacement token
```

atomic.

Do not leave the old token revoked without a replacement because of unrelated `SaveChanges` calls.

## 42. Logging

Log meaningful security events with structured logging, for example:

```text
registration succeeded
login succeeded
login failed
refresh rotation succeeded
refresh-token reuse detected
logout completed
```

Never log:

```text
password
password hash
access token
refresh token
refresh token hash
signing key
```

Prefer identifiers over sensitive values:

```csharp
logger.LogInformation(
    "Authentication succeeded for UserId {UserId}",
    user.Id);
```

## 43. Rate Limiting

Apply stricter ASP.NET Core rate-limiting policies to:

```text
POST /api/v1/auth/login
POST /api/v1/auth/register
POST /api/v1/auth/refresh
```

Use reasonable values appropriate for a demo/interview system.

Do not add aggressive account locking in this initial feature unless already designed.

## 44. CORS and Cookies

Do not use a wildcard production CORS policy.

When React integration occurs, configure the actual allowed frontend origin.

If cookies are used across origins, allow credentials only for explicitly configured origins.

Never combine credentialed requests with a wildcard origin.

## 45. CSRF Consideration

Because refresh tokens may be cookies, consider CSRF protection for refresh/logout based on deployment topology using appropriate SameSite settings, Origin checks, or a CSRF strategy.

Do not blindly use `SameSite=None` without understanding the deployment topology.

## 46. OpenAPI / Swagger

Expose/document in Development:

```text
POST /api/v1/auth/register
POST /api/v1/auth/login
POST /api/v1/auth/refresh
POST /api/v1/auth/logout
GET  /api/v1/auth/me
```

Configure Bearer support so `/me` can be tested through Swagger.

Never expose secrets in API documentation.

## 47. Dependency Injection

Register auth infrastructure through existing DI extensions.

Conceptual example only:

```csharp
services.AddScoped<IPasswordHasher, PasswordHasher>();
services.AddScoped<IAccessTokenProvider, JwtAccessTokenProvider>();
services.AddSingleton<IRefreshTokenGenerator, RefreshTokenGenerator>();
services.AddSingleton<ITokenHasher, TokenHasher>();
services.AddScoped<ICurrentUser, CurrentUser>();
```

Choose lifetimes intentionally and adapt to actual implementations.

Request-dependent services must not be singletons.

Also configure `IHttpContextAccessor`, JWT bearer authentication, authorization, and options validation where appropriate.

## 48. Middleware Ordering

Respect existing middleware ordering.

Authentication/authorization must run before protected endpoint execution:

```csharp
app.UseAuthentication();
app.UseAuthorization();
```

Do not randomly reorder exception handling, CORS, request logging, or rate-limiting middleware.

## 49. Registration Example

Request:

```http
POST /api/v1/auth/register
Content-Type: application/json
```

```json
{
  "name": "John Smith",
  "email": "john.smith@example.com",
  "company": "Acme Manufacturing",
  "password": "StrongPassword123!"
}
```

Success:

```http
201 Created
```

```json
{
  "id": "4ec28c8a-d587-420e-a04f-5d6680d6bc51",
  "name": "John Smith",
  "email": "john.smith@example.com",
  "company": "Acme Manufacturing"
}
```

Conflict:

```http
409 Conflict
```

```json
{
  "title": "Conflict",
  "status": 409,
  "code": "Auth.EmailAlreadyExists",
  "detail": "An account with this email already exists."
}
```

## 50. Login Example

Request:

```http
POST /api/v1/auth/login
Content-Type: application/json
```

```json
{
  "email": "john.smith@example.com",
  "password": "StrongPassword123!"
}
```

Success:

```http
200 OK
```

```json
{
  "accessToken": "<jwt>",
  "accessTokenExpiresAtUtc": "2026-09-22T04:15:00Z",
  "user": {
    "id": "4ec28c8a-d587-420e-a04f-5d6680d6bc51",
    "name": "John Smith",
    "email": "john.smith@example.com",
    "company": "Acme Manufacturing"
  }
}
```

Failure:

```http
401 Unauthorized
```

```json
{
  "title": "Unauthorized",
  "status": 401,
  "code": "Auth.InvalidCredentials",
  "detail": "Invalid email or password."
}
```

## 51. `/me` Example

Request:

```http
GET /api/v1/auth/me
Authorization: Bearer <access-token>
```

Response:

```json
{
  "id": "4ec28c8a-d587-420e-a04f-5d6680d6bc51",
  "name": "John Smith",
  "email": "john.smith@example.com",
  "company": "Acme Manufacturing"
}
```

## 52. Concurrency

The database unique email constraint is the final protection against concurrent duplicate registration.

Example race:

```text
Request A checks email → not found
Request B checks email → not found
A inserts successfully
B inserts → unique constraint failure
```

Translate B to:

```text
409 Auth.EmailAlreadyExists
```

Never return `500` for this expected race.

## 53. Performance Rules

Do not add Redis or caching for authentication.

Ensure the normalized email lookup is indexed.

Use async EF Core APIs and pass cancellation tokens end-to-end.

## 54. Unit Tests — Registration

Add tests for:

```text
valid registration succeeds
email is normalized
name is trimmed
company is trimmed
password is hashed
raw password is not stored
duplicate email returns conflict
invalid email fails validation
empty name fails validation
empty company fails validation
short password fails validation
cancellation token is propagated
```

Do not test framework internals.

## 55. Unit Tests — Login

Add tests for:

```text
valid credentials succeed
missing user returns InvalidCredentials
invalid password returns InvalidCredentials
inactive user cannot login
password verifier is used
access-token provider is used
refresh token is persisted
raw refresh token is not persisted
```

Use the existing clock abstraction instead of relying directly on wall-clock time.

## 56. SQL Server Integration Tests

Use SQL Server Testcontainers, not EF Core InMemory, for persistence integration tests.

Test:

```text
Users mapping
unique normalized email constraint
RefreshTokens mapping
unique refresh-token hash constraint
migrations apply successfully
registration persistence
password hash stored instead of raw password
refresh-token rotation persistence
```

## 57. REST Functional Tests

Add API-level tests for:

### Registration success

```text
POST /api/v1/auth/register
→ 201
→ safe response
→ persisted user
```

### Duplicate registration

```text
→ 409
→ Auth.EmailAlreadyExists
```

### Login success

```text
→ 200
→ access token returned
→ safe user response
→ refresh session created
```

### Login failure

```text
→ 401
→ generic invalid credentials
```

### `/me` without token

```text
→ 401
```

### `/me` with token

```text
→ 200
→ correct user
```

### Refresh

```text
→ new access token
→ replacement refresh token
→ old token revoked
```

### Logout

```text
→ token revoked
→ cookie cleared
→ 204
```

## 58. Security Tests

Add tests for:

```text
password never returned
password hash never returned
invalid JWT rejected
expired JWT rejected
tampered JWT rejected
revoked refresh token rejected
expired refresh token rejected
reused refresh token rejected if reuse detection is implemented
case-insensitive duplicate email rejected
```

For example `John@example.com` and `john@example.com` must represent the same account when lowercase normalization is used.

## 59. Architecture Tests

Extend existing architecture tests where useful:

```text
Auth endpoints do not use EF DbContext directly
Application auth handlers do not depend on HttpContext
Domain User does not reference ASP.NET Core
Domain User does not reference EF Core
Password/JWT implementations live in Infrastructure
```

Avoid brittle tests based only on folder names.

## 60. Suggested File Organization

Adapt to current conventions rather than forcing duplicates.

```text
MultiTech.Platform.Contracts/
└── Authentication/
    ├── RegisterRequest.cs
    ├── RegisterResponse.cs
    ├── LoginRequest.cs
    ├── LoginResponse.cs
    └── UserSummaryResponse.cs
```

```text
MultiTech.Platform.Application/
└── Features/
    └── Authentication/
        ├── Register/
        │   ├── RegisterUserCommand.cs
        │   ├── RegisterUserCommandHandler.cs
        │   ├── RegisterUserCommandValidator.cs
        │   └── RegisterUserResult.cs
        ├── Login/
        │   ├── LoginCommand.cs
        │   ├── LoginCommandHandler.cs
        │   ├── LoginCommandValidator.cs
        │   └── LoginResult.cs
        ├── Refresh/
        ├── Logout/
        └── GetCurrentUser/
```

```text
MultiTech.Platform.Domain/
└── Users/
    ├── User.cs
    ├── RefreshToken.cs
    └── UserErrors.cs
```

```text
MultiTech.Platform.Infrastructure/
├── Authentication/
│   ├── PasswordHasher.cs
│   ├── JwtAccessTokenProvider.cs
│   ├── RefreshTokenGenerator.cs
│   ├── TokenHasher.cs
│   └── CurrentUser.cs
└── Persistence/
    └── Configurations/
        ├── UserConfiguration.cs
        └── RefreshTokenConfiguration.cs
```

```text
MultiTech.Platform.Api/
└── Endpoints/
    └── Auth/
        └── AuthEndpoints.cs
```

## 61. Do Not Implement Yet

Do not add the following in this task:

```text
Forgot password
Reset password
Email verification
MFA / 2FA
Social login
Google login
Microsoft login
OAuth authorization server
OpenID Connect provider
Account deletion
Profile editing
Organization management
Subscriptions / billing
Full multi-tenancy
React integration
GraphQL authentication
Redis
Service Bus
Hangfire
Kafka
```

Keep the feature focused.

## 62. Definition of Done

### Registration

- [ ] `POST /api/v1/auth/register` works.
- [ ] accepts `name`, `email`, `company`, `password`.
- [ ] backend validation works.
- [ ] email normalization works.
- [ ] duplicate email returns `409`.
- [ ] unique DB index exists.
- [ ] password is securely hashed.
- [ ] raw password is never persisted.
- [ ] safe user response is returned.

### Login

- [ ] `POST /api/v1/auth/login` works.
- [ ] generic invalid-credential response is used.
- [ ] framework password verification works.
- [ ] JWT access token is issued.
- [ ] JWT issuer/audience/signature/lifetime validation is configured.
- [ ] refresh token is generated securely.
- [ ] refresh token is stored only as a hash.

### Session

- [ ] `POST /api/v1/auth/refresh` works.
- [ ] refresh-token rotation works.
- [ ] old token is revoked.
- [ ] expired refresh token is rejected.
- [ ] revoked refresh token is rejected.
- [ ] `POST /api/v1/auth/logout` works.
- [ ] refresh cookie/token is cleared/revoked.

### Current User

- [ ] `GET /api/v1/auth/me` requires authentication.
- [ ] valid JWT returns the correct user.
- [ ] invalid JWT returns `401`.

### Architecture

- [ ] endpoints are thin.
- [ ] handlers contain application use-case flow.
- [ ] password/JWT implementation is in Infrastructure.
- [ ] Domain does not reference ASP.NET Core or EF Core.
- [ ] no duplicate abstractions were created.

### Security

- [ ] passwords are never logged.
- [ ] tokens are never logged.
- [ ] passwords/hashes are never returned.
- [ ] auth rate limiting exists.
- [ ] raw exception details are not exposed.
- [ ] signing key is outside source control.

### Tests

- [ ] unit tests pass.
- [ ] SQL Server Testcontainers integration tests pass.
- [ ] auth functional tests pass.
- [ ] architecture tests pass.
- [ ] `dotnet test` passes.

### Build

- [ ] `dotnet restore` passes.
- [ ] `dotnet build` passes with the solution's existing warning policy.
- [ ] migration applies successfully to Docker SQL Server.

## 63. AI Agent Execution Order

### Step 1 — Inspect Existing Architecture

Read the solution, architecture MD, `Program.cs`, DI extensions, DbContext, Result pattern, command/query abstractions, exception handling, Docker setup, and tests.

Do not create duplicate patterns.

### Step 2 — Domain / Persistence

Implement/adapt:

```text
User
RefreshToken
EF configurations
unique normalized-email index
migration
```

Verify migration on SQL Server.

### Step 3 — Authentication Infrastructure

Implement/reuse:

```text
IPasswordHasher
IAccessTokenProvider
IRefreshTokenGenerator
ITokenHasher
ICurrentUser
```

Keep implementations in Infrastructure.

### Step 4 — Registration

Create contracts, command, validator, handler, endpoint, database behavior, tests.

Run build/tests.

### Step 5 — Login

Create request/response, command, validator, handler, JWT provider, refresh token generation/persistence, endpoint, tests.

Run build/tests.

### Step 6 — JWT Middleware

Configure:

```text
JwtOptions
JwtBearer
authentication
authorization
Swagger bearer support
```

Test valid, invalid, tampered, and expired tokens.

### Step 7 — Refresh

Create refresh command/handler/endpoint, rotation transaction, cookie handling, tests.

### Step 8 — Logout

Create logout command/handler/endpoint, revocation, cookie clearing, tests.

### Step 9 — Current User

Create query/handler and `GET /api/v1/auth/me`, protect it, add tests.

### Step 10 — Hardening

Review/add:

```text
rate limiting
Problem Details
safe logging
CORS compatibility
cookie configuration
options validation
```

### Step 11 — Final Verification

Run:

```bash
dotnet restore
dotnet build
dotnet test
docker compose up
```

Then manually test registration, login, `/me`, refresh, and logout using Swagger/Postman.

## 64. Final Agent Instruction

Implement the complete authentication feature described in this document in the existing `MultiTech.Platform` backend.

Do not only output sample code.

Rules:

1. Follow the existing architecture.
2. Reuse existing abstractions.
3. Do not touch the React application.
4. Do not add unnecessary infrastructure.
5. Authentication remains REST-based, not GraphQL-based.
6. Use SQL Server through the existing EF Core infrastructure.
7. Use framework password hashing.
8. Use JWT access tokens.
9. Use secure refresh-token rotation.
10. Keep endpoints thin.
11. Keep transport concerns out of Application and Domain.
12. Add migrations.
13. Add unit tests.
14. Add SQL Server Testcontainers integration tests.
15. Add REST functional tests.
16. Add/update architecture tests.
17. Pass cancellation tokens.
18. Use structured safe logging.
19. Run and fix `dotnet restore`, `dotnet build`, and `dotnet test`.
20. Do not leave TODO placeholders for core auth behavior.
21. Do not finish until registration, login, refresh, logout, and `/me` work end-to-end.

When finished, provide a concise summary containing:

```text
Files created
Files modified
Database migration created
REST endpoints implemented
Security decisions
Tests added
Commands used to verify
Any remaining non-blocking future work
```

## 65. Security Principles to Follow

- Use ASP.NET Core's password hashing facilities instead of custom password cryptography.
- Never store passwords in plain text.
- Treat access and refresh tokens as credentials.
- Avoid account enumeration during login.
- Use cryptographically secure random refresh tokens.
- Prefer an HttpOnly cookie for browser refresh tokens when integrating React.
- Use short-lived access tokens and refresh-token rotation.
- Apply explicit rate limiting to public authentication endpoints.
- Never leak passwords, password hashes, tokens, signing keys, or raw exceptions into logs/responses.

The goal is a secure, testable, production-oriented implementation that is still simple enough to explain clearly in a senior .NET interview.
