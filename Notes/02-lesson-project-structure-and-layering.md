# 02 - Project Structure and Layering

## Lesson Purpose

Create the solution skeleton for the cumulative learning project and establish the dependency direction that every later lesson must respect.


## Training Project

All lessons build the same cumulative project: **Tenant-Aware Work Item Tracker API**.

The project is intentionally small. It is not a mini version of the full platform. It is a bounded teaching project that lets engineers practice modern .NET habits in a repo they can understand, run, test, and explain.

Expected solution structure:

```text
Training.WorkItems.Api
Training.WorkItems.Application
Training.WorkItems.Domain
Training.WorkItems.Infrastructure
Training.WorkItems.Tests
```

The target domain includes:

- Work items
- Work item status changes
- Work item notes
- Work item audit records
- Tenant-scoped access
- Current user context from graspAUTH
- SQL-backed infrastructure access
- Unit tests with Moq, Awesome Assertions, and in-memory fakes where appropriate


## Learning Objectives

By the end of this lesson, the engineer should be able to:

- Create a layered .NET solution.
- Explain the responsibility of each project.
- Configure valid project references.
- Prevent domain/application logic from depending on infrastructure details.
- Add an initial architecture note to the repo.

## Target Solution Structure

```text
Training.WorkItems.sln
/src
  /Training.WorkItems.Api
  /Training.WorkItems.Application
  /Training.WorkItems.Domain
  /Training.WorkItems.Infrastructure
/tests
  /Training.WorkItems.Tests
/docs
```

## Project Responsibilities

| Project | Responsibilities | Must Not Contain |
|---|---|---|
| Api | HTTP endpoints, authentication setup, authorization policies, request/response mapping | SQL, business rules, persistence details |
| Application | Use case coordination, commands/queries, service interfaces, application results | HTTP context parsing, SQL implementation |
| Domain | Entities, value objects, business rules, status behavior | DI, SQL, ASP.NET Core, config |
| Infrastructure | SQL implementations, grasp.Infrastructure package usage, current user adapter, external integrations | Use case orchestration, controller logic |
| Tests | Unit tests, fake implementations, Moq tests, Awesome Assertions assertions | Production-only wiring assumptions |

## Dependency Direction

Allowed references:

```text
Api -> Application
Api -> Infrastructure
Application -> Domain
Infrastructure -> Application
Infrastructure -> Domain
Tests -> Api/Application/Domain/Infrastructure as needed
```

Not allowed:

```text
Domain -> Infrastructure
Domain -> Api
Application -> Infrastructure
Application -> Api
Infrastructure -> Api
```

## Why This Matters

The dependency direction is the fence around the garden. Without it, SQL, HTTP, authentication claims, logging details, and business rules all wander into each other's flowerbeds.

The point is not academic purity. The point is change safety:

- We can swap SQL implementation without changing domain rules.
- We can test application services without hosting the API.
- We can change authentication mechanics without changing every use case.
- We can explain where new behavior belongs.

## Commands

Create the solution:

```bash
dotnet new sln -n Training.WorkItems
mkdir src tests docs

dotnet new webapi -n Training.WorkItems.Api -o src/Training.WorkItems.Api
dotnet new classlib -n Training.WorkItems.Application -o src/Training.WorkItems.Application
dotnet new classlib -n Training.WorkItems.Domain -o src/Training.WorkItems.Domain
dotnet new classlib -n Training.WorkItems.Infrastructure -o src/Training.WorkItems.Infrastructure
dotnet new xunit -n Training.WorkItems.Tests -o tests/Training.WorkItems.Tests

dotnet sln add src/Training.WorkItems.Api/Training.WorkItems.Api.csproj
dotnet sln add src/Training.WorkItems.Application/Training.WorkItems.Application.csproj
dotnet sln add src/Training.WorkItems.Domain/Training.WorkItems.Domain.csproj
dotnet sln add src/Training.WorkItems.Infrastructure/Training.WorkItems.Infrastructure.csproj
dotnet sln add tests/Training.WorkItems.Tests/Training.WorkItems.Tests.csproj
```

Add references:

```bash
dotnet add src/Training.WorkItems.Api reference src/Training.WorkItems.Application
dotnet add src/Training.WorkItems.Api reference src/Training.WorkItems.Infrastructure
dotnet add src/Training.WorkItems.Application reference src/Training.WorkItems.Domain
dotnet add src/Training.WorkItems.Infrastructure reference src/Training.WorkItems.Application
dotnet add src/Training.WorkItems.Infrastructure reference src/Training.WorkItems.Domain
dotnet add tests/Training.WorkItems.Tests reference src/Training.WorkItems.Application
dotnet add tests/Training.WorkItems.Tests reference src/Training.WorkItems.Domain
dotnet add tests/Training.WorkItems.Tests reference src/Training.WorkItems.Infrastructure
```

Add testing packages:

```bash
dotnet add tests/Training.WorkItems.Tests package Moq
dotnet add tests/Training.WorkItems.Tests package AwesomeAssertions
```

For Awesome Assertions v9+, use:

```csharp
using AwesomeAssertions;
```

Older migrated examples may use `using FluentAssertions;`; do not mix styles in new lesson code.

## Initial Folder Conventions

Suggested folders:

```text
Application
  /Abstractions
  /WorkItems
  /Common
Domain
  /WorkItems
Infrastructure
  /Persistence
  /Authentication
  /DependencyInjection
Api
  /Controllers
  /Contracts
  /DependencyInjection
Tests
  /Application
  /Domain
  /Infrastructure
  /Fakes
```

## Architecture Note

Create:

```text
/docs/architecture.md
```

Start with:

```markdown
# Architecture Notes

## API Layer
Owns HTTP concerns, authentication setup, authorization policies, and request/response mapping.

## Application Layer
Owns use case coordination and depends on abstractions, not infrastructure implementations.

## Domain Layer
Owns core business concepts and rules.

## Infrastructure Layer
Owns external implementation details such as SQL, authentication adapters, and infrastructure package integration.

## Test Layer
Uses Moq for simple collaborator isolation, Awesome Assertions for readable assertions, and in-memory fakes when behavior is more important than interaction verification.
```

## Challenge

Submit a pull request containing:

- Solution and project structure.
- Valid project references.
- Testing packages added.
- Initial `/docs/architecture.md`.
- A successful `dotnet build`.
- A short explanation of why `Application` does not reference `Infrastructure`.

## Measurement Rubric

| Score | Evidence |
|---:|---|
| 1 | Projects created but references are incorrect or circular |
| 2 | Basic structure exists but responsibilities are unclear |
| 3 | Correct structure and references with minimal explanation |
| 4 | Clear architecture note and correct test package setup |
| 5 | Explains dependency direction and tradeoffs confidently |


## References

- Microsoft Learn: Dependency injection in .NET - https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection/overview
- Microsoft Learn: Dependency injection in ASP.NET Core - https://learn.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection
- Microsoft Learn: Controller-based web APIs - https://learn.microsoft.com/en-us/aspnet/core/web-api
- Microsoft Learn: Unit testing best practices for .NET - https://learn.microsoft.com/en-us/dotnet/core/testing/unit-testing-best-practices
- Microsoft Learn: Integration tests in ASP.NET Core - https://learn.microsoft.com/en-us/aspnet/core/test/integration-tests
- Microsoft Learn: Common web application architectures / Clean Architecture - https://learn.microsoft.com/en-us/dotnet/architecture/modern-web-apps-azure/common-web-application-architectures
- Moq GitHub / Quickstart - https://github.com/devlooped/moq
- Awesome Assertions documentation - https://awesomeassertions.org/
