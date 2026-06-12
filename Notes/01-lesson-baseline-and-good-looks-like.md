# 01 - Baseline and What Good Looks Like

## Lesson Purpose

Establish a fair baseline for each engineer and create a shared picture of modern .NET expectations.

This lesson is diagnostic. The goal is not to make anyone feel cornered. The goal is to identify which skills need deliberate practice before the project work begins.


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

- Identify common layering violations in an ASP.NET Core endpoint.
- Explain why controller-heavy code becomes difficult to test and change.
- Recognize hard-coded dependencies and hidden coupling.
- Identify SQL injection risk and unsafe data access patterns.
- Explain why tenant access should be explicit and consistent.
- Describe what "good" looks like in this learning project.

## Scenario

The engineer receives a deliberately flawed endpoint:

```csharp
[ApiController]
[Route("api/work-items")]
public sealed class WorkItemsController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public WorkItemsController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpPost]
    public async Task<IActionResult> CreateWorkItem(CreateWorkItemRequest request)
    {
        var tenantClaim = User.Claims.FirstOrDefault(x => x.Type == "tenant_id")?.Value;
        if (string.IsNullOrWhiteSpace(tenantClaim))
        {
            return Unauthorized();
        }

        if (string.IsNullOrWhiteSpace(request.Title))
        {
            return BadRequest("Title is required.");
        }

        using var connection = new SqlConnection(_configuration["ConnectionStrings:Default"]);
        await connection.OpenAsync();

        var sql = $"INSERT INTO WorkItem (Id, TenantId, Title, Description, Status) VALUES ('{Guid.NewGuid()}', '{tenantClaim}', '{request.Title}', '{request.Description}', 'New')";

        await connection.ExecuteAsync(sql);

        return Ok();
    }
}
```

## Problems To Identify

The engineer should identify at least these issues:

| Issue | Why It Matters |
|---|---|
| SQL in controller | Couples HTTP handling to persistence |
| String-interpolated SQL | Creates injection risk and fragile SQL |
| Claim parsing in endpoint | Repeats auth mechanics everywhere |
| Validation in controller | Makes rules inconsistent and hard to test |
| No application service | No clear use-case boundary |
| No domain model | Business rules have nowhere to live |
| No abstraction for data access | Hard to test without a database |
| No cancellation token | Poor async API hygiene |
| No logging/correlation | Weak operational visibility |
| No result model | Error handling becomes ad hoc |
| No tenant-safe repository method | Cross-tenant access risk |

## What Good Looks Like

A better implementation has a thin controller:

```csharp
[HttpPost]
public async Task<ActionResult<WorkItemDto>> CreateAsync(
    CreateWorkItemRequest request,
    CancellationToken cancellationToken)
{
    var command = new CreateWorkItemCommand(
        request.Title,
        request.Description,
        request.DueDate);

    var result = await _workItemService.CreateAsync(command, cancellationToken);

    return result.ToActionResult();
}
```

The controller maps HTTP to application input and application output back to HTTP. It does not decide business rules, query SQL, or parse tenant claims directly.

## Discussion Prompts

Use these prompts during the mentoring session:

1. What would happen if we added status transition rules to the flawed endpoint?
2. How would we test the flawed endpoint without a database?
3. Where should tenant ID come from?
4. Where should SQL live?
5. Where should the rule "title is required" live?
6. What part of this code would be hardest to change safely?

## Baseline Exercise

Each engineer should submit a short review document with:

- At least 8 identified issues.
- A proposed layer for each responsibility.
- A short explanation of how they would refactor it.
- One example test they would want to write after refactoring.

## Measurement Rubric

| Score | Evidence |
|---:|---|
| 1 | Identifies only formatting or superficial issues |
| 2 | Identifies some code smells but cannot map them to layers |
| 3 | Identifies most core issues and proposes reasonable separation |
| 4 | Identifies security, testability, layering, and tenant risks clearly |
| 5 | Explains tradeoffs and proposes an incremental refactor path |

## Instructor Notes

Watch for engineers who say, "It works, so it is fine." That is the central habit this program is designed to repair.

Also watch for overcorrection. The goal is not to create fourteen abstractions for one endpoint. The goal is clear boundaries, explicit dependencies, and testable behavior.

## Deliverable

Create a file in the repo:

```text
/docs/lesson-01-baseline-review.md
```

The file should contain the engineer's review of the flawed endpoint and their proposed refactor direction.


## References

- Microsoft Learn: Dependency injection in .NET - https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection/overview
- Microsoft Learn: Dependency injection in ASP.NET Core - https://learn.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection
- Microsoft Learn: Controller-based web APIs - https://learn.microsoft.com/en-us/aspnet/core/web-api
- Microsoft Learn: Unit testing best practices for .NET - https://learn.microsoft.com/en-us/dotnet/core/testing/unit-testing-best-practices
- Microsoft Learn: Integration tests in ASP.NET Core - https://learn.microsoft.com/en-us/aspnet/core/test/integration-tests
- Microsoft Learn: Common web application architectures / Clean Architecture - https://learn.microsoft.com/en-us/dotnet/architecture/modern-web-apps-azure/common-web-application-architectures
- Moq GitHub / Quickstart - https://github.com/devlooped/moq
- Awesome Assertions documentation - https://awesomeassertions.org/
