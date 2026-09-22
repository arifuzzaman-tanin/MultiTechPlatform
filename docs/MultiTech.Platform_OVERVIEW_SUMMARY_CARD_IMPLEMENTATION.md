# Dashboard Overview Backend Implementation Guide

> Use this file as the implementation prompt/specification for an AI coding agent working in the existing .NET backend project.
>
> Scope: backend only. Do not create React components, frontend GraphQL hooks, frontend types, or UI implementation in this task.

## 1. Objective

Implement the backend data model, seed data, EF Core persistence, application/service layer, and GraphQL API required by the Dashboard Overview page.

The Dashboard Overview page needs these summary cards:

- Managed Devices
- Online
- Gateways
- Sensors
- Critical Alerts
- Connectivity Health
- Messages / 24h
- Sites

Expected backend flow:

```text
Database Tables/Entities
    -> Seed Data
    -> EF Core
    -> Backend Service/Application Layer
    -> GraphQL Query
    -> GraphQL API Response
```

The frontend will later call the GraphQL API and display these values in Dashboard Overview cards.

## 2. Mandatory First Step

Before changing code, inspect the existing .NET backend project.

Identify and follow:

1. Solution and project structure.
2. Existing domain/entity folder conventions.
3. Existing `DbContext` location and EF Core configuration style.
4. Existing migration project and migration naming conventions.
5. Existing seed-data approach.
6. Existing repository, service, application, or CQRS patterns.
7. Existing GraphQL server library and conventions, such as Hot Chocolate.
8. Existing GraphQL query/type/resolver organization.
9. Existing DTO, mapper, validation, logging, and error-handling patterns.
10. Existing test projects and test-data conventions.

Do not introduce a parallel architecture. Reuse existing layers and patterns where they already exist.

## 3. Out Of Scope

Do not implement:

- React components.
- Frontend GraphQL queries.
- Frontend generated types.
- Frontend state management.
- Dashboard charts beyond the summary-card backend data.
- Authentication or authorization changes unless the existing GraphQL endpoint requires applying an existing policy.
- Real external telemetry ingestion.
- Background jobs for live aggregation unless the backend already has an established pattern and the product requires it.

## 4. Data Requirements

The GraphQL API must return all values needed for the Dashboard Overview summary cards in one efficient request.

Required response fields:

```text
managedDevices
onlineDevices
gateways
sensors
criticalAlerts
connectivityHealthPercent
messagesLast24Hours
sites
lastUpdatedUtc
```

Recommended optional fields if they fit the existing dashboard model:

```text
offlineDevices
degradedDevices
openAlerts
warningAlerts
messagesPrevious24Hours
connectivityHealthStatus
```

The implementation should avoid separate database calls per card. Prefer a single service method that returns a strongly typed Overview DTO/view model.

## 5. Recommended Domain Model

Adapt names to the existing project conventions.

If equivalent entities already exist, reuse them instead of creating duplicates.

Recommended entities:

```text
Site
Device
Gateway
Sensor
Alert
TelemetryMessageAggregate
```

If the backend already models gateways and sensors as device subtypes, prefer one `Device` table with a `DeviceType` enum. If the backend already has separate `Gateways` and `Sensors` tables, follow that existing pattern.

### Device Type

Use an enum or existing constants:

```csharp
public enum DeviceType
{
    Gateway = 1,
    Sensor = 2
}
```

### Device Status

Use an enum or existing constants:

```csharp
public enum DeviceStatus
{
    Online = 1,
    Offline = 2,
    Degraded = 3,
    Maintenance = 4
}
```

### Alert Severity

Use an enum or existing constants:

```csharp
public enum AlertSeverity
{
    Info = 1,
    Warning = 2,
    Critical = 3
}
```

### Alert Status

Use an enum or existing constants:

```csharp
public enum AlertStatus
{
    Open = 1,
    Acknowledged = 2,
    Resolved = 3
}
```

Avoid hard-coded strings for device type, status, severity, and alert state.

## 6. Entity Guidance

Use the existing entity base classes, audit fields, tenant/organization fields, and soft-delete conventions if they exist.

Suggested entity shape:

```text
Site
  Id
  Name
  Code
  Region
  IsActive
  CreatedAtUtc
  UpdatedAtUtc

Device
  Id
  SiteId
  Name
  SerialNumber
  DeviceType
  Status
  LastSeenAtUtc
  IsManaged
  CreatedAtUtc
  UpdatedAtUtc

Alert
  Id
  DeviceId
  SiteId
  Title
  Severity
  Status
  CreatedAtUtc
  ResolvedAtUtc

TelemetryMessageAggregate
  Id
  SiteId
  BucketStartUtc
  BucketEndUtc
  MessageCount
  CreatedAtUtc
```

Use `DateTimeOffset` for UTC timestamps if that is the existing project standard. Otherwise follow the backend's current timestamp convention.

## 7. EF Core Configuration

Create entity configurations using the existing EF Core pattern, such as `IEntityTypeConfiguration<T>`.

Configure:

- Table names according to project conventions.
- Primary keys.
- Required fields.
- Max lengths for names, codes, serial numbers, and titles.
- Enum conversion according to existing conventions.
- Relationships and delete behavior.
- Indexes needed by the overview query.

Recommended indexes:

```text
Devices: SiteId
Devices: DeviceType
Devices: Status
Devices: IsManaged
Devices: LastSeenAtUtc
Alerts: Severity
Alerts: Status
Alerts: CreatedAtUtc
TelemetryMessageAggregates: BucketStartUtc, BucketEndUtc
TelemetryMessageAggregates: SiteId
Sites: IsActive
```

If the project uses SQL Server, PostgreSQL, or another provider-specific convention, match the existing provider style.

## 8. Seed Data

Create a dedicated seed-data file for Dashboard Overview data.

Possible names:

```text
DashboardOverviewSeedData.cs
DashboardOverviewSeeder.cs
DashboardOverviewDataSeed.cs
```

Place it wherever the existing backend keeps seed data.

The seed data must be:

- Reusable.
- Maintainable.
- Easy to update.
- Idempotent where possible.
- Deterministic enough for local development and tests.
- Realistic enough for the dashboard to look credible.

Do not scatter seed values across entity configurations, migrations, and startup code. Keep Dashboard Overview seed data together unless existing project conventions require otherwise.

### Suggested Seed Dataset

Create realistic dummy data such as:

```text
Sites: 8 to 12
Gateways: 18 to 30
Sensors: 150 to 300
Managed devices total: gateways + sensors
Online devices: roughly 90% to 96%
Degraded devices: roughly 2% to 5%
Offline devices: roughly 2% to 5%
Critical alerts: 5 to 15 open critical alerts
Messages / 24h: 1,500,000 to 8,000,000
Connectivity health: computed from online managed devices / total managed devices
```

Use stable names and serial numbers:

```text
Riverside Plant
Detroit Plant
Toronto Distribution Center
Austin Assembly
Chicago Cold Storage
Vancouver Utilities
Northwind Manufacturing
Summit Energy Site
```

Example device names:

```text
GW-RIV-001
GW-DET-002
SNS-TEMP-RIV-001
SNS-VIB-DET-014
SNS-PWR-AUS-009
```

### Seed Implementation Rules

The seed process should:

1. Check whether dashboard seed data already exists.
2. Insert sites first.
3. Insert gateways and sensors next.
4. Insert alerts related to seeded devices.
5. Insert telemetry message aggregates for the last 24 hours.
6. Save changes using the existing unit-of-work or `DbContext` pattern.
7. Use `CancellationToken` if the existing seed pipeline supports it.

Avoid random data that changes on every run unless the existing seed architecture intentionally supports generated data. If generated values are used, use a fixed seed so local and test data remain predictable.

## 9. Dashboard Overview DTO

Create a backend DTO or read model for the GraphQL response.

Suggested shape:

```csharp
public sealed class DashboardOverviewDto
{
    public int ManagedDevices { get; init; }
    public int OnlineDevices { get; init; }
    public int Gateways { get; init; }
    public int Sensors { get; init; }
    public int CriticalAlerts { get; init; }
    public decimal ConnectivityHealthPercent { get; init; }
    public long MessagesLast24Hours { get; init; }
    public int Sites { get; init; }
    public DateTimeOffset LastUpdatedUtc { get; init; }
}
```

Use the existing project naming conventions. If the GraphQL layer already uses response models instead of DTOs, follow that approach.

## 10. Service/Application Layer

Create or extend the appropriate backend service/application layer according to the existing architecture.

Possible interface:

```csharp
public interface IDashboardOverviewService
{
    Task<DashboardOverviewDto> GetOverviewAsync(CancellationToken cancellationToken);
}
```

Implementation guidance:

- Use `AsNoTracking()` for read-only EF Core queries.
- Use `async/await` properly.
- Pass `CancellationToken` through all async calls.
- Avoid one database call per dashboard card.
- Compute metrics in the database where reasonable.
- Do not load full entity graphs just to count rows.
- Keep the service testable and free of GraphQL-specific concerns.
- Log unexpected failures using the existing logging abstraction.
- Do not log sensitive data.

Efficient query options:

- Use grouped aggregate queries.
- Use independent aggregate tasks only when the active `DbContext` pattern supports it safely.
- Remember that a single EF Core `DbContext` does not support concurrent operations.
- If parallel queries are needed, use existing context factory/unit-of-work patterns. Otherwise run aggregate queries sequentially and keep them minimal.

Connectivity health should be computed as:

```text
online managed devices / managed devices * 100
```

Handle zero managed devices safely and return `0` or the existing domain-approved default.

Messages / 24h should be computed using UTC:

```text
BucketStartUtc >= nowUtc.AddHours(-24)
```

Use the existing clock/time abstraction if the backend has one. Otherwise use `DateTimeOffset.UtcNow` in one place in the service.

## 11. GraphQL API

Expose the Dashboard Overview data through GraphQL using the existing GraphQL architecture.

If the backend uses Hot Chocolate, add a query field similar to:

```graphql
dashboardOverview: DashboardOverview!
```

Suggested query:

```graphql
query DashboardOverview {
  dashboardOverview {
    managedDevices
    onlineDevices
    gateways
    sensors
    criticalAlerts
    connectivityHealthPercent
    messagesLast24Hours
    sites
    lastUpdatedUtc
  }
}
```

Create the required GraphQL query, type, resolver, DTO, mapper, or service registration according to the existing GraphQL organization.

GraphQL layer rules:

- Resolver should be thin.
- Resolver should call the application/service layer.
- Resolver should accept and pass `CancellationToken`.
- Do not place EF Core query logic directly in GraphQL types unless that is the existing architecture.
- Follow existing authorization policies if dashboard data is protected.
- Follow existing GraphQL error-handling conventions.
- Ensure field names match GraphQL naming conventions already used by the API.

## 12. Expected GraphQL Response

Example response:

```json
{
  "data": {
    "dashboardOverview": {
      "managedDevices": 248,
      "onlineDevices": 232,
      "gateways": 24,
      "sensors": 224,
      "criticalAlerts": 9,
      "connectivityHealthPercent": 93.55,
      "messagesLast24Hours": 3864200,
      "sites": 10,
      "lastUpdatedUtc": "2026-09-22T06:30:00Z"
    }
  }
}
```

Keep the response stable and frontend-friendly. The frontend should not need to issue multiple GraphQL requests to render the eight overview cards.

## 13. Dependency Injection

Register any new services using the existing DI conventions.

Example:

```csharp
services.AddScoped<IDashboardOverviewService, DashboardOverviewService>();
```

Only add interfaces where they fit the existing architecture. If the project does not use interfaces for application services, follow the project standard.

## 14. Validation, Error Handling, And Logging

Add validation and guards where they are meaningful.

Requirements:

- Avoid null reference paths.
- Handle empty database state.
- Handle division by zero.
- Use structured logging for unexpected failures.
- Use existing domain/application exception patterns.
- Return GraphQL errors using the established GraphQL error filter/middleware approach.
- Do not expose stack traces or database details through GraphQL.

This query is read-only and does not require input validation unless optional filters are added later.

## 15. Testing

Add tests using the existing backend test stack.

Prioritize:

- Service returns correct counts from seeded/in-memory/test database data.
- Connectivity health calculation handles normal data.
- Connectivity health calculation handles zero managed devices.
- Messages / 24h excludes old aggregates.
- Critical alerts count only open critical alerts.
- GraphQL query returns all required fields.
- GraphQL query uses the expected field names.

Do not write brittle tests that depend on wall-clock timing. Use the existing clock abstraction or controlled timestamps.

## 16. EF Core Migration Instructions

After creating or updating entities and EF Core configurations, create a migration using the existing migration project and startup project.

Inspect the solution first to determine the correct command.

Common examples:

```bash
dotnet ef migrations add AddDashboardOverviewDataModel --project <PersistenceProject> --startup-project <ApiProject>
```

or:

```bash
dotnet ef migrations add AddDashboardOverviewDataModel
```

Then update the database locally:

```bash
dotnet ef database update --project <PersistenceProject> --startup-project <ApiProject>
```

If the project uses a migration bundle, deployment pipeline, or generated SQL scripts, follow that existing process instead.

Review the generated migration before committing:

- Confirm table names are correct.
- Confirm columns are nullable only when intended.
- Confirm enum storage matches existing conventions.
- Confirm indexes are present.
- Confirm foreign keys and delete behavior are correct.
- Confirm no unrelated schema changes were generated.

## 17. Suggested Build Order

Implement in this order:

1. Inspect existing backend architecture.
2. Identify reusable entities or existing telemetry/device/site tables.
3. Create or update domain entities.
4. Create or update EF Core configurations.
5. Register configurations in the existing `DbContext` if required.
6. Create dedicated Dashboard Overview seed-data file.
7. Wire seed data into the existing seed pipeline.
8. Create Dashboard Overview DTO/read model.
9. Create service/application-layer method.
10. Add DI registration if required.
11. Add GraphQL type/resolver/query field.
12. Add tests for service and GraphQL query.
13. Create EF Core migration.
14. Run database update locally if appropriate.
15. Run build, tests, and formatting/lint/analyzer checks.

## 18. Acceptance Criteria

The backend implementation is complete when:

- Required database entities/tables exist or existing equivalents are reused.
- EF Core configurations are explicit and follow project conventions.
- A dedicated Dashboard Overview seed-data file exists.
- Realistic dummy data is inserted through the existing seed process.
- Seed data is maintainable and easy to update.
- The service/application layer returns all eight card values in one DTO/read model.
- The GraphQL API exposes a single efficient Dashboard Overview query.
- The GraphQL response includes all required fields.
- Async/await and `CancellationToken` are used properly.
- The implementation avoids unnecessary database calls.
- Strongly typed enums/constants are used instead of repeated hard-coded strings.
- Validation, error handling, and logging follow backend standards.
- Tests cover the important aggregate behavior.
- EF Core migration is created and reviewed.
- No frontend implementation is included.

## 19. Final Prompt To Give The Coding Agent

Use this after placing this Markdown file in the .NET backend repository:

> Read this entire specification before making changes.
>
> Implement the backend data required for the Dashboard Overview cards only. Do not create React components or frontend integration.
>
> First inspect the existing .NET solution architecture, EF Core setup, seed-data pipeline, service/application patterns, GraphQL architecture, test setup, and coding standards.
>
> Reuse existing Site, Device, Gateway, Sensor, Alert, Telemetry, Organization, or Message entities if they already exist. Only create new entities/tables where the backend does not already have a suitable model.
>
> Create a dedicated Dashboard Overview seed-data file and insert realistic dummy data through the existing seed process.
>
> Expose one efficient GraphQL query that returns all card data needed by the frontend:
>
> - Managed Devices
> - Online
> - Gateways
> - Sensors
> - Critical Alerts
> - Connectivity Health
> - Messages / 24h
> - Sites
>
> Keep GraphQL resolvers thin. Put aggregation logic in the appropriate service/application layer. Use strongly typed models, enums, constants, async/await, cancellation tokens, no-tracking queries, error handling, structured logging, and tests according to the existing project standards.
>
> After changing the EF Core model, create and review the required migration using the backend solution's existing migration command and project conventions.
>
> Run the relevant build, tests, and analyzer checks before finishing.

