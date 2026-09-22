# MultiTech.Platform — Backend Architecture & Implementation Guide

> Purpose: Build an interview-grade .NET backend that demonstrates strong senior-level engineering practices before integrating the existing React application.

---

# 1. Project Goals

`MultiTech.Platform` is a dashboard backend designed to demonstrate practical .NET engineering skills.

The backend must:

- Use a **modular monolith** architecture.
- Use **REST** for authentication and session-oriented operations.
- Use **GraphQL** for dashboard/domain queries and selected updates.
- Use **Microsoft SQL Server** as the primary relational database.
- Use **Entity Framework Core** for persistence.
- Run locally using **Docker / Docker Compose**.
- Use **JWT access tokens** with refresh-token rotation.
- Support **role- and permission-based authorization**.
- Use **CQRS-style commands and queries**.
- Use **Vertical Slice Architecture** inside modules/features.
- Apply **Clean Architecture dependency rules**.
- Apply **DDD selectively**, where it creates value.
- Use reusable abstractions without creating unnecessary generic frameworks.
- Provide production-oriented:
  - validation
  - error handling
  - logging
  - health checks
  - observability
  - security
  - audit information
  - testing
- Be easy to integrate later with the existing React application.
- Be easy to explain in a senior software-engineering interview.

Do not build microservices for this project.

The architecture must remain microservice-ready by enforcing clear module boundaries.

---

# 2. High-Level Architecture

```text
                        Existing React Application
                                  │
                                  │ later
                 ┌────────────────┴────────────────┐
                 │                                 │
               REST                             GraphQL
                 │                                 │
        Authentication API                Queries / Mutations
                 │                                 │
                 └──────────────┬──────────────────┘
                                │
                         Presentation Layer
                                │
                         Application Layer
                     Commands / Queries / DTOs
                                │
                           Domain Layer
                                │
                       Infrastructure Layer
                         │        │        │
                    SQL Server  Cache   Logging
```

Primary rule:

```text
Presentation
     ↓
Application
     ↓
Domain

Infrastructure implements interfaces owned by Application/Domain.
```

The Domain project must never reference:

- Entity Framework Core
- ASP.NET Core
- GraphQL
- REST
- SQL Server
- Docker
- Redis
- logging providers
- Azure services

---

# 3. Recommended Technology Stack

Use stable versions supported by the selected .NET SDK.

Recommended stack:

- ASP.NET Core
- .NET
- C#
- Entity Framework Core
- SQL Server
- Hot Chocolate GraphQL
- FluentValidation
- JWT Bearer authentication
- ASP.NET Core password hashing / Identity password hasher
- Serilog or built-in structured logging
- OpenTelemetry
- Swagger / OpenAPI
- xUnit
- FluentAssertions
- NSubstitute or Moq
- Testcontainers for SQL Server
- ArchUnitNET or NetArchTest
- Docker
- Docker Compose

Optional later additions:

- Redis
- Seq
- Application Insights
- Azure Key Vault
- Azure Service Bus

Do not add optional infrastructure until there is a real use case.

---

# 4. Solution Structure

Use the existing solution name:

```text
MultiTech.Platform.sln
```

Recommended structure:

```text
MultiTech.Platform/
│
├── src/
│   │
│   ├── MultiTech.Platform.Api/
│   │   ├── Endpoints/
│   │   │   └── Auth/
│   │   │
│   │   ├── GraphQL/
│   │   │   ├── Queries/
│   │   │   ├── Mutations/
│   │   │   ├── Types/
│   │   │   ├── Inputs/
│   │   │   ├── Payloads/
│   │   │   └── DataLoaders/
│   │   │
│   │   ├── Middleware/
│   │   ├── ExceptionHandling/
│   │   ├── Extensions/
│   │   ├── Health/
│   │   ├── DependencyInjection/
│   │   ├── appsettings.json
│   │   ├── appsettings.Development.json
│   │   └── Program.cs
│   │
│   ├── MultiTech.Platform.Application/
│   │   ├── Abstractions/
│   │   │   ├── Authentication/
│   │   │   ├── Authorization/
│   │   │   ├── Clock/
│   │   │   ├── Messaging/
│   │   │   ├── Persistence/
│   │   │   └── Services/
│   │   │
│   │   ├── Behaviors/
│   │   ├── Common/
│   │   │   ├── Errors/
│   │   │   ├── Results/
│   │   │   ├── Pagination/
│   │   │   └── Mapping/
│   │   │
│   │   └── Features/
│   │       ├── Authentication/
│   │       ├── Users/
│   │       ├── Dashboard/
│   │       ├── Sites/
│   │       ├── Gateways/
│   │       ├── Devices/
│   │       └── Alerts/
│   │
│   ├── MultiTech.Platform.Domain/
│   │   ├── Common/
│   │   │   ├── Entity.cs
│   │   │   ├── AggregateRoot.cs
│   │   │   ├── DomainEvent.cs
│   │   │   └── DomainException.cs
│   │   │
│   │   ├── Users/
│   │   ├── Sites/
│   │   ├── Gateways/
│   │   ├── Devices/
│   │   ├── Alerts/
│   │   └── Shared/
│   │
│   ├── MultiTech.Platform.Infrastructure/
│   │   ├── Authentication/
│   │   ├── Authorization/
│   │   ├── Persistence/
│   │   │   ├── Configurations/
│   │   │   ├── Interceptors/
│   │   │   ├── Migrations/
│   │   │   ├── Repositories/
│   │   │   └── PlatformDbContext.cs
│   │   │
│   │   ├── Services/
│   │   ├── Observability/
│   │   └── DependencyInjection/
│   │
│   └── MultiTech.Platform.Contracts/
│       ├── Authentication/
│       ├── Users/
│       ├── Dashboard/
│       ├── Devices/
│       ├── Gateways/
│       └── Alerts/
│
├── tests/
│   ├── MultiTech.Platform.UnitTests/
│   ├── MultiTech.Platform.IntegrationTests/
│   ├── MultiTech.Platform.FunctionalTests/
│   └── MultiTech.Platform.ArchitectureTests/
│
├── docker/
│   └── sql/
│
├── .env.example
├── .gitignore
├── docker-compose.yml
├── Directory.Build.props
├── Directory.Packages.props
└── README.md
```

---

# 5. Project Dependency Rules

Allowed dependencies:

```text
Api
 ├── Application
 ├── Infrastructure
 └── Contracts

Infrastructure
 ├── Application
 └── Domain

Application
 ├── Domain
 └── Contracts

Domain
 └── no project dependencies
```

Never allow:

```text
Domain → Infrastructure
Domain → Api
Application → Api
Application → Infrastructure
```

Enforce these rules with architecture tests.

---

# 6. Feature / Vertical Slice Organization

Do not create giant folders such as:

```text
Controllers/
Services/
Repositories/
Models/
```

Prefer feature slices.

Example:

```text
Features/
└── Devices/
    ├── GetDevice/
    │   ├── GetDeviceQuery.cs
    │   ├── GetDeviceQueryHandler.cs
    │   └── DeviceDto.cs
    │
    ├── GetDevices/
    │   ├── GetDevicesQuery.cs
    │   ├── GetDevicesQueryHandler.cs
    │   └── GetDevicesValidator.cs
    │
    └── UpdateDevice/
        ├── UpdateDeviceCommand.cs
        ├── UpdateDeviceCommandHandler.cs
        └── UpdateDeviceCommandValidator.cs
```

Keep code required for a use case close together.

---

# 7. CQRS Abstractions

Commands change state.

Queries read state.

Create lightweight reusable abstractions.

```csharp
public interface ICommand;

public interface ICommand<TResult>;

public interface IQuery<TResult>;
```

Handlers:

```csharp
public interface ICommandHandler<in TCommand>
    where TCommand : ICommand
{
    Task<Result> Handle(
        TCommand command,
        CancellationToken cancellationToken);
}

public interface ICommandHandler<in TCommand, TResult>
    where TCommand : ICommand<TResult>
{
    Task<Result<TResult>> Handle(
        TCommand command,
        CancellationToken cancellationToken);
}

public interface IQueryHandler<in TQuery, TResult>
    where TQuery : IQuery<TResult>
{
    Task<Result<TResult>> Handle(
        TQuery query,
        CancellationToken cancellationToken);
}
```

A mediator library may be used, but do not make the application architecture dependent on unnecessary magic.

If MediatR is used, keep the application code structured around commands/queries, not around MediatR itself.

---

# 8. Result Pattern

Expected business failures must not require exceptions.

Create:

```csharp
public sealed record Error(
    string Code,
    string Message,
    ErrorType Type);
```

```csharp
public enum ErrorType
{
    Failure,
    Validation,
    NotFound,
    Conflict,
    Unauthorized,
    Forbidden
}
```

Example:

```csharp
public sealed class Result
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public Error? Error { get; }
}
```

Also create:

```csharp
Result<T>
```

Use results for expected cases such as:

- invalid credentials
- duplicate email
- device not found
- alert already acknowledged
- forbidden action
- validation error

Use exceptions for genuinely unexpected failures.

---

# 9. Domain Model

Suggested interview-friendly domain:

```text
Organization
    ↓
Site
    ↓
Gateway
    ↓
Device
    ↓
Sensor
    ↓
Telemetry
```

Supporting concepts:

```text
User
Role
Permission
RefreshToken

Alert
DeviceEvent
```

Initial entities:

- User
- RefreshToken
- Role
- Permission
- Site
- Gateway
- Device
- Alert

Telemetry can be added after the primary architecture is working.

---

# 10. Base Domain Entity

Example:

```csharp
public abstract class Entity
{
    private readonly List<IDomainEvent> _domainEvents = [];

    public IReadOnlyCollection<IDomainEvent> DomainEvents =>
        _domainEvents.AsReadOnly();

    protected void RaiseDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}
```

Avoid public setters everywhere.

Prefer domain behavior.

Bad:

```csharp
device.Status = DeviceStatus.Offline;
```

Better:

```csharp
device.ChangeStatus(DeviceStatus.Offline);
```

Domain entities must protect their own invariants.

---

# 11. Auditing

Create:

```csharp
public interface IAuditableEntity
{
    DateTimeOffset CreatedAtUtc { get; }
    Guid? CreatedBy { get; }
    DateTimeOffset? UpdatedAtUtc { get; }
    Guid? UpdatedBy { get; }
}
```

Use an EF Core `SaveChangesInterceptor` to populate audit fields.

Do not repeat auditing logic in every command handler.

---

# 12. Date and Time

Never scatter:

```csharp
DateTime.UtcNow
```

through application code.

Use:

```csharp
public interface IDateTimeProvider
{
    DateTimeOffset UtcNow { get; }
}
```

Infrastructure implementation:

```csharp
public sealed class DateTimeProvider : IDateTimeProvider
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}
```

This improves testability.

---

# 13. SQL Server

Use SQL Server in Docker for local development.

Suggested schemas:

```text
auth.Users
auth.RefreshTokens
auth.Roles
auth.Permissions
auth.UserRoles
auth.RolePermissions

platform.Sites
platform.Gateways
platform.Devices

monitoring.Alerts
monitoring.DeviceEvents
```

Do not allow modules/features to casually query unrelated tables.

---

# 14. EF Core DbContext

Start with a single `PlatformDbContext` for this portfolio application.

Do not split into many DbContexts unless the application actually needs module-isolated persistence.

Example:

```csharp
public sealed class PlatformDbContext(
    DbContextOptions<PlatformDbContext> options)
    : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<Site> Sites => Set<Site>();
    public DbSet<Gateway> Gateways => Set<Gateway>();
    public DbSet<Device> Devices => Set<Device>();
    public DbSet<Alert> Alerts => Set<Alert>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(PlatformDbContext).Assembly);
    }
}
```

Using one context keeps the showcase understandable while the domain/module boundaries are maintained in code and database schemas.

---

# 15. EF Entity Configurations

Do not configure entities inside one giant `OnModelCreating`.

Use:

```csharp
public sealed class DeviceConfiguration
    : IEntityTypeConfiguration<Device>
{
    public void Configure(EntityTypeBuilder<Device> builder)
    {
        builder.ToTable("Devices", "platform");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.HasIndex(x => x.Name);
    }
}
```

Each entity should have an independent configuration.

---

# 16. Migrations

Keep migrations in:

```text
MultiTech.Platform.Infrastructure/Persistence/Migrations
```

Migration workflow:

```bash
dotnet ef migrations add InitialCreate \
  --project src/MultiTech.Platform.Infrastructure \
  --startup-project src/MultiTech.Platform.Api
```

Apply:

```bash
dotnet ef database update \
  --project src/MultiTech.Platform.Infrastructure \
  --startup-project src/MultiTech.Platform.Api
```

Do not automatically run destructive migrations in production.

For local development, migrations may be applied during startup only if clearly limited to Development.

---

# 17. Repository Rules

Do not create:

```csharp
IGenericRepository<T>
```

with generic CRUD methods simply to claim the repository pattern is used.

EF Core already handles much of generic data access.

Create repositories only when they model aggregate persistence behavior.

Example:

```csharp
public interface IUserRepository
{
    Task<User?> GetByEmailAsync(
        string email,
        CancellationToken cancellationToken);

    void Add(User user);
}
```

Query handlers may use a read abstraction or DbContext-based query interface directly when appropriate.

Commands should operate on aggregates.

---

# 18. REST Responsibilities

Use REST for:

```text
POST /api/v1/auth/register
POST /api/v1/auth/login
POST /api/v1/auth/refresh
POST /api/v1/auth/logout
GET  /api/v1/auth/me
```

Optional:

```text
GET /health
GET /health/live
GET /health/ready
```

Do not expose dashboard CRUD through REST unless needed.

Keep authentication REST-based to clearly demonstrate both REST and GraphQL in the project.

---

# 19. REST Endpoint Style

Prefer endpoint groups or thin Minimal API endpoints.

Example:

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
        group.MapPost("/logout", LogoutAsync)
            .RequireAuthorization();
        group.MapGet("/me", MeAsync)
            .RequireAuthorization();

        return app;
    }
}
```

Endpoints must not contain business logic.

---

# 20. Authentication

Authentication implementation must include:

- registration
- secure password hashing
- login
- JWT access token
- refresh token
- refresh token rotation
- token revocation
- logout
- current-user abstraction
- authorization policies

Never implement custom cryptography.

Use a proven password hasher.

Create:

```csharp
public interface IPasswordHasher
{
    string Hash(string password);

    bool Verify(
        string passwordHash,
        string providedPassword);
}
```

Infrastructure may wrap ASP.NET Core Identity's password hasher.

---

# 21. User Registration Flow

```text
POST /api/v1/auth/register
          ↓
RegisterRequest
          ↓
RegisterCommand
          ↓
Validation
          ↓
Check duplicate email
          ↓
Hash password
          ↓
Create User aggregate
          ↓
SaveChanges
          ↓
Return safe user DTO
```

Never return password hashes.

---

# 22. Login Flow

```text
POST /api/v1/auth/login
          ↓
LoginRequest
          ↓
LoginCommand
          ↓
Find user
          ↓
Verify password
          ↓
Create access token
          ↓
Create refresh token
          ↓
Store hashed refresh token
          ↓
Return auth response
```

Authentication response:

```json
{
  "accessToken": "<jwt>",
  "refreshToken": "<refresh-token>",
  "expiresIn": 900
}
```

For a real browser production system, review whether the refresh token should be delivered through a secure `HttpOnly`, `Secure`, `SameSite` cookie.

Do not blindly store long-lived tokens in browser local storage.

---

# 23. Refresh Tokens

Suggested persistence fields:

```text
Id
UserId
TokenHash
CreatedAtUtc
ExpiresAtUtc
RevokedAtUtc
ReplacedByTokenId
CreatedByIp
RevokedByIp
```

Store a hash of the refresh token rather than plain text.

Implement rotation:

```text
Client refresh token
      ↓
Validate token
      ↓
Revoke old refresh token
      ↓
Generate new access token
      ↓
Generate new refresh token
      ↓
Persist replacement
      ↓
Return new pair
```

If a previously rotated token is reused, treat it as suspicious and consider revoking the token family.

---

# 24. Current User Abstraction

Application code must not directly depend on `HttpContext`.

Create:

```csharp
public interface ICurrentUser
{
    bool IsAuthenticated { get; }
    Guid? UserId { get; }
    string? Email { get; }
}
```

Implement it in Infrastructure/Api using `IHttpContextAccessor`.

---

# 25. Authorization

Support:

- authenticated user
- role-based rules
- permission-based policies

Permissions example:

```csharp
public static class Permissions
{
    public static class Dashboard
    {
        public const string View = "dashboard:view";
    }

    public static class Devices
    {
        public const string View = "devices:view";
        public const string Edit = "devices:edit";
    }

    public static class Alerts
    {
        public const string View = "alerts:view";
        public const string Acknowledge = "alerts:acknowledge";
    }
}
```

Avoid code like:

```csharp
if (user.Role == "Admin")
```

throughout the application.

Prefer authorization policies.

---

# 26. Validation

Use FluentValidation.

Example:

```csharp
public sealed class RegisterCommandValidator
    : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.LastName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(320);

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(8);
    }
}
```

Validation should happen before the handler.

Create a reusable validation behavior if using a mediator pipeline.

---

# 27. Application Pipeline Behaviors

Recommended pipeline:

```text
Request
  ↓
Logging behavior
  ↓
Validation behavior
  ↓
Authorization behavior when applicable
  ↓
Handler
```

Do not make every handler repeat cross-cutting logic.

---

# 28. GraphQL Responsibilities

Use GraphQL for:

## Queries

```text
dashboard
sites
site
gateways
gateway
devices
device
alerts
alert
```

## Mutations

```text
updateDevice
updateGateway
acknowledgeAlert
```

Do not build authentication through GraphQL for this project.

Authentication remains REST-based.

---

# 29. Initial GraphQL Schema

Example:

```graphql
type Query {
  dashboard: Dashboard!
  devices(
    first: Int
    after: String
    status: DeviceStatus
  ): DeviceConnection
  device(id: UUID!): Device
  gateways(first: Int, after: String): GatewayConnection
  alerts(first: Int, after: String): AlertConnection
}
```

Example mutations:

```graphql
type Mutation {
  updateDevice(input: UpdateDeviceInput!): UpdateDevicePayload!
  acknowledgeAlert(input: AcknowledgeAlertInput!): AcknowledgeAlertPayload!
}
```

---

# 30. Dashboard GraphQL Query

Target query:

```graphql
query Dashboard {
  dashboard {
    totalSites
    totalGateways
    onlineGateways
    offlineGateways
    totalDevices
    onlineDevices
    offlineDevices
    activeAlerts
    criticalAlerts

    recentAlerts {
      id
      severity
      message
      createdAtUtc
    }

    gateways {
      id
      name
      status
      lastSeenAtUtc
    }
  }
}
```

This demonstrates why GraphQL is valuable for dashboard aggregation.

---

# 31. GraphQL Resolver Rules

Resolvers must remain thin.

Bad:

```csharp
public async Task<Device> UpdateDevice(...)
{
    var device = await dbContext.Devices.FindAsync(id);
    device.Name = input.Name;
    await dbContext.SaveChangesAsync();
    return device;
}
```

Correct idea:

```csharp
public Task<Result<DeviceDto>> UpdateDevice(
    UpdateDeviceInput input,
    ICommandDispatcher dispatcher,
    CancellationToken cancellationToken)
{
    return dispatcher.Send(
        new UpdateDeviceCommand(
            input.Id,
            input.Name,
            input.Enabled),
        cancellationToken);
}
```

GraphQL is only a transport/presentation layer.

Business rules live in Application/Domain.

---

# 32. GraphQL DataLoader

Use DataLoader for relationship-heavy GraphQL fields.

Example problem:

```text
20 gateways
   ↓
20 separate device queries
```

This is an N+1 problem.

Use a batching DataLoader so device data can be fetched efficiently.

Possible DataLoaders:

```text
DevicesByGatewayIdDataLoader
AlertsByDeviceIdDataLoader
GatewayByIdDataLoader
SiteByIdDataLoader
```

Do not add DataLoader where no N+1 risk exists.

---

# 33. GraphQL Performance and Security

Configure:

- authentication
- authorization
- pagination
- maximum page size
- query depth limits
- query complexity limits
- execution timeout
- error filtering
- DataLoader
- persisted queries later if useful
- introspection policy appropriate to environment

Never allow an unrestricted query to return the entire database.

---

# 34. Pagination

Prefer cursor pagination for GraphQL.

Example:

```graphql
query {
  devices(first: 25, after: "cursor") {
    nodes {
      id
      name
      status
    }

    pageInfo {
      hasNextPage
      endCursor
    }
  }
}
```

Apply a maximum page size.

Example:

```text
Default page size: 20
Maximum page size: 100
```

---

# 35. Filtering and Sorting

Support controlled filtering.

Example:

```graphql
query {
  devices(
    first: 20
    where: {
      status: { eq: ONLINE }
    }
    order: {
      name: ASC
    }
  ) {
    nodes {
      id
      name
    }
  }
}
```

Do not automatically expose every internal database field for filtering/sorting.

Expose intentional API contracts.

---

# 36. DTO Rules

Never expose EF entities directly through REST or GraphQL.

Use dedicated contracts/DTOs.

Reasons:

- prevents over-posting
- avoids leaking persistence structure
- provides API stability
- controls serialization
- prevents cyclic relationships
- improves security

---

# 37. Exception Handling

Implement one global exception handler.

Use ASP.NET Core `IExceptionHandler` or equivalent centralized middleware.

Map expected `Result` errors to REST status codes:

```text
Validation   → 400
Unauthorized → 401
Forbidden    → 403
NotFound     → 404
Conflict     → 409
```

Unexpected failures:

```text
500 Internal Server Error
```

REST responses should use Problem Details.

Never return:

- stack traces
- connection strings
- SQL queries
- internal class names
- secret values

GraphQL must also filter unexpected exceptions.

---

# 38. Structured Logging

Use structured logging.

Correct:

```csharp
logger.LogInformation(
    "Device {DeviceId} changed status to {Status}",
    device.Id,
    device.Status);
```

Avoid interpolation:

```csharp
logger.LogInformation(
    $"Device {device.Id} changed status to {device.Status}");
```

Never log:

- passwords
- access tokens
- refresh tokens
- connection strings
- personal secrets

---

# 39. Correlation IDs

Every request should have a correlation/trace ID.

Flow:

```text
React
  ↓
REST / GraphQL
  ↓
Trace ID
  ↓
Command / Query
  ↓
EF Core
  ↓
logs
```

Include trace ID in error responses where appropriate.

---

# 40. OpenTelemetry

Add OpenTelemetry instrumentation for:

- ASP.NET Core
- HTTP client
- EF Core
- runtime metrics

During local development, output traces/logs to a useful local target.

Later this can connect to:

- Application Insights
- OTLP collector
- Grafana stack
- other compatible observability tools

---

# 41. Health Checks

Expose:

```text
GET /health/live
GET /health/ready
```

Liveness:

```text
Is API process running?
```

Readiness:

```text
Can application serve traffic?
Can SQL Server be reached?
```

Docker health checking may use readiness.

---

# 42. Docker Compose

Recommended local services:

```yaml
services:

  sqlserver:
    image: mcr.microsoft.com/mssql/server:2022-latest
    environment:
      ACCEPT_EULA: "Y"
      MSSQL_SA_PASSWORD: "${MSSQL_SA_PASSWORD}"
    ports:
      - "1433:1433"
    volumes:
      - multitech-sql-data:/var/opt/mssql

  api:
    build:
      context: .
      dockerfile: src/MultiTech.Platform.Api/Dockerfile
    environment:
      ASPNETCORE_ENVIRONMENT: Development
      ConnectionStrings__Database: >
        Server=sqlserver;
        Database=MultiTechPlatform;
        User Id=sa;
        Password=${MSSQL_SA_PASSWORD};
        TrustServerCertificate=True
    ports:
      - "8080:8080"
    depends_on:
      - sqlserver

volumes:
  multitech-sql-data:
```

Add a proper SQL Server health check before making the API depend on database readiness.

---

# 43. Dockerfile

Use multi-stage builds.

Concept:

```dockerfile
FROM mcr.microsoft.com/dotnet/sdk AS build
WORKDIR /src

COPY . .
RUN dotnet restore
RUN dotnet publish src/MultiTech.Platform.Api \
    -c Release \
    -o /app/publish \
    --no-restore

FROM mcr.microsoft.com/dotnet/aspnet AS runtime
WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "MultiTech.Platform.Api.dll"]
```

Pin image versions appropriate to the chosen SDK instead of using uncontrolled tags in a final production build.

Run as non-root when supported by the selected runtime/container setup.

---

# 44. Environment Configuration

Use:

```text
appsettings.json
appsettings.Development.json
environment variables
user secrets
```

Never commit secrets.

`.gitignore` should include:

```text
.env
*.user
*.suo
.vs/
bin/
obj/
appsettings.Local.json
```

Provide:

```text
.env.example
```

with placeholder values only.

---

# 45. Configuration Options

Use strongly typed options.

Example:

```csharp
public sealed class JwtOptions
{
    public const string SectionName = "Jwt";

    public required string Issuer { get; init; }
    public required string Audience { get; init; }
    public required string SigningKey { get; init; }
    public int AccessTokenLifetimeMinutes { get; init; }
    public int RefreshTokenLifetimeDays { get; init; }
}
```

Register with options validation.

Fail fast on invalid configuration.

---

# 46. Dependency Injection

Group registrations by project.

API:

```csharp
builder.Services.AddPresentation();
```

Application:

```csharp
builder.Services.AddApplication();
```

Infrastructure:

```csharp
builder.Services.AddInfrastructure(
    builder.Configuration);
```

Keep `Program.cs` readable.

Target:

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration)
    .AddPresentation(builder.Configuration);

var app = builder.Build();

app.UsePlatformPipeline();
app.MapPlatformEndpoints();

app.Run();
```

Do not allow `Program.cs` to become hundreds of lines.

---

# 47. Seed Data

Add Development-only seed data.

Seed:

```text
Admin user
Demo user

2 sites

3 gateways
- 2 online
- 1 offline

8–12 devices

Several alerts
- information
- warning
- critical
```

The system should be demo-ready immediately after:

```bash
docker compose up
```

Use predictable demo credentials documented in README only for local development.

Never use demo credentials in production.

---

# 48. Security Checklist

Implement:

- HTTPS outside local container-only scenarios
- JWT validation
- token expiration
- refresh token rotation
- password hashing
- authorization policies
- input validation
- rate limiting
- CORS restrictions
- safe error responses
- GraphQL depth/complexity controls
- pagination limits
- no secrets in Git
- parameterized EF queries
- secure headers
- audit fields
- sensitive logging protection

Review OWASP API Security guidance while implementing.

---

# 49. CORS

During backend-only development, configure development origins explicitly.

Later allow the actual React application origin.

Example idea:

```text
http://localhost:5173
```

Never use an unrestricted production policy such as:

```text
AllowAnyOrigin
AllowCredentials
```

together.

Use environment-specific policy configuration.

---

# 50. Rate Limiting

Add ASP.NET Core rate limiting.

Apply stricter limits to:

```text
/api/v1/auth/login
/api/v1/auth/register
/api/v1/auth/refresh
```

Add sensible general API limits.

Do not make the demo unusable with extreme limits.

---

# 51. API Versioning

REST:

```text
/api/v1/auth/login
```

GraphQL should evolve through schema evolution rather than `/graphql/v1`, `/graphql/v2`.

When removing GraphQL fields, deprecate them first.

---

# 52. REST OpenAPI

Expose OpenAPI documentation in Development.

Document:

- auth endpoints
- request models
- response models
- Problem Details responses
- bearer authentication scheme

Use meaningful endpoint names and tags.

---

# 53. GraphQL Naming Rules

Use consistent GraphQL naming.

Examples:

```text
Query.dashboard
Query.devices
Mutation.updateDevice
Mutation.acknowledgeAlert

UpdateDeviceInput
UpdateDevicePayload
AcknowledgeAlertInput
AcknowledgeAlertPayload
```

Avoid leaking internal C# implementation names into the public schema unnecessarily.

---

# 54. Error Contracts

GraphQL business errors should be predictable.

Do not make clients parse arbitrary exception strings.

Possible payload:

```graphql
type UpdateDevicePayload {
  device: Device
  errors: [UserError!]!
}

type UserError {
  code: String!
  message: String!
}
```

REST uses Problem Details.

Keep the underlying application errors shared while transport mapping remains separate.

---

# 55. Cancellation Tokens

Every asynchronous operation that supports cancellation must pass the request cancellation token.

Example:

```csharp
await dbContext.Devices
    .AsNoTracking()
    .SingleOrDefaultAsync(
        x => x.Id == query.Id,
        cancellationToken);
```

Do not silently drop cancellation tokens between layers.

---

# 56. EF Query Best Practices

For read-only queries use:

```csharp
.AsNoTracking()
```

Project directly to DTOs.

Prefer:

```csharp
.Select(x => new DeviceDto(
    x.Id,
    x.Name,
    x.Status))
```

rather than loading full aggregates when only a read model is needed.

Avoid accidental N+1 queries.

Use indexes for:

- email
- status where appropriate
- foreign keys
- frequent dashboard filters
- unique identifiers

Measure before adding speculative indexes.

---

# 57. Optimistic Concurrency

Add a concurrency mechanism where updates may conflict.

For SQL Server, consider a row-version column.

Example:

```csharp
public byte[] RowVersion { get; private set; } = [];
```

Configure:

```csharp
builder.Property(x => x.RowVersion)
    .IsRowVersion();
```

Map concurrency conflicts to a useful result instead of exposing EF exceptions.

---

# 58. Domain Events

Use domain events for meaningful in-process reactions.

Example:

```text
Device status changed
        ↓
DeviceStatusChangedDomainEvent
        ↓
handler
        ↓
create monitoring event / alert if appropriate
```

Do not use domain events for every property change.

Keep them business-significant.

---

# 59. Outbox Pattern

The Outbox Pattern is optional for the first implementation.

Add it only if demonstrating asynchronous integration events.

If added:

```text
Transaction
  ├── Domain data
  └── OutboxMessages
           ↓
Background worker
           ↓
Integration event
```

This becomes useful later for Azure Service Bus.

Do not add Service Bus simply to make the project appear complex.

---

# 60. Caching

Do not add Redis initially unless dashboard performance creates a real reason.

First implement correct SQL queries.

If Redis is introduced later, use it for:

- expensive dashboard aggregates
- reference data
- permission lookup

Create an abstraction such as:

```csharp
public interface ICacheService
{
    Task<T?> GetAsync<T>(
        string key,
        CancellationToken cancellationToken);

    Task SetAsync<T>(
        string key,
        T value,
        TimeSpan expiration,
        CancellationToken cancellationToken);

    Task RemoveAsync(
        string key,
        CancellationToken cancellationToken);
}
```

Do not build elaborate cache abstraction before Redis is actually used.

---

# 61. Testing Strategy

Use four test projects.

## Unit Tests

Test:

- domain behavior
- command handlers
- query logic where isolated
- validators
- token-related pure logic
- Result mappings

## Integration Tests

Test:

- EF Core mappings
- repositories
- migrations
- SQL behavior
- persistence constraints

Use real SQL Server through Testcontainers.

Do not use EF Core InMemory as a replacement for SQL Server integration tests.

## Functional Tests

Start the real API and test:

```text
register
login
refresh
authorized REST endpoint
GraphQL dashboard query
GraphQL device query
GraphQL mutation
forbidden mutation
```

## Architecture Tests

Enforce dependency and naming rules.

---

# 62. Testcontainers

Integration tests must use the SQL Server Testcontainers module.

Concept:

```csharp
private readonly MsSqlContainer _sqlServer =
    new MsSqlBuilder()
        .Build();
```

Test lifecycle:

```text
Start SQL container
      ↓
Configure test DbContext
      ↓
Apply migrations
      ↓
Run test
      ↓
Destroy container
```

This demonstrates production-like persistence testing.

---

# 63. Functional Test Scenario

Create one end-to-end functional test covering:

```text
Register user
    ↓
Login
    ↓
Receive access token
    ↓
Call authenticated GraphQL query
    ↓
Read dashboard data
    ↓
Execute updateDevice mutation
    ↓
Query device again
    ↓
Verify update
```

This is a strong interview demonstration.

---

# 64. Architecture Tests

Examples:

```text
Domain must not reference Infrastructure
Domain must not reference Api

Application must not reference Infrastructure
Application must not reference Api

Infrastructure may reference Application and Domain

Handlers should be sealed

Validators should end with Validator

Commands should end with Command

Queries should end with Query
```

Keep architecture tests useful.

Do not turn stylistic preferences into hundreds of brittle tests.

---

# 65. Code Quality Rules

Use:

- nullable reference types
- implicit usings if desired consistently
- file-scoped namespaces
- sealed classes where inheritance is not intended
- records for immutable commands/queries/contracts where appropriate
- async suffix for public asynchronous service methods when appropriate
- `CancellationToken`
- guard clauses
- small methods
- small classes
- cohesive files

Avoid:

- god classes
- static service locators
- service classes with dozens of unrelated methods
- generic abstractions with no real value
- unnecessary inheritance
- magic strings
- duplicate constants

---

# 66. Constants and Enums

Do not scatter repeated strings.

Use constants for stable values:

```csharp
public static class ClaimNames
{
    public const string Permissions = "permissions";
}
```

Use enums where the set of domain states is closed and meaningful.

Example:

```csharp
public enum DeviceStatus
{
    Unknown = 0,
    Online = 1,
    Offline = 2,
    Warning = 3
}
```

Persist enums intentionally, not accidentally.

---

# 67. Reusable Code Rule

Before creating a reusable abstraction, ask:

1. Is this logic truly used by multiple features?
2. Does extracting it improve clarity?
3. Will the abstraction hide useful domain meaning?
4. Is this abstraction easier to maintain than duplication?

Do not create generic code simply to demonstrate generics.

Good generic examples:

- `Result<T>`
- paging result types
- command/query handler contracts
- reusable validation behavior

Bad generic examples:

- `GenericService<T>`
- `GenericManager<T>`
- giant `BaseController<T>`
- generic repository with every CRUD method

---

# 68. Mapping

For a showcase project, manual mapping is preferred where mappings are simple.

Example:

```csharp
.Select(x => new DeviceDto(
    x.Id,
    x.Name,
    x.Status,
    x.LastSeenAtUtc))
```

This keeps query behavior visible.

Introduce a mapping library only when mapping volume justifies it.

---

# 69. Background Work

Do not add Hangfire/Quartz until a real background scenario exists.

Possible later scenario:

```text
Mark gateways offline when heartbeat is stale.
```

If added, keep the business decision in Application/Domain and scheduler implementation in Infrastructure.

---

# 70. Initial Implementation Phases

## Phase 1 — Solution Foundation

Create:

```text
Api
Application
Domain
Infrastructure
Contracts

UnitTests
IntegrationTests
FunctionalTests
ArchitectureTests
```

Configure:

- project references
- nullable
- analyzers
- shared build properties
- central package management

Completion criteria:

```text
dotnet build
```

passes.

---

## Phase 2 — Docker + SQL Server

Create:

```text
docker-compose.yml
.env.example
API Dockerfile
```

Start SQL Server.

Verify connectivity.

Completion criteria:

```bash
docker compose up
```

starts SQL Server and API successfully.

---

## Phase 3 — Persistence

Implement:

- `PlatformDbContext`
- entity configurations
- schemas
- initial migration
- migration commands
- health check

Completion criteria:

Database can be created from migrations.

---

## Phase 4 — Common Application Building Blocks

Implement:

```text
Result
Result<T>
Error
ErrorType

ICommand
ICommand<T>
IQuery<T>

handlers

validation pipeline

IDateTimeProvider
ICurrentUser
```

Completion criteria:

Handlers can be implemented without depending on web or EF-specific details unnecessarily.

---

## Phase 5 — Authentication Domain

Implement:

```text
User
RefreshToken
Role
Permission
```

Add EF mappings.

Add initial roles and permissions.

---

## Phase 6 — Registration REST API

Implement:

```text
POST /api/v1/auth/register
```

Include:

- request contract
- command
- validator
- handler
- duplicate email check
- password hashing
- persistence
- response contract
- tests

---

## Phase 7 — Login REST API

Implement:

```text
POST /api/v1/auth/login
```

Include:

- credential validation
- JWT
- refresh token
- token persistence
- structured logging
- functional tests

---

## Phase 8 — Token Refresh / Logout

Implement:

```text
POST /api/v1/auth/refresh
POST /api/v1/auth/logout
GET  /api/v1/auth/me
```

Add refresh token rotation.

Add token revocation.

Add tests.

---

## Phase 9 — Authorization

Implement:

- roles
- permissions
- authorization policies
- claims creation
- current user

Protect REST and GraphQL operations.

---

## Phase 10 — Platform Domain

Implement:

```text
Site
Gateway
Device
Alert
```

Add:

- invariants
- EF mappings
- relationships
- migrations

---

## Phase 11 — Seed Data

Add Development-only seed data.

Verify database contains enough data to render a useful dashboard.

---

## Phase 12 — GraphQL Foundation

Configure Hot Chocolate.

Add:

```text
Query
Mutation
authorization
error filtering
pagination
DataLoader infrastructure
```

Expose GraphQL development UI only in appropriate environments.

---

## Phase 13 — GraphQL Dashboard

Implement:

```graphql
dashboard
```

Return aggregated platform statistics.

Add integration/functional tests.

---

## Phase 14 — GraphQL Devices

Implement:

```graphql
devices
device
```

Support:

- authorization
- pagination
- filtering
- sorting

Add tests.

---

## Phase 15 — GraphQL Mutations

Implement:

```graphql
updateDevice
acknowledgeAlert
```

Flow:

```text
GraphQL mutation
      ↓
Command
      ↓
Validation
      ↓
Domain behavior
      ↓
EF persistence
      ↓
Payload
```

Add tests.

---

## Phase 16 — DataLoader

Add batching for relationships that cause N+1 queries.

Measure queries before and after if possible.

---

## Phase 17 — Error Handling

Implement:

- global REST exception handler
- Problem Details
- GraphQL error filtering
- Result mapping
- trace IDs

---

## Phase 18 — Logging and Observability

Implement:

- structured logging
- request logging
- trace/correlation ID
- OpenTelemetry
- health checks

---

## Phase 19 — Rate Limiting and Security Hardening

Implement:

- auth rate limits
- GraphQL limits
- CORS
- safe headers
- configuration validation
- sensitive data review

---

## Phase 20 — Complete Automated Tests

Ensure:

```text
Unit
Integration
Functional
Architecture
```

all run through:

```bash
dotnet test
```

---

## Phase 21 — CI

Create CI workflow:

```text
Checkout
   ↓
Setup .NET
   ↓
Restore
   ↓
Build
   ↓
Test
   ↓
Publish
   ↓
Docker build
```

Optional:

- formatting check
- coverage
- dependency scanning

---

# 71. Directory.Build.props

Recommended common settings:

```xml
<Project>
  <PropertyGroup>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <TreatWarningsAsErrors>true</TreatWarningsAsErrors>
    <AnalysisLevel>latest</AnalysisLevel>
  </PropertyGroup>
</Project>
```

If some analyzer warnings are unsuitable, configure them intentionally instead of disabling all warnings.

---

# 72. Central Package Management

Use:

```text
Directory.Packages.props
```

to keep package versions centralized.

This keeps projects consistent and simplifies upgrades.

---

# 73. README Requirements

The repository README must explain:

## What the project demonstrates

```text
Modular monolith
Clean Architecture
Vertical slices
CQRS
REST
GraphQL
JWT authentication
Refresh tokens
SQL Server
EF Core
Docker
Testcontainers
OpenTelemetry
Architecture testing
```

## Why REST for authentication

Authentication is command/session oriented and maps clearly to HTTP endpoints.

## Why GraphQL for dashboard data

Dashboard pages aggregate several related resources and benefit from client-selected fields and composed reads.

## Why modular monolith

It keeps deployment simple while preserving internal boundaries.

## Why SQL Server

It demonstrates enterprise relational persistence and EF Core experience.

## Why Docker

It creates a repeatable local environment.

## Why Testcontainers

Tests run against real SQL Server behavior instead of relying on an in-memory substitute.

---

# 74. Demo Script for Interview

The finished project should support this demo:

```text
1. docker compose up

2. Show SQL Server running.

3. Open REST/OpenAPI documentation.

4. Register a user.

5. Login.

6. Show returned access token / safe auth response.

7. Open GraphQL development UI.

8. Add bearer token.

9. Run dashboard query.

10. Query devices using pagination/filter.

11. Execute updateDevice mutation.

12. Query device again.

13. Show permission-protected operation.

14. Run automated tests.

15. Explain Testcontainers SQL Server tests.

16. Show architecture tests.

17. Show Dockerfile and CI pipeline.

18. Explain why this is a modular monolith instead of microservices.
```

---

# 75. Interview Talking Points

Be able to explain:

### Why not microservices?

The application does not yet need independent deployment, scaling, or organizational ownership. A modular monolith avoids distributed-system complexity while preserving boundaries.

### Why CQRS?

Commands and queries have different concerns. Dashboard reads benefit from optimized projections while commands preserve domain behavior.

### Why not a generic repository?

EF Core already provides repository/unit-of-work-like capabilities. Domain-specific repositories are introduced only where they improve aggregate semantics.

### Why REST and GraphQL together?

They solve different API needs. REST gives straightforward authentication/session operations. GraphQL provides flexible dashboard aggregation and domain reads.

### Why real SQL Server integration tests?

Relational constraints, SQL translation, indexes, transactions, and provider-specific behavior cannot be reliably validated with an in-memory database.

### Why Docker?

Every interviewer/developer can run the same environment without installing SQL Server locally.

---

# 76. Definition of Done

Backend is considered interview-ready when all items below are complete.

## Architecture

- [ ] Solution builds with no warnings.
- [ ] Project dependency rules are enforced.
- [ ] Architecture tests pass.
- [ ] `Program.cs` remains small and readable.
- [ ] Business logic does not exist in REST endpoints or GraphQL resolvers.

## Authentication

- [ ] Registration works.
- [ ] Login works.
- [ ] Passwords are securely hashed.
- [ ] JWT access token works.
- [ ] Refresh token works.
- [ ] Rotation works.
- [ ] Logout/revocation works.
- [ ] `/me` works.

## Authorization

- [ ] Roles exist.
- [ ] Permissions exist.
- [ ] REST authorization works.
- [ ] GraphQL authorization works.

## Database

- [ ] SQL Server runs in Docker.
- [ ] EF migrations work.
- [ ] Database schemas are organized.
- [ ] Important constraints/indexes exist.
- [ ] Seed data exists.

## GraphQL

- [ ] Dashboard query works.
- [ ] Device queries work.
- [ ] Pagination works.
- [ ] Filtering works.
- [ ] Sorting works.
- [ ] Mutations work.
- [ ] Authorization works.
- [ ] DataLoader is used where needed.
- [ ] Query safeguards are configured.

## Quality

- [ ] Validation is centralized.
- [ ] Error responses are standardized.
- [ ] Logging is structured.
- [ ] Health checks work.
- [ ] Trace/correlation IDs exist.
- [ ] OpenTelemetry is configured.
- [ ] Cancellation tokens are passed properly.

## Tests

- [ ] Unit tests pass.
- [ ] SQL Server Testcontainers integration tests pass.
- [ ] Functional REST tests pass.
- [ ] Functional GraphQL tests pass.
- [ ] Architecture tests pass.

## DevOps

- [ ] Dockerfile builds.
- [ ] Docker Compose starts complete local environment.
- [ ] Secrets are not committed.
- [ ] CI builds and tests the solution.

---

# 77. AI Coding Agent Rules

When an AI coding agent implements this architecture, it must follow these rules.

1. Read this entire document before changing code.

2. Inspect existing files before creating duplicates.

3. Follow the existing solution/project naming:
   `MultiTech.Platform.*`

4. Do not modify the React application during backend implementation.

5. Do not introduce microservices.

6. Do not add Redis, Service Bus, Hangfire, Kafka, Kubernetes, or other infrastructure unless explicitly requested.

7. Keep REST endpoints thin.

8. Keep GraphQL resolvers thin.

9. Put business rules inside Domain/Application.

10. Use commands for state-changing use cases.

11. Use queries for read use cases.

12. Never expose EF entities directly through public APIs.

13. Never create a generic repository unless a concrete need is demonstrated.

14. Reuse existing abstractions before creating new ones.

15. Do not create a generic helper merely because two lines look similar.

16. Use async APIs where I/O exists.

17. Pass `CancellationToken`.

18. Use structured logging.

19. Never log credentials or tokens.

20. Add validation for public inputs.

21. Add automated tests with each feature.

22. Use SQL Server Testcontainers for persistence integration tests.

23. Keep methods and classes focused.

24. Prefer composition over inheritance.

25. Add XML/docs only where they add value; do not generate noisy comments.

26. Avoid comments that merely restate code.

27. Run build/tests after each implementation phase.

28. Do not leave failing warnings.

29. Do not silently swallow exceptions.

30. Do not return raw exceptions to clients.

31. Keep dependency directions intact.

32. Add migrations whenever persistence models change.

33. Update README when developer setup changes.

34. Update GraphQL schema tests when the public schema changes.

35. Keep solution production-oriented but intentionally simple.

---

# 78. First Implementation Task

The first coding-agent task should be:

```text
Create the foundation of the MultiTech.Platform backend.

Requirements:

1. Keep the existing MultiTech.Platform solution.

2. Create these projects if they do not already exist:
   - MultiTech.Platform.Api
   - MultiTech.Platform.Application
   - MultiTech.Platform.Domain
   - MultiTech.Platform.Infrastructure
   - MultiTech.Platform.Contracts

3. Create:
   - UnitTests
   - IntegrationTests
   - FunctionalTests
   - ArchitectureTests

4. Configure project references according to this document.

5. Enable nullable reference types.

6. Add Directory.Build.props.

7. Add Directory.Packages.props for central package management.

8. Create the base directory structure described in this document.

9. Do not implement domain features yet.

10. Add simple dependency-registration extension methods:
    - AddApplication()
    - AddInfrastructure()
    - AddPresentation()

11. Keep Program.cs minimal.

12. Add architecture tests for dependency direction.

13. Run:
    dotnet restore
    dotnet build
    dotnet test

14. Fix all failures and warnings before finishing.

Do not add Redis, messaging, Kubernetes, React integration,
or unrelated infrastructure.
```

After the foundation is stable, proceed to:

```text
Docker + SQL Server
```

and then to authentication.

---

# 79. Target End State

The final backend should have this flow:

```text
                    ┌─────────────────────┐
                    │ Existing React App  │
                    └──────────┬──────────┘
                               │
                 ┌─────────────┴─────────────┐
                 │                           │
               REST                       GraphQL
                 │                           │
          Authentication             Query / Mutation
                 │                           │
                 └──────────────┬────────────┘
                                │
                       Application Layer
                   Commands / Queries / DTOs
                                │
                             Domain
                                │
                         Infrastructure
                   ┌────────────┴────────────┐
                   │                         │
              EF Core                   Observability
                   │
              SQL Server
                   │
                Docker
```

The architecture should demonstrate senior engineering through clear decisions, not through unnecessary complexity.

The most important quality is that every major design choice can be explained and defended during an interview.
