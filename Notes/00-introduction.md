# 00 - Program Introduction

## Purpose

This 30-day mentoring boot camp is designed to help experienced engineers close practical skill gaps in modern Microsoft-stack development while preserving their domain knowledge and institutional IP.

The program is intentionally practical. It is not a lecture series, not an architecture ceremony, and not a miniature platform implementation hiding in a trench coat. Each lesson adds capability to a single cumulative learning project so progress is visible, reviewable, and measurable.


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


## Program Goals

By the end of the program, each engineer should be able to:

1. Build a maintainable ASP.NET Core API using layered architecture.
2. Apply SOLID principles in practical C# code.
3. Use dependency injection correctly, including service lifetimes.
4. Integrate authentication through graspAUTH without spreading claim parsing throughout the codebase.
5. Access SQL through infrastructure abstractions instead of controller or application-layer SQL.
6. Write meaningful tests using Moq, Awesome Assertions, and in-memory fakes.
7. Recognize when Moq is useful and when it creates brittle, over-specified tests.
8. Explain internal platform concepts at a developer level without trying to implement landing, orchestration, replay, or quarantine subsystems.
9. Participate in code review using concrete standards.
10. Produce a final repo that demonstrates intermediate working competency.

## What This Program Is Not

This program does not ask engineers to implement:

- Landing zones
- Object lanes
- Stream pointer orchestration
- Full validation pipelines
- Quarantine infrastructure
- Replay engines
- Transform/load subsystems
- Analytics platform components

Those concepts are taught for literacy and placement. The implementation work stays inside a realistic tenant-aware API.

## Cumulative Deliverable

Each engineer will produce a working solution with:

- API endpoint layer
- Application service layer
- Domain model and rules
- Infrastructure data access
- SQL-backed repository implementation
- graspAUTH/current-user integration
- Dependency injection registration
- Options/configuration
- Logging and correlation ID basics
- Unit tests
- Moq-based interaction tests where appropriate
- Awesome Assertions for readable assertions
- In-memory fake repositories for behavioral tests
- Architecture notes
- Final code review artifacts

## Testing Philosophy

Testing will not be treated as a decorative garnish. It is part of the design feedback loop.

The program uses three complementary testing tools and styles:

| Tool or Style | Best Used For | Avoid When |
|---|---|---|
| Moq | Simple interaction boundaries, external collaborators, verifying calls to side-effect ports | The mock setup becomes longer than the behavior being tested, or the test knows too much about implementation sequence |
| Awesome Assertions | Clear, expressive assertions on outcomes, exceptions, collections, and object equivalency | The assertion hides important intent or compares huge object graphs without explaining what matters |
| In-memory fakes | Repository/connector behavior, tenant-scoped query behavior, multi-step application use cases | The fake diverges from production behavior or starts becoming a second database engine |

A useful rule of thumb:

> Use Moq to isolate a simple collaborator. Use an in-memory fake when behavior matters more than call choreography.

## Weekly Cadence

Each week should include at least one 60-minute mentoring session:

| Segment | Time | Purpose |
|---|---:|---|
| Review previous work | 15 min | Discuss submitted code and gaps |
| Teach concept | 15 min | Short practical lesson |
| Work through example | 20 min | Pair/group refactor or implementation |
| Assign challenge | 10 min | Clarify measurable output |

Additional support sessions may be added for engineers who need focused pair programming or review.

## Measurement

Each engineer is assessed weekly using a 1-5 score in these categories:

| Category | Meaning |
|---|---|
| Layering | Places responsibilities in the correct project/layer |
| SOLID | Applies design principles without overengineering |
| DI | Uses constructor injection and lifetimes correctly |
| Auth/Tenant Safety | Maintains tenant-scoped behavior and avoids scattered claim parsing |
| Data Access | Keeps SQL/infrastructure concerns out of API/application layers |
| Testing | Uses Moq, Awesome Assertions, and fakes appropriately |
| Maintainability | Produces readable, reviewable, change-tolerant code |
| Explanation | Can explain why the code is shaped the way it is |

## Expected Final Outcome

The final question is not: "Did they memorize design principle names?"

The final question is:

> Can this engineer build and explain a maintainable, tenant-aware, authenticated, testable Microsoft-stack API using our expected engineering standards?


## References

- Microsoft Learn: Dependency injection in .NET - https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection/overview
- Microsoft Learn: Dependency injection in ASP.NET Core - https://learn.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection
- Microsoft Learn: Controller-based web APIs - https://learn.microsoft.com/en-us/aspnet/core/web-api
- Microsoft Learn: Unit testing best practices for .NET - https://learn.microsoft.com/en-us/dotnet/core/testing/unit-testing-best-practices
- Microsoft Learn: Integration tests in ASP.NET Core - https://learn.microsoft.com/en-us/aspnet/core/test/integration-tests
- Microsoft Learn: Common web application architectures / Clean Architecture - https://learn.microsoft.com/en-us/dotnet/architecture/modern-web-apps-azure/common-web-application-architectures
- Moq GitHub / Quickstart - https://github.com/devlooped/moq
- Awesome Assertions documentation - https://awesomeassertions.org/
