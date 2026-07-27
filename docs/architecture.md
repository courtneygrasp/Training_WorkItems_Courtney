# Architecture Notes

## Project Responsibility Summary

| Project | Responsibility |
|---|---|
| `Training.WorkItems.Api` | HTTP concerns: controllers, request/response contracts, middleware, authentication setup, authorization policies, and request-to-command mapping |
| `Training.WorkItems.Application` | Use case orchestration, repository interfaces, application result types, validators, and application-level service abstractions |
| `Training.WorkItems.Infrastructure` | External implementation details: SQL repositories, HTTP context adapters, storage records, clock implementation, and infrastructure configuration |
| `Training.WorkItems.Domain` | Core business entities, value types, business rules, domain services, and domain exceptions |
| `Training.WorkItems.Core` | Composition root that wires Application and Infrastructure registrations together; no domain or use case logic |

## Dependency Direction

Dependencies flow inward. Outer layers depend on inner layers; inner layers do not depend on outer layers.

```
Api  ->  Application  ->  Domain
         Infrastructure  ->  Application, Domain
         Core  ->  Application, Infrastructure  (composition root only)
```

- `Application` defines abstractions (interfaces); `Infrastructure` implements them.
- `Application` never references `Infrastructure` at compile time — if it does, a boundary has been violated.
- `Api` calls use cases through `Application` interfaces; it does not instantiate infrastructure directly.

## Application and Infrastructure Boundary

Application owns the **why** (what must happen). Infrastructure owns the **how** (how it is stored or called).

Application defines:
- Use case interfaces and implementations
- Repository interfaces consumed by use cases
- Validation logic at the application level

Infrastructure implements:
- Repository interfaces defined in Application
- SQL adapters, HTTP adapters, external clients
- Infrastructure-specific configuration and registration

The boundary is enforced by project references: `Training.WorkItems.Application` does not reference `Training.WorkItems.Infrastructure`.

## Repository Placement

| Concern | Location |
|---|---|
| Repository interface (used by use cases) | `Training.WorkItems.Application/[Feature]/Repositories/` |
| Repository implementation (SQL, in-memory) | `Training.WorkItems.Infrastructure/[Feature]/Repositories/` |
| Storage record (database row shape) | `Training.WorkItems.Infrastructure/[Feature]/Storage/` |

Use cases depend on the interface in Application. Infrastructure provides the implementation. Tests can substitute an in-memory fake without requiring Infrastructure.

## API Contract Placement

Request and response contracts belong in the API project, scoped by feature.

```
Api/[Feature]/Contracts/Requests/   — inbound request DTOs
Api/[Feature]/Contracts/Responses/  — outbound response DTOs
Api/[Feature]/Controllers/          — controllers and endpoints
Api/Middleware/                     — cross-cutting HTTP middleware
```

Controllers translate HTTP into use-case commands and use-case results into HTTP responses. They do not contain business logic or domain knowledge.

## Web and Domain Guidance

Web should avoid depending on Domain by default. Domain entities and domain services should not be used directly by Web.

Web may reference Domain sparingly for stable, low-behavior shared concepts:

Acceptable:
```text
Training.WorkItems.Domain.WorkItems.ValueTypes.WorkItemId
Training.WorkItems.Domain.WorkItems.Enums.WorkItemStatus
```

Avoid in Web:
```text
Training.WorkItems.Domain.WorkItems.Entities.WorkItem
Training.WorkItems.Domain.WorkItems.Services.DefaultWorkItemStatusPolicy
Training.WorkItems.Domain.WorkItems.Exceptions.InvalidWorkItemStateException
```

Preferred pattern: Application or Domain determines whether an action is allowed; Api returns a flag (`CanPerformAction = true/false`); Web displays or hides the action based on that response.

Avoid: Web loads a Domain entity, invokes a business method, then decides whether an action is allowed.

## When to Split Core

The solution currently has separate `Training.WorkItems.Application` and `Training.WorkItems.Infrastructure` projects, with `Training.WorkItems.Core` acting as a composition root.

Do not split further unless there is practical need:

- Application is accidentally referencing Infrastructure at compile time
- Compile-time enforcement is needed across multiple products
- Infrastructure grows large enough to create noise alongside Application
- Multiple APIs or workers need Application without Infrastructure
- Testing becomes awkward because Infrastructure is always pulled along
- Genuine cross-product reuse makes a standalone Application package valuable

Do not split if the result is tiny projects with only a few files and no realized benefit.

## Folder Naming

Do not create `.Common` or `.Utils` folders. Name every folder by its feature or by a specific responsibility.

| Avoid | Prefer |
|---|---|
| `Application/Common/` for unrelated types | `Application/WorkItems/Results/`, `Application/WorkItems/Time/` |
| `Infrastructure/Utils/` | `Infrastructure/WorkItems/Configuration/`, `Infrastructure/WorkItems/Storage/` |

Every folder should answer: *who owns the reason this exists?*

Note: `Training.WorkItems.Application/Common/` currently holds `ApplicationResult<T>` and `ISystemClock`. These are candidates for `Application/WorkItems/Results/` and `Application/WorkItems/Time/` respectively when the cost of the rename is justified.

## Test Layer

Uses Moq for simple collaborator isolation, Awesome Assertions for readable assertions, and in-memory fakes when behavior is more important than interaction verification. Integration tests target a real database and SQL infrastructure — not mocks — to catch migration and mapping issues at the boundary.
