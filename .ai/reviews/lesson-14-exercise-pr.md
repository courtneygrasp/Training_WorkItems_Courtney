---
type: exercise-pr
lesson: 14
feature: Add Work Item Note
status: flawed — reviewed; corrected implementation committed
---

# Exercise PR — Add Work Item Note (Flawed Submission)

This file documents the flawed pull-request diff used as the review target for Exercise 1.
The diff was never merged. The corrected implementation is in the main branch.

---

## PR Description

> Add a POST endpoint to attach a text note to an existing work item.

## Diff Summary

### New file: `src/Training.WorkItems.Api/WorkItems/Contracts/Requests/AddWorkItemNoteRequest.cs`

```csharp
namespace Training.WorkItems.Api.WorkItems.Contracts.Requests;

public sealed record AddWorkItemNoteRequest
{
    public string? Content { get; init; }
}
```

### Modified: `src/Training.WorkItems.Api/WorkItems/Controllers/WorkItemsController.cs`

Constructor change — `IConfiguration` added:

```diff
 public sealed class WorkItemsController(
     ICreateWorkItemUseCase createWorkItem,
     IChangeWorkItemStatusUseCase changeWorkItemStatus,
     IGetWorkItemByIdUseCase getWorkItemById,
-    IListWorkItemsUseCase listWorkItems) : ControllerBase
+    IListWorkItemsUseCase listWorkItems,
+    IConfiguration configuration) : ControllerBase
```

New action appended to the controller:

```csharp
[HttpPost("{id:guid}/notes")]
public async Task<IActionResult> AddNote(
    Guid id,
    [FromBody] AddWorkItemNoteRequest request,
    CancellationToken cancellationToken)
{
    // Parses tenant claim directly from JWT
    var tenantIdClaim = User.FindFirst("tenant_id")?.Value;
    if (string.IsNullOrEmpty(tenantIdClaim) || !Guid.TryParse(tenantIdClaim, out var tenantGuid))
        return Unauthorized();

    // Validates content length directly in controller
    if (string.IsNullOrWhiteSpace(request.Content))
        return BadRequest("Note content is required.");
    if (request.Content.Length > 2000)
        return BadRequest("Note content cannot exceed 2000 characters.");

    // Opens SQL connection directly in controller
    var connectionString = configuration.GetConnectionString("Default");
    using var connection = new SqlConnection(connectionString);
    await connection.OpenAsync(cancellationToken);

    // String-interpolated SQL — SQL injection risk on request.Content
    var sql = $"INSERT INTO dbo.WorkItemNotes (WorkItemId, TenantId, Content, CreatedAt) " +
              $"VALUES ('{id}', '{tenantGuid}', '{request.Content}', GETUTCDATE())";
    using var command = new SqlCommand(sql, connection);
    await command.ExecuteNonQueryAsync(cancellationToken);

    return Ok(new { Message = "Note added." });
}
```
