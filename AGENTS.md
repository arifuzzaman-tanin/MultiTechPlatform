# AGENTS.md

## Purpose

This file defines mandatory coding rules for AI coding agents and developers working in this .NET / C# codebase.

The goals are:

- Produce clean, readable, maintainable, secure, testable, production-quality code.
- Follow modern .NET and C# practices.
- Prefer simple, reusable designs over clever or over-engineered solutions.
- Keep business logic explicit and easy to understand.
- Make generated code look like code written by an experienced .NET engineer.
- Preserve the existing solution architecture and conventions unless a change is explicitly requested.

These rules apply to all newly created or modified C# code unless a project-specific rule explicitly overrides them.

---

# 1. General Engineering Principles

## 1.1 Mandatory principles

Always follow:

- SOLID principles.
- DRY where it improves maintainability.
- KISS.
- YAGNI.
- Separation of concerns.
- Dependency inversion.
- Encapsulation.
- High cohesion.
- Low coupling.
- Composition over inheritance where appropriate.
- Explicit behavior over hidden magic.
- Immutability where practical.
- Fail fast for invalid programmer assumptions.
- Defensive validation at external boundaries.

Do not introduce abstractions only for the sake of abstraction.

Do not create a design pattern unless it solves a real problem.

Prefer readable code over clever code.

---

# 2. Preserve Existing Architecture

Before writing code:

1. Inspect the existing project structure.
2. Identify existing patterns, abstractions, naming conventions, and shared components.
3. Reuse existing infrastructure instead of creating duplicates.
4. Do not introduce a new architectural style without explicit approval.
5. Place new code in the correct layer and folder.
6. Respect dependency direction between layers.

Do not:

- Reference Infrastructure directly from Domain.
- Put business logic in Controllers.
- Put database logic in UI/API layers.
- Create duplicate helpers, validators, mappers, repositories, or services when an appropriate implementation already exists.
- Move files or rename namespaces unnecessarily.

---

# 3. C# Language Version

Use the C# version supported by the solution.

Prefer modern C# features when they improve clarity.

Examples:

- Records for immutable DTO/value-oriented models where appropriate.
- Pattern matching.
- Switch expressions.
- Required members where appropriate.
- Nullable reference types.
- File-scoped namespaces if already used.
- Collection expressions if supported and consistent with the codebase.
- Primary constructors only when they remain readable.

Do not use a new language feature simply because it exists.

---

# 4. Naming Conventions

Use meaningful names.

Never use vague names such as:

```csharp
data
obj
temp
item1
x
y
foo
bar
result1
manager
helper
util
```

unless their meaning is obvious in a very small local scope.

Use:

- `PascalCase` for classes.
- `PascalCase` for records.
- `PascalCase` for structs.
- `PascalCase` for enums.
- `PascalCase` for methods.
- `PascalCase` for properties.
- `PascalCase` for constants.
- `PascalCase` for public fields when a field is truly required.
- `camelCase` for parameters.
- `camelCase` for local variables.
- `_camelCase` for private instance fields.
- `I` prefix for interfaces.

Examples:

```csharp
public interface IUserService
{
}

public sealed class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
}
```

Use names that describe intent.

Prefer:

```csharp
GetActiveCustomersAsync
```

over:

```csharp
GetCustomers
```

when only active customers are returned.

Boolean names should normally read naturally:

```csharp
isActive
hasPermission
canDelete
shouldRetry
```

Avoid negative boolean names such as:

```csharp
isNotEnabled
```

when a positive alternative is possible.

---

# 5. Never Use Magic Strings

Do not directly compare business-significant strings throughout the code.

Bad:

```csharp
if (status == "Active")
{
}
```

Use an enum when the values represent a closed domain.

Good:

```csharp
public enum UserStatus
{
    Active,
    Inactive,
    Suspended
}
```

```csharp
if (user.Status == UserStatus.Active)
{
}
```

Use constants when the values must remain strings.

Good:

```csharp
public static class ClaimTypes
{
    public const string TenantId = "tenant_id";
    public const string Permission = "permission";
}
```

Use static readonly values when the value cannot be a compile-time constant.

```csharp
public static class ApplicationDefaults
{
    public static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(30);
}
```

Use strongly typed options for configuration values.

Do not scatter:

- role names
- claim names
- route names
- header names
- cache prefixes
- queue names
- topic names
- permission names
- status names
- error codes
- environment names
- policy names

throughout the code as raw strings.

Centralize them using:

- enums
- constants
- static classes
- strongly typed value objects
- options classes

depending on their purpose.

---

# 6. String Comparison Rules

Never use casual string comparison for values where casing or culture matters.

Avoid:

```csharp
if (status.ToLower() == "active")
```

Prefer:

```csharp
if (string.Equals(status, StatusNames.Active, StringComparison.OrdinalIgnoreCase))
```

For identifiers, keys, codes, claims, headers, and internal tokens, prefer ordinal comparison.

Use culture-aware comparison only for human-language content where culture is intentionally relevant.

Never call `ToLower()` or `ToUpper()` only to compare two strings.

---

# 7. No Deeply Nested Conditions

Avoid nested `if` statements.

Maximum recommended nesting depth: **2 levels**.

Three or more nested conditional levels require refactoring unless there is a strong justification.

Bad:

```csharp
if (user != null)
{
    if (user.IsActive)
    {
        if (user.Account != null)
        {
            if (user.Account.IsVerified)
            {
                Process(user);
            }
        }
    }
}
```

Prefer guard clauses:

```csharp
if (user is null)
{
    return;
}

if (!user.IsActive)
{
    return;
}

if (user.Account is null || !user.Account.IsVerified)
{
    return;
}

Process(user);
```

Prefer:

- guard clauses
- extracted methods
- switch expressions
- pattern matching
- polymorphism
- specification/policy objects when complexity justifies them

Do not write long Boolean expressions.

Extract meaningful conditions.

Instead of:

```csharp
if (user != null &&
    user.IsActive &&
    user.Account != null &&
    user.Account.IsVerified &&
    !user.IsBlocked)
```

prefer:

```csharp
if (!CanProcess(user))
{
    return;
}
```

---

# 8. Method Rules

A method should perform one clear responsibility.

Prefer small methods.

General guideline:

- Target: under 20-30 lines.
- Review carefully when a method exceeds 40 lines.
- Refactor when a method exceeds 60 lines unless there is a strong reason.

Do not split methods mechanically just to satisfy a line count.

Extract methods when extraction creates a meaningful business or technical concept.

Avoid methods with many parameters.

Recommended maximum: **4 parameters**.

If many related values are required, consider:

- request objects
- parameter objects
- records
- options types
- domain objects

Avoid Boolean parameters when they change behavior significantly.

Bad:

```csharp
ProcessOrder(order, true, false);
```

Prefer separate intent-revealing methods or an options object.

---

# 9. Class Rules

Each class must have a clear responsibility.

Prefer small, cohesive classes.

General guideline:

- Aim for fewer than 300 lines.
- Review classes over 400 lines.
- Strongly consider refactoring classes over 500 lines.

Do not split classes arbitrarily when doing so harms cohesion.

Avoid "God classes".

A service with many unrelated dependencies is a design smell.

Constructor dependencies should normally remain reasonably small.

If a class requires 7-10+ dependencies, review its responsibilities.

---

# 10. Documentation and Comment Rules

## 10.1 XML documentation is mandatory

Every created or modified:

- class
- interface
- record
- struct
- enum
- public method
- protected method
- public property when its purpose is not completely obvious
- DTO
- request model
- response model
- command
- query
- event
- domain model
- configuration/options model

must contain meaningful XML documentation.

Example:

```csharp
/// <summary>
/// Represents a request to register a new user.
/// </summary>
public sealed record RegisterUserRequest
{
    /// <summary>
    /// Gets the email address used to identify the user.
    /// </summary>
    public required string Email { get; init; }
}
```

Method example:

```csharp
/// <summary>
/// Retrieves an active user by identifier.
/// </summary>
/// <param name="userId">The unique identifier of the user.</param>
/// <param name="cancellationToken">
/// Token used to cancel the asynchronous operation.
/// </param>
/// <returns>
/// The matching active user, or <see langword="null"/> when no user exists.
/// </returns>
public Task<User?> GetActiveUserAsync(
    Guid userId,
    CancellationToken cancellationToken)
{
}
```

Document exceptions when callers are expected to handle them.

```csharp
/// <exception cref="ArgumentException">
/// Thrown when the email address is invalid.
/// </exception>
```

## 10.2 Comments must explain WHY

Do not write comments that merely repeat the code.

Bad:

```csharp
// Check if user is active.
if (user.IsActive)
{
}
```

Use comments for:

- non-obvious business rules
- architectural decisions
- security-sensitive behavior
- unusual framework behavior
- compatibility workarounds
- intentional performance optimizations
- temporary limitations
- reasoning that future developers need

Good:

```csharp
// Keep suspended users in the identity store so audit records
// can continue to reference the original account.
```

## 10.3 Comment style

Use:

```csharp
// Comment.
```

Avoid block comments unless necessary.

Comments must:

- use complete sentences
- begin with a capital letter
- end with punctuation
- remain accurate
- be updated when behavior changes

Never leave misleading comments.

## 10.4 TODO comments

Do not add anonymous TODOs.

Bad:

```csharp
// TODO: Fix this.
```

Prefer:

```csharp
// TODO(ABC-1234): Replace polling with event-driven notification.
```

If no work-item tracking mechanism exists, explain the specific reason and expected future change.

---

# 11. Nullable Reference Types

Nullable reference types should be enabled.

Do not silence nullable warnings without understanding the issue.

Avoid:

```csharp
value!
```

unless nullability has been proven by a framework contract or invariant that the compiler cannot infer.

Validate required external input.

Use nullable types intentionally:

```csharp
string?
Customer?
DateTimeOffset?
```

Do not use nullable types when null has no valid business meaning.

---

# 12. Guard Clauses

Validate public method arguments at boundaries.

Use:

```csharp
ArgumentNullException.ThrowIfNull(user);
```

Use framework guard helpers where appropriate.

For domain/business validation, return or throw domain-specific results according to the project architecture.

Do not mix:

- developer/programming errors
- user validation failures
- business rule failures
- infrastructure failures

into one generic error mechanism.

---

# 13. Constants and Configuration

Never hard-code:

- connection strings
- URLs
- API keys
- credentials
- secrets
- timeout values used across the system
- retry counts
- important limits
- environment-specific values

Use configuration and strongly typed options.

Prefer:

```csharp
public sealed class EmailOptions
{
    public const string SectionName = "Email";

    public required string SenderAddress { get; init; }

    public int TimeoutSeconds { get; init; } = 30;
}
```

Validate options at startup when possible.

---

# 14. Dependency Injection

Use constructor injection by default.

Do not use service locator patterns.

Bad:

```csharp
var service = serviceProvider.GetRequiredService<IUserService>();
```

inside ordinary business classes.

Do not directly instantiate dependencies.

Bad:

```csharp
var repository = new UserRepository();
```

Prefer:

```csharp
public sealed class UserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
}
```

Choose the correct lifetime:

- Singleton: application-wide, thread-safe, no scoped dependencies.
- Scoped: request/unit-of-work scoped dependencies.
- Transient: lightweight stateless services where a new instance is appropriate.

Never inject a scoped dependency into a singleton.

Keep services stateless unless state is intentionally required.

---

# 15. Async / Await

Use asynchronous APIs for I/O-bound operations.

Suffix asynchronous methods with `Async`.

Good:

```csharp
GetUserAsync
SaveChangesAsync
SendEmailAsync
```

Do not use:

```csharp
.Result
.Wait()
.GetAwaiter().GetResult()
```

in normal async application code.

Use:

```csharp
await
```

Avoid unnecessary `Task.Run` around naturally asynchronous I/O.

Propagate `CancellationToken`.

Example:

```csharp
public async Task<Customer?> GetCustomerAsync(
    Guid customerId,
    CancellationToken cancellationToken)
{
    return await _dbContext.Customers
        .AsNoTracking()
        .FirstOrDefaultAsync(
            customer => customer.Id == customerId,
            cancellationToken);
}
```

Do not create a new `CancellationTokenSource` unnecessarily.

Do not ignore a cancellation token when an underlying API supports it.

Use `ConfigureAwait(false)` only where appropriate for reusable library code and where the project's conventions require it. It is generally unnecessary in ASP.NET Core application code.

---

# 16. Exception Handling

Never use exceptions for normal control flow.

Do not catch an exception unless you can:

- handle it
- translate it
- add useful context
- perform necessary cleanup
- retry safely

Avoid:

```csharp
catch (Exception)
{
    return null;
}
```

Never swallow exceptions silently.

Do not expose raw internal exceptions to clients.

Use centralized exception handling for API applications.

Prefer specific exceptions over generic exceptions.

Do not write:

```csharp
throw new Exception("Invalid user.");
```

Prefer an appropriate exception or domain error.

When rethrowing:

Good:

```csharp
throw;
```

Bad:

```csharp
throw ex;
```

because `throw ex` resets the stack trace.

---

# 17. Logging

Use structured logging.

Bad:

```csharp
_logger.LogInformation("User " + userId + " logged in");
```

Good:

```csharp
_logger.LogInformation(
    "User {UserId} logged in successfully.",
    userId);
```

Do not log:

- passwords
- access tokens
- refresh tokens
- API keys
- connection strings
- secrets
- full authentication headers
- sensitive personal information unless explicitly approved and protected

Use appropriate log levels.

- Trace: very detailed diagnostics.
- Debug: developer diagnostics.
- Information: meaningful normal operations.
- Warning: unexpected but recoverable situation.
- Error: failed operation.
- Critical: application/system-level failure.

Do not log and rethrow the same exception in every layer.

Log at the layer where enough context exists and avoid duplicate log noise.

---

# 18. Collections

Expose the least mutable type required.

Prefer:

```csharp
IReadOnlyCollection<T>
IReadOnlyList<T>
IEnumerable<T>
```

when callers should not modify a collection.

Avoid returning a mutable internal collection directly.

Use empty collections instead of `null` when "no items" is valid.

Prefer:

```csharp
return [];
```

or:

```csharp
return Array.Empty<T>();
```

depending on language version and context.

---

# 19. LINQ Rules

Use LINQ when it improves readability.

Avoid complex LINQ chains that hide important logic.

Do not enumerate an expensive sequence repeatedly.

Be aware of deferred execution.

For EF Core, remember that LINQ expressions may be translated into SQL.

Do not call `ToList()` too early.

Bad:

```csharp
var users = await db.Users.ToListAsync(cancellationToken);

return users
    .Where(user => user.IsActive)
    .ToList();
```

Prefer filtering in the database:

```csharp
return await db.Users
    .Where(user => user.IsActive)
    .ToListAsync(cancellationToken);
```

---

# 20. Switches and Pattern Matching

Prefer switch expressions for simple mapping.

Example:

```csharp
return status switch
{
    OrderStatus.Pending => "Pending",
    OrderStatus.Completed => "Completed",
    OrderStatus.Cancelled => "Cancelled",
    _ => throw new ArgumentOutOfRangeException(
        nameof(status),
        status,
        "Unsupported order status.")
};
```

Always handle unknown enum values appropriately.

Do not assume persisted or deserialized enum values are always valid.

---

# 21. Enums

Use enums for closed sets of domain values.

Enum names should be singular unless marked with `[Flags]`.

Good:

```csharp
public enum AccountStatus
{
    Pending = 1,
    Active = 2,
    Suspended = 3
}
```

For persisted enums, prefer explicit numeric values.

Do not persist enum names as strings unless there is a deliberate compatibility strategy.

Do not use enum values for concepts that change dynamically from configuration or database data.

---

# 22. Static Classes

Use static classes for:

- pure utility behavior
- constants grouped by purpose
- extension methods
- stateless deterministic functionality

Do not use static mutable state for application services.

Avoid global mutable state.

Do not replace dependency injection with static service classes.

---

# 23. Extension Methods

Create extension methods only when they:

- improve readability
- represent behavior naturally associated with the extended type
- are broadly reusable
- do not hide significant side effects

Do not create an extension method merely to avoid writing a normal service.

Keep extension classes focused by topic.

---

# 24. Records and DTOs

Prefer records for immutable data-transfer types when appropriate.

Example:

```csharp
public sealed record CreateCustomerRequest(
    string Name,
    string Email);
```

Do not expose EF Core entities directly through APIs.

Use:

- request DTOs
- response DTOs
- commands
- queries
- domain models

as appropriate.

Do not reuse one DTO for unrelated use cases simply to reduce file count.

---

# 25. Domain Models

Keep domain behavior close to the domain entity/value object when appropriate.

Avoid anemic domain models when business rules naturally belong in the domain object.

Protect invariants.

Bad:

```csharp
order.Status = OrderStatus.Completed;
```

when completing an order requires business validation.

Prefer:

```csharp
order.Complete();
```

when the domain model owns that rule.

Do not allow public setters on domain state unless mutation is intentionally unrestricted.

---

# 26. Value Objects

Use value objects for important domain concepts such as:

- Money
- EmailAddress
- PhoneNumber
- Address
- TenantId
- OrderNumber

when doing so meaningfully protects invariants or improves type safety.

Do not create value objects for every primitive without a practical benefit.

---

# 27. Date and Time

Prefer `DateTimeOffset` for real-world timestamps.

Use UTC internally unless a domain requirement says otherwise.

Do not use:

```csharp
DateTime.Now
```

for domain logic that should be testable.

Prefer `TimeProvider` in modern .NET.

Example:

```csharp
public sealed class SubscriptionService
{
    private readonly TimeProvider _timeProvider;

    public SubscriptionService(TimeProvider timeProvider)
    {
        _timeProvider = timeProvider;
    }
}
```

Use `DateOnly` and `TimeOnly` when the domain represents only a date or only a time.

---

# 28. GUID Rules

Prefer:

```csharp
Guid.Empty
```

over manually written empty GUID strings.

Do not use GUID strings internally if a `Guid` type can be used.

Validate externally supplied GUID strings at the boundary.

---

# 29. Money and Decimal Values

Use `decimal` for financial calculations.

Do not use `float` or `double` for currency.

Example:

```csharp
decimal amount = 10.50m;
```

Define and document rounding rules for financial operations.

---

# 30. Equality

Implement value equality intentionally.

Records already provide value-oriented equality.

For classes requiring custom equality, implement it carefully and consistently with `GetHashCode`.

Avoid comparing complex objects by serialized representation.

---

# 31. API Design

Controllers/endpoints must remain thin.

They should mainly:

1. Validate/accept transport input.
2. Delegate to application logic.
3. Translate application results into HTTP responses.

Do not put business logic in controllers.

Use correct HTTP verbs:

- GET: retrieve.
- POST: create/action.
- PUT: complete replacement where appropriate.
- PATCH: partial update.
- DELETE: remove.

Use appropriate HTTP status codes.

Do not always return `200 OK`.

Use consistent error responses, preferably Problem Details for HTTP APIs.

Do not expose stack traces or internal exception messages.

Use API versioning when the application requires long-lived public or external APIs.

---

# 32. REST API Rules

Resource routes should be noun-oriented.

Prefer:

```text
GET /api/customers/{customerId}
```

over:

```text
GET /api/getCustomer/{customerId}
```

Do not leak database implementation details into API contracts.

Use pagination for potentially large collections.

Define:

- maximum page size
- default page size
- sorting rules
- filtering rules

Avoid returning unlimited rows.

---

# 33. GraphQL Rules

Keep GraphQL resolvers thin.

Resolvers should delegate to application/query services.

Avoid business logic directly inside resolvers.

Prevent N+1 queries.

Use DataLoader/batching when appropriate.

Define authorization at the correct field/query/mutation boundary.

Use pagination for large connections/collections.

Do not expose EF entities as an accidental schema contract.

Use explicit input/output types where appropriate.

Avoid expensive unrestricted nested queries.

Apply:

- query depth limits
- complexity/cost limits
- request timeout
- authorization
- validation

where supported by the GraphQL framework.

---

# 34. Authentication

Never implement custom password hashing.

Use approved identity libraries/frameworks.

Never store plain-text passwords.

Use secure password hashing through established identity components.

Do not return passwords or password hashes from APIs.

Authentication answers:

> Who is the user?

Authorization answers:

> What is the user allowed to do?

Do not confuse the two.

---

# 35. Authorization

Never rely only on frontend authorization.

All sensitive operations must be authorized on the server.

Prefer policy/permission-based authorization for scalable business permissions.

Do not scatter role-name comparisons throughout business code.

Bad:

```csharp
if (user.Role == "Admin")
```

Prefer:

```csharp
[Authorize(Policy = AuthorizationPolicies.ManageUsers)]
```

or a centralized authorization service.

Use constants for policies, claims, and permissions.

---

# 36. Security

Follow secure-by-default behavior.

Validate all untrusted external input.

Use parameterized database access.

Do not build SQL using string concatenation.

Never commit:

- secrets
- credentials
- tokens
- certificates containing private keys
- production connection strings

Use secret stores/environment-specific secure configuration.

Protect against relevant vulnerabilities including:

- SQL injection
- command injection
- path traversal
- insecure deserialization
- SSRF
- XSS
- CSRF where applicable
- open redirects
- broken access control
- mass assignment/over-posting
- sensitive data exposure

Do not disable security controls to make development easier without explicit instruction.

---

# 37. Entity Framework Core

Use async database methods.

Use `AsNoTracking()` for read-only queries unless tracking is needed.

Example:

```csharp
var customer = await _dbContext.Customers
    .AsNoTracking()
    .FirstOrDefaultAsync(
        customer => customer.Id == customerId,
        cancellationToken);
```

Project only required columns for read models.

Avoid loading full entities when a DTO projection is enough.

Avoid N+1 query patterns.

Do not call `SaveChangesAsync()` repeatedly inside loops.

Batch changes when practical.

Use explicit transactions only when required.

Remember that `SaveChanges` already uses a transaction for a single save operation where supported.

Use indexes based on actual query patterns.

Never add indexes blindly.

Avoid excessive `Include()` chains.

Use split queries only where they solve a measured/cartesian-explosion problem.

Do not expose `IQueryable<T>` beyond boundaries where arbitrary query composition would leak persistence concerns.

---

# 38. Repository Pattern

Do not add generic repositories automatically on top of EF Core.

`DbContext` already provides repository/unit-of-work behavior.

Introduce repository abstractions only when they provide real value, such as:

- domain-focused persistence behavior
- isolation from persistence technology
- testing strategy
- aggregate-specific access
- complex reusable queries

Avoid generic repositories that merely wrap every `DbSet` method.

Bad abstraction:

```csharp
IRepository<T>
```

with dozens of generic CRUD methods when it provides no meaningful domain behavior.

---

# 39. Transactions

Use a transaction only when multiple operations must succeed or fail atomically and are not already covered by the persistence mechanism.

Keep transactions short.

Do not perform slow network calls while holding a database transaction unless unavoidable.

For cross-service consistency, consider patterns such as:

- transactional outbox
- idempotency
- eventual consistency

when required by the architecture.

---

# 40. Database Migrations

Migrations must be reviewed.

Do not delete production data unintentionally.

Avoid destructive schema changes without a migration strategy.

For large tables, consider deployment impact before:

- rebuilding indexes
- adding non-null columns
- changing data types
- large data backfills

Keep migrations deterministic.

---

# 41. Mapping

Keep mapping logic centralized.

Use explicit mappings when business transformations matter.

Mapping libraries are acceptable when they reduce repetitive code without hiding important behavior.

Do not put business decisions inside opaque mapping configurations.

---

# 42. Validation

Validate input at the application boundary.

Separate:

- syntactic validation
- domain validation
- authorization

Do not duplicate the same validation logic across multiple handlers.

Use a validation library if the project already uses one.

Do not trust client-side validation.

---

# 43. Result Pattern

When business/application failures are expected, prefer the project's established result/error approach rather than throwing exceptions for normal failures.

Examples of expected failures:

- duplicate email
- insufficient balance
- invalid state transition
- missing business resource
- permission denied

Exceptions should normally represent exceptional/unexpected conditions or framework contract failures.

Do not introduce a new Result abstraction if the application already uses another error model.

---

# 44. CQRS

Use CQRS when it improves separation and clarity.

Do not force CQRS on trivial CRUD operations unless the project architecture already requires it.

Commands mutate state.

Queries retrieve data.

Avoid commands that secretly behave as queries.

Keep command/query handlers focused.

---

# 45. Domain Events

Use domain events for meaningful domain occurrences.

Examples:

```text
OrderPlaced
PaymentReceived
CustomerActivated
```

Do not create events for every property change.

Event names should describe something that already happened.

Use past tense where appropriate.

---

# 46. Integration Events

Keep integration contracts versionable and stable.

Do not expose internal entity models directly as integration messages.

Include a unique event/message identifier when idempotency is important.

Consumers must tolerate duplicate delivery when using at-least-once messaging systems.

---

# 47. Messaging

Message handlers should be idempotent where redelivery is possible.

Do not assume a message is delivered exactly once.

Do not perform long blocking operations in message handlers.

Use retries only for transient failures.

Use dead-letter handling where supported.

Do not endlessly retry permanent validation/business failures.

---

# 48. Retry and Resilience

Do not implement manual retry loops unless necessary.

Use established resilience libraries/policies.

Retry only transient failures.

Use:

- timeout
- retry with backoff
- circuit breaker
- rate limiting
- bulkhead/concurrency controls

where appropriate.

Never retry operations that are unsafe to repeat unless they are idempotent or protected by an idempotency mechanism.

---

# 49. HTTP Client

Do not create a new `HttpClient` for every request.

Use `IHttpClientFactory` or the application's established client registration.

Use typed clients where useful.

Configure:

- timeout
- base address
- headers
- resilience policies

centrally.

Validate external API responses.

Do not assume a `2xx` response contains a valid expected payload.

---

# 50. Serialization

Use the project's configured serializer consistently.

Do not mix serializers without a specific need.

Be deliberate about:

- property naming
- enum serialization
- null handling
- date handling
- case sensitivity
- reference cycles

Never deserialize untrusted data into dangerous polymorphic object graphs without strict controls.

---

# 51. Caching

Use caching only when there is a clear performance or scale benefit.

Define:

- cache key
- expiration
- invalidation
- ownership
- fallback behavior

Centralize cache key formats.

Bad:

```csharp
$"customer-{id}"
```

scattered across the codebase.

Prefer:

```csharp
public static class CacheKeys
{
    public static string Customer(Guid customerId) =>
        $"customers:{customerId}";
}
```

Never cache sensitive data without considering exposure and lifecycle.

---

# 52. Performance

Optimize based on evidence.

Do not sacrifice readability for micro-optimization without measurements.

Watch for:

- unnecessary allocations
- repeated database queries
- repeated enumeration
- large in-memory collections
- synchronous I/O
- N+1 queries
- unnecessary serialization
- chatty network calls

Use profiling/metrics for significant optimizations.

Document non-obvious performance-driven code.

---

# 53. Memory Management

Dispose `IDisposable`/`IAsyncDisposable` resources correctly.

Prefer:

```csharp
using
await using
```

when appropriate.

Do not dispose DI-owned services manually.

Be careful with long-lived event subscriptions that can cause memory leaks.

---

# 54. Thread Safety

Singleton services must be thread-safe.

Avoid shared mutable state.

Do not use `lock` unless concurrency requirements justify it.

Prefer concurrent collections where they correctly model the need.

Do not assume ordinary collection types are thread-safe.

---

# 55. File and Stream Handling

Use async stream APIs when handling significant I/O.

Do not load very large files completely into memory without a reason.

Validate:

- file size
- extension
- content type
- storage location

when files come from untrusted users.

Never trust a client-provided filename as a server filesystem path.

---

# 56. Dependency Rules

Avoid adding a package when the framework already provides an adequate solution.

Before introducing a NuGet package:

1. Check whether .NET already supports the feature.
2. Confirm the package is maintained.
3. Review license.
4. Review security history.
5. Avoid packages with excessive dependencies.
6. Prefer widely adopted, mature libraries for infrastructure concerns.

Do not introduce multiple libraries that solve the same problem.

---

# 57. Testing

New business logic must be testable.

Use unit tests for isolated business behavior.

Use integration tests for:

- database behavior
- API behavior
- authentication/authorization
- serialization
- infrastructure boundaries

Tests must follow Arrange / Act / Assert when practical.

Name tests clearly.

Recommended format:

```text
MethodName_Scenario_ExpectedResult
```

Example:

```csharp
RegisterAsync_EmailAlreadyExists_ReturnsConflict
```

Test:

- happy path
- invalid input
- boundary cases
- authorization
- important failures
- state transitions

Do not test private methods directly.

Test public behavior.

Avoid brittle tests coupled to implementation details.

---

# 58. Mocking

Mock external collaborators, not everything.

Avoid mocking simple domain objects.

Do not over-mock EF Core query behavior when an integration test would be safer.

Prefer real objects for simple deterministic dependencies.

Use test doubles intentionally:

- mock
- stub
- fake
- spy

depending on the test purpose.

---

# 59. Integration Testing

Prefer realistic infrastructure for important integration behavior.

Where practical, use:

- test containers
- isolated databases
- real serialization
- real middleware
- real authorization configuration

Do not claim database behavior is tested when only mocked repository calls were tested.

---

# 60. Test Data

Use builders/factories for complicated test data.

Avoid enormous inline object initialization repeated across tests.

Test fixtures must remain deterministic.

Do not depend on:

- current local time
- random external APIs
- execution order
- production resources

---

# 61. Code Duplication

Do not duplicate business logic.

Before creating new code, search for an existing:

- helper
- extension
- validator
- mapper
- service
- constant
- policy
- specification
- formatter
- base implementation

However, do not create a generic abstraction after only one occurrence unless there is a clear future/use-case benefit.

A small amount of duplication is sometimes better than the wrong abstraction.

---

# 62. Abstractions

An abstraction must have a clear purpose.

Do not create:

```text
BaseService
CommonHelper
GeneralManager
UtilityService
IAnythingService
```

with unrelated responsibilities.

Prefer domain-specific names.

Bad:

```csharp
CommonHelper.Process(...)
```

Good:

```csharp
InvoiceNumberGenerator.Generate(...)
```

---

# 63. Interfaces

Do not create an interface for every class automatically.

Create interfaces when needed for:

- dependency inversion
- multiple implementations
- architectural boundaries
- testability
- plugin/strategy behavior
- infrastructure abstraction

A simple immutable utility or internal implementation may not need an interface.

Keep interfaces small and cohesive.

---

# 64. Inheritance

Prefer composition over inheritance.

Use inheritance only for genuine "is-a" relationships or framework extension points.

Avoid deep inheritance hierarchies.

Do not use a base class only to share a few unrelated helper methods.

---

# 65. Sealed Classes

Consider `sealed` for implementation classes that are not designed for inheritance.

Especially consider sealing:

- services
- handlers
- DTOs/classes not intended for extension

Do not seal framework/domain types that intentionally support inheritance.

---

# 66. Access Modifiers

Use the most restrictive access level possible.

Prefer:

- `private`
- `internal`

over `public` when external access is unnecessary.

Keep the public API surface small.

---

# 67. Properties and Fields

Prefer properties over public mutable fields.

Use read-only fields where mutation is unnecessary.

Example:

```csharp
private readonly IUserRepository _userRepository;
```

Avoid unnecessary setters.

Prefer:

```csharp
public string Name { get; private set; }
```

for controlled domain mutation.

---

# 68. `var` Usage

Use `var` when the type is obvious from the right-hand side and improves readability.

Good:

```csharp
var customer = new Customer();
```

Explicit types are preferred when they communicate useful information.

Good:

```csharp
Customer? customer = await FindCustomerAsync(
    customerId,
    cancellationToken);
```

Do not use `var` when it makes the code harder to understand.

Follow the solution's `.editorconfig` if it defines a stricter rule.

---

# 69. Type Keywords

Prefer C# keywords:

```csharp
string
int
bool
decimal
object
```

instead of:

```csharp
String
Int32
Boolean
Decimal
Object
```

unless referring specifically to static members of the runtime type where clarity benefits.

---

# 70. Braces

Always use braces for control structures.

Bad:

```csharp
if (isValid)
    Save();
```

Good:

```csharp
if (isValid)
{
    Save();
}
```

This applies to:

- `if`
- `else`
- `for`
- `foreach`
- `while`
- `using` blocks where applicable

---

# 71. Formatting

Use four spaces for indentation.

Do not use tabs unless the repository explicitly requires them.

One statement per line.

One declaration per line.

Use blank lines to separate logical sections.

Do not vertically align code with excessive spaces.

Let `dotnet format` and `.editorconfig` control formatting where configured.

---

# 72. File Organization

Prefer one primary type per file.

File name should normally match the primary type.

Example:

```text
CustomerService.cs
ICustomerService.cs
CustomerStatus.cs
```

Small tightly related private/nested types may share a file when readability improves.

Do not place many unrelated public classes in one file.

---

# 73. Namespace Rules

Namespaces must follow the solution structure.

Prefer:

```text
Company.Product.Module.Layer
```

or the established project convention.

Do not create random namespace names.

Avoid unnecessary namespace depth.

---

# 74. Using Directives

Remove unused `using` directives.

Follow project conventions for:

- global usings
- implicit usings
- ordering

Do not fully qualify types everywhere if a clean `using` improves readability.

Avoid aliases unless they solve a real name collision or readability problem.

---

# 75. Partial Classes

Avoid partial classes unless required or strongly justified.

Acceptable examples:

- generated code
- framework patterns
- source-generated integration
- very large UI-generated types

Do not use partial classes to hide poor class organization.

---

# 76. Generated Code

Do not manually modify generated code unless the generation mechanism requires it.

Extend generated types through supported extension mechanisms.

Keep custom code separate from generated output.

---

# 77. API Models

Request models should include only fields accepted from clients.

Response models should include only fields clients should see.

Avoid over-posting.

Do not bind external requests directly to domain entities or EF Core entities.

---

# 78. Pagination

Any endpoint/query capable of returning a large dataset must support pagination.

Apply a maximum page size.

Example constants:

```csharp
public static class PaginationDefaults
{
    public const int DefaultPageSize = 25;
    public const int MaximumPageSize = 100;
}
```

Validate page values.

Do not permit arbitrarily large page sizes.

---

# 79. Sorting and Filtering

Whitelist sortable/filterable fields.

Do not dynamically concatenate client input into SQL/order expressions unsafely.

Centralize complex filter logic where it can be tested.

---

# 80. Error Codes

Use stable application error codes for client-consumable business failures.

Do not make clients parse English error messages.

Example:

```csharp
public static class ErrorCodes
{
    public const string UserAlreadyExists = "USER_ALREADY_EXISTS";
}
```

Human-readable error messages may change.

Machine-readable codes should remain stable.

---

# 81. Secrets

Secrets must come from appropriate secret/configuration providers.

Local development may use user secrets or approved local secret mechanisms.

Production should use a managed secret store where available.

Do not log secrets.

Do not include secret values in exceptions.

---

# 82. Environment Checks

Avoid scattering:

```csharp
if (environment == "Production")
```

Use the host environment APIs.

Example:

```csharp
if (environment.IsProduction())
{
}
```

Business behavior should not normally depend directly on environment names.

Prefer configuration/features when possible.

---

# 83. Feature Flags

Use a feature management mechanism for runtime feature rollout when required.

Do not scatter ad-hoc Boolean configuration checks throughout the application.

Centralize feature evaluation.

Remove obsolete feature flags after rollout is complete.

---

# 84. Observability

Important operations should be observable.

Where appropriate include:

- structured logs
- metrics
- distributed traces
- correlation IDs

Do not create excessive high-cardinality metric dimensions.

Do not put sensitive data into telemetry.

---

# 85. Correlation and Tracing

Propagate correlation/tracing context across service boundaries where supported.

Use standard trace context where possible.

Do not invent custom tracing headers when framework/industry standards already solve the problem.

---

# 86. Health Checks

Expose health checks for infrastructure dependencies where operationally useful.

Differentiate:

- liveness
- readiness

Do not mark the application unhealthy for optional/noncritical dependencies unless required.

---

# 87. Background Services

Background workers must:

- support graceful shutdown
- observe cancellation tokens
- handle transient failures
- log failures with useful context
- avoid tight loops
- use delays/timers responsibly

Do not use fire-and-forget tasks from request handlers for important work.

Important work should use a durable background mechanism if reliability matters.

---

# 88. Fire-and-Forget

Avoid:

```csharp
_ = DoSomethingAsync();
```

for important operations.

This can hide:

- exceptions
- cancellation
- process shutdown
- lost work

Use a queue/background worker or await the operation.

---

# 89. Idempotency

Operations likely to be retried must be designed for idempotency where appropriate.

Examples:

- payment requests
- message handlers
- webhook processing
- external commands

Do not assume clients or infrastructure will call an operation only once.

---

# 90. Webhooks

Verify webhook authenticity.

Use the provider's signature mechanism.

Protect against replay when relevant.

Webhook processing should be idempotent.

Acknowledge quickly when provider timeout behavior requires asynchronous processing.

---

# 91. External Integrations

Wrap important external integrations behind a focused client/adapter.

Do not scatter direct third-party SDK calls throughout domain/application code.

Translate third-party models into internal models at the boundary.

Do not leak vendor-specific exceptions through every layer.

---

# 92. Retryable vs Non-Retryable Failures

Classify failures.

Examples of potentially retryable failures:

- transient network error
- throttling
- temporary service unavailable

Usually non-retryable:

- validation failure
- authentication failure
- authorization failure
- malformed request
- domain rule violation

Do not blindly retry every exception.

---

# 93. `CancellationToken`

Any async method that may perform I/O or long-running work should normally accept a `CancellationToken`.

Place it last:

```csharp
public Task<User?> GetUserAsync(
    Guid userId,
    CancellationToken cancellationToken)
```

Do not use `CancellationToken.None` when a caller-provided token is available.

---

# 94. `async void`

Never use `async void` except event handlers that require it.

Use `Task` for asynchronous methods.

Bad:

```csharp
public async void SaveAsync()
```

Good:

```csharp
public async Task SaveAsync(
    CancellationToken cancellationToken)
```

---

# 95. Null Collections

Do not return `null` to represent an empty collection.

Bad:

```csharp
return null;
```

Prefer:

```csharp
return [];
```

when "no results" is a valid outcome.

---

# 96. Boolean Expressions

Avoid comparisons such as:

```csharp
if (isActive == true)
```

Prefer:

```csharp
if (isActive)
```

Avoid:

```csharp
if (isActive == false)
```

Prefer:

```csharp
if (!isActive)
```

unless the longer form is necessary for nullable Boolean handling.

---

# 97. Ternary Expressions

Use ternary expressions only for simple value selection.

Good:

```csharp
var label = isActive ? "Active" : "Inactive";
```

Do not nest ternary expressions.

Bad:

```csharp
var value = a ? x : b ? y : z;
```

Use an `if`, switch expression, or method instead.

---

# 98. Loops

Prefer readable loops over clever LINQ when side effects are involved.

Do not modify a collection while enumerating it unless the collection explicitly supports that operation.

Avoid expensive work repeatedly inside loops.

Move invariant work outside the loop.

---

# 99. Reflection

Avoid reflection when ordinary typed code can solve the problem.

If reflection is required:

- isolate it
- cache expensive metadata where appropriate
- test it thoroughly
- document why it is necessary

Do not use reflection to bypass access or type safety casually.

---

# 100. Dynamic

Avoid `dynamic` unless integrating with a system where static typing is impractical.

Prefer strongly typed models.

Document why `dynamic` is necessary.

---

# 101. Unsafe Code

Do not use `unsafe` code unless there is a measured performance/interoperability requirement and explicit approval.

Keep unsafe sections isolated.

Add tests and documentation.

---

# 102. Regular Expressions

Do not create expensive regex instances repeatedly.

Use generated regex support or cached compiled regex where appropriate.

Set a timeout for regex operating on untrusted input when catastrophic backtracking is possible.

Keep regex patterns readable and documented when complex.

---

# 103. Serialization Contracts

Treat externally consumed contracts as versioned APIs.

Adding fields is usually safer than renaming/removing existing fields.

Do not change JSON property names casually.

Consider backward compatibility before modifying:

- API DTOs
- events
- queue messages
- persisted JSON
- public enums

---

# 104. Backward Compatibility

Before changing a public or persisted contract, check:

- API clients
- database data
- message consumers
- scheduled jobs
- caches
- external integrations

Do not make breaking changes silently.

---

# 105. Feature Implementation Workflow for AI Agents

Before implementing a feature:

1. Read this `AGENTS.md`.
2. Inspect related existing code.
3. Identify existing architecture and patterns.
4. Search for reusable code.
5. Identify security/authorization implications.
6. Identify validation rules.
7. Identify cancellation/async requirements.
8. Identify persistence behavior.
9. Identify tests that should exist.
10. Make the smallest cohesive change that fulfills the requirement.

After implementation:

1. Compile the solution.
2. Run relevant tests.
3. Run formatting/analyzers.
4. Check nullable warnings.
5. Check warnings introduced by the change.
6. Remove unused code.
7. Remove debugging code.
8. Confirm no secrets were added.
9. Confirm comments/XML docs are accurate.
10. Confirm no magic strings were introduced.
11. Confirm no deep nesting was introduced.
12. Confirm new I/O paths propagate cancellation.
13. Confirm authorization is enforced server-side.
14. Confirm error handling is consistent.
15. Confirm logs do not contain sensitive data.

---

# 106. AI Agent Restrictions

AI coding agents must not:

- Rewrite unrelated files.
- Reformat the entire repository for a small change.
- Introduce a new package without a clear reason.
- Introduce a new framework without explicit need.
- Change public APIs unnecessarily.
- Change database schema without explaining the migration impact.
- Disable analyzers to silence valid warnings.
- suppress compiler warnings casually.
- use `#pragma warning disable` without a documented reason.
- use nullable suppression (`!`) as a shortcut.
- add empty catch blocks.
- use `catch (Exception)` without a legitimate boundary-level reason.
- add magic strings.
- add duplicated constants.
- create deeply nested `if` blocks.
- create large methods when the logic can be cleanly decomposed.
- create large multi-purpose services.
- store secrets in source code.
- write business logic inside controllers/resolvers.
- return EF entities directly from public APIs.
- use synchronous I/O in async request paths without justification.
- use `.Result` or `.Wait()` in async code.
- add comments that merely repeat the code.
- remove tests to make a build pass.
- weaken validation or authorization to satisfy a test.
- change working architecture without explicit instruction.

---

# 107. Preferred Code Shape

Prefer code like:

```csharp
/// <summary>
/// Retrieves a customer when the customer exists and is active.
/// </summary>
public sealed class GetCustomerHandler
{
    private readonly ICustomerRepository _customerRepository;

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="GetCustomerHandler"/> class.
    /// </summary>
    /// <param name="customerRepository">
    /// Repository used to retrieve customer information.
    /// </param>
    public GetCustomerHandler(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    /// <summary>
    /// Retrieves an active customer.
    /// </summary>
    /// <param name="customerId">
    /// The unique customer identifier.
    /// </param>
    /// <param name="cancellationToken">
    /// Token used to cancel the operation.
    /// </param>
    /// <returns>
    /// The customer when found; otherwise, <see langword="null"/>.
    /// </returns>
    public async Task<Customer?> HandleAsync(
        Guid customerId,
        CancellationToken cancellationToken)
    {
        if (customerId == Guid.Empty)
        {
            throw new ArgumentException(
                "Customer identifier cannot be empty.",
                nameof(customerId));
        }

        var customer = await _customerRepository.GetByIdAsync(
            customerId,
            cancellationToken);

        if (customer is null)
        {
            return null;
        }

        if (customer.Status != CustomerStatus.Active)
        {
            return null;
        }

        return customer;
    }
}
```

Characteristics:

- Clear names.
- No magic strings.
- Enum used for domain status.
- Shallow conditions.
- Guard clause.
- Async naming.
- Cancellation propagated.
- Dependency injection.
- XML documentation.
- Focused responsibility.
- No unnecessary abstraction.
- Easy to test.

---

# 108. Forbidden Example

Avoid code like:

```csharp
public class Manager
{
    public async Task<object> Do(string type, string status)
    {
        if (type == "customer")
        {
            if (status.ToLower() == "active")
            {
                if (DateTime.Now.Hour > 5)
                {
                    try
                    {
                        var db = new AppDbContext();
                        var x = db.Customers.ToList();

                        // Gets active customers.
                        return x;
                    }
                    catch
                    {
                    }
                }
            }
        }

        return null;
    }
}
```

Problems:

- vague class name
- vague method name
- vague parameters
- magic strings
- unsafe string comparison
- deep nesting
- direct dependency creation
- synchronous database call
- no cancellation
- swallowed exception
- `DateTime.Now`
- vague return type
- unnecessary comment
- no XML documentation
- possible resource/lifetime issue
- no separation of concerns
- no meaningful error model

---

# 109. Analyzer and Build Quality Rules

Treat compiler warnings and analyzer warnings seriously.

New code should not introduce warnings.

Prefer enforcing conventions through:

- `.editorconfig`
- .NET analyzers
- nullable reference types
- `TreatWarningsAsErrors` where appropriate
- `dotnet format`
- architecture tests where valuable

Do not rely only on this document when a rule can be automatically enforced.

---

# 110. Definition of Done

Code is not complete until:

- The solution compiles.
- Relevant tests pass.
- No new warnings are introduced.
- Formatting is correct.
- Naming is meaningful.
- Methods/classes remain focused.
- Nesting is shallow.
- No new magic strings exist.
- Constants/enums/value objects are used appropriately.
- Async code propagates cancellation.
- Exceptions are handled correctly.
- Logging is structured.
- Sensitive values are not logged.
- Public behavior has appropriate tests.
- Documentation comments are present and accurate.
- Security and authorization are enforced.
- Database queries are efficient enough for the use case.
- Public/API contracts are intentional.
- Existing architectural boundaries are preserved.
- No unnecessary packages or abstractions were added.

---

# 111. Final Rule

When multiple implementations are possible, choose the implementation that is:

1. Correct.
2. Secure.
3. Easy to understand.
4. Easy to test.
5. Easy to maintain.
6. Consistent with the existing codebase.
7. Reusable where reuse is genuinely beneficial.
8. Efficient enough for the expected workload.

Do not optimize for the fewest lines of code.

Optimize for clarity, correctness, maintainability, and long-term engineering quality.
