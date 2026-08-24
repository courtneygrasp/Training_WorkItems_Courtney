---
type: review-staging-report
lesson: 14
feature: Add Work Item Note
verdict: REQUEST CHANGES
reviewed-head: exercise-only (see lesson-14-exercise-pr.md — not merged)
---

# Staging Report — Add Work Item Note

## Verdict

**REQUEST CHANGES** — 2 blockers present; do not merge until both are resolved.

---

## Scope

- **Merge base:** `master` HEAD
- **Reviewed HEAD:** Exercise PR (documented in `.ai/reviews/lesson-14-exercise-pr.md`; not committed to branch)
- **Files in diff:** `WorkItemsController.cs`, `AddWorkItemNoteRequest.cs`
- **Work item:** Not linked — Acceptance-Criteria traceability not performed.

---

## Input-to-Sink Trace

| Caller input | Type | Path | Sink |
|---|---|---|---|
| `id` (URL route) | `Guid` | Direct string interpolation into SQL | `VALUES ('{id}', ...)` |
| `tenantGuid` (JWT claim parsed to Guid) | `Guid` | Direct string interpolation into SQL | `VALUES (..., '{tenantGuid}', ...)` |
| `request.Content` (request body) | `string?` | Direct string interpolation into SQL | `VALUES (..., '{request.Content}', ...)` |

`id` and `tenantGuid` are `Guid` values; a valid GUID contains only hex digits and hyphens and cannot introduce SQL syntax.
`request.Content` is an unconstrained string. Arbitrary SQL syntax in its value becomes part of the INSERT statement — this is the injection vector for BL1.

The work item identified by `id` is never read before the INSERT. A caller who supplies any `id` (including a foreign or non-existent one) can write a note row — the authorization and existence path for BL2.

---

## Findings

### [BL1] Controller interpolates `request.Content` into SQL

**Location:** `WorkItemsController.cs` — AddNote action, SQL string construction
**Issue:** `request.Content` from the request body is concatenated into the SQL INSERT using string interpolation. A caller who submits content containing `'`, `--`, or a semicolon can close the string literal and inject additional SQL statements, enabling arbitrary reads from or writes to the database.
**Recommendation:** Move the INSERT behind an `IWorkItemNoteRepository` abstraction in Core.Application. Implement it in Core.Infrastructure using `IAsyncEntityConnector<WorkItemNoteStorageRecord>`, which composes queries structurally and cannot accept raw SQL fragments from caller input.

---

### [BL2] No work item existence or tenant ownership check before inserting

**Location:** `WorkItemsController.cs` — AddNote action
**Issue:** The action inserts a note for any caller-supplied `id` without first verifying that a work item with that ID exists under the caller's tenant. A caller who knows or guesses a foreign work item ID can attach notes to it. The note's `TenantId` is set from the caller's own claim (not the target work item's tenant), so the note row is not cross-tenant — but the operation reveals whether any work item ID exists in the system and produces orphaned note rows for non-existent IDs.
**Recommendation:** In the use case, call `IWorkItemRepository.GetByIdAsync(tenantId, workItemId)` before inserting. Return `ApplicationResult.NotFound()` if no matching record is found for the caller's tenant. This closes the cross-tenant inference path and prevents orphaned rows.

---

### [MJ1] SQL connection opened directly in controller

**Location:** `WorkItemsController.cs` — AddNote action, `new SqlConnection(...)`
**Issue:** Storage technology selection, connection string reading, and connection lifecycle belong in Core.Infrastructure. A controller that manages a SQL connection is doing infrastructure work. Future storage changes require a controller change instead of an infrastructure change.
**Recommendation:** Introduce `IWorkItemNoteRepository` in Core.Application. Implement `SqlWorkItemNoteRepository` in Core.Infrastructure. Inject the interface into the use case.

---

### [MJ2] No repository interface in Core.Application

**Location:** (missing class)
**Issue:** There is no `IWorkItemNoteRepository` in Core.Application. Core.Application cannot describe what storage it needs, and the storage access is taken directly from the controller, violating the dependency direction rule.
**Recommendation:** Create `IWorkItemNoteRepository` in `Core.Application/WorkItems/Repositories/`.

---

### [MJ3] No use case in Core.Application

**Location:** (missing class)
**Issue:** The entire workflow — parse caller identity, validate content, check work item ownership, persist the note, return a result — runs in the controller action. Application-layer orchestration is absent. The workflow cannot be unit-tested without an HTTP context.
**Recommendation:** Create `AddWorkItemNoteUseCase` in Core.Application implementing `IAddWorkItemNoteUseCase`. The controller action should map the request to `AddWorkItemNoteCommand` and delegate to the use case, consistent with every other action in this controller.

---

### [MJ4] Tenant ID parsed from JWT claim directly in controller

**Location:** `WorkItemsController.cs` — AddNote action, `User.FindFirst("tenant_id")`
**Issue:** `User.FindFirst("tenant_id")` is called in the action body, hard-coding the claim key and the parse fallback inline. The established abstraction for caller identity is `ICurrentUserContext`. Two locations now own the same concern. A future claim schema change requires finding every `FindFirst("tenant_id")` call site rather than updating `HttpContextCurrentUserContext` once.
**Recommendation:** Inject `ICurrentUserContext` into the use case and read `TenantId` from it, consistent with `CreateWorkItemUseCase` and `ChangeWorkItemStatusUseCase`.

---

### [MJ5] Note content validated as raw string in controller; no domain value type

**Location:** `WorkItemsController.cs` — AddNote action, inline length check
**Issue:** The 2000-character maximum is enforced by an inline `if` in the controller. This is a domain invariant. When a second path to note creation exists (import, webhook, admin API) the controller-level guard is bypassed. Domain invariants must live in a value type so they cannot be bypassed regardless of entry point.
**Recommendation:** Create `NoteContent` value type in Core.Domain following the pattern of `WorkItemTitle`. Construct it in the use case; return `ApplicationResult.Invalid()` on violation.

---

### [MJ6] No structured log event for note creation

**Location:** (missing)
**Issue:** No log entry is emitted on success or failure. Every other use case in this solution logs at `Information` level on success with structured fields (`WorkItemId`, `TenantId`). The omission leaves an observability gap — a note flood or unexpected error is invisible in the logs.
**Recommendation:** Log at `Information` level in the use case on success, following the pattern in `CreateWorkItemUseCase` and `ChangeWorkItemStatusUseCase`.

---

### [MJ7] No test coverage for the endpoint or its workflow

**Location:** (tests project)
**Issue:** No unit test covers the happy path, the not-found path, the ownership-check path, or validation failures. The workflow logic and result mapping have no coverage.
**Recommendation:** Add unit tests for `AddWorkItemNoteUseCase` covering the not-found path, the success path, and the invalid-content path, using fakes consistent with the existing test infrastructure.

---

### [MN1] Response is an anonymous object

**Location:** `WorkItemsController.cs` — AddNote action, `Ok(new { Message = "Note added." })`
**Issue:** An anonymous object is returned from the action. API response contracts belong to the Api project as named record types so they are discoverable, versioned, and testable. An anonymous `{ Message }` leaks no note identity back to the caller.
**Recommendation:** Create `WorkItemNoteResponse` in `WorkItems/Contracts/Responses/` with the fields callers need (note ID, work item ID, content, timestamp). Return it from the action.

---

### [MN2] `AddWorkItemNoteRequest` lacks DataAnnotations

**Location:** `AddWorkItemNoteRequest.cs`
**Issue:** `Content` is declared `string?` with no `[Required]` or `[StringLength]` attributes. Model binding silently accepts null; the only guard is an inline null/whitespace check in the action body. The pattern for all other request DTOs in this project is to use DataAnnotations for request-shape validation at the API boundary.
**Recommendation:** Add `[Required(AllowEmptyStrings = false)]` and `[StringLength(2000)]` to `Content` and make it `required string`, consistent with `CreateWorkItemRequest`.

---

### [MN3] `IConfiguration` injected into `WorkItemsController`

**Location:** `WorkItemsController.cs` — constructor
**Issue:** The controller now takes `IConfiguration` to look up a connection string. Configuration-backed infrastructure services belong in Core.Infrastructure. A controller that reads connection strings directly is taking on an infrastructure concern — it reveals the storage mechanism to the API layer.
**Recommendation:** Remove `IConfiguration` from the controller. Connection string access belongs in `SqlWorkItemNoteRepository` registered in Core.Infrastructure. This finding is resolved by fixing MJ1.

---

### [NT1] Claim key literal `"tenant_id"` in action body

**Location:** `WorkItemsController.cs` — AddNote action
**Note:** The string literal `"tenant_id"` is duplicated if other actions parse claims directly. This nit is superseded by MJ4; fix MJ4 and NT1 disappears.

---

## Context-Lane Changes

None in this PR.

---

## Acceptance-Criteria Traceability

Not performed — no work item was linked to this PR.

---

## Checklist Coverage

| Check | Status | Notes |
|---|---|---|
| Correct ownership | ❌ | SQL in controller; no use case; no repository interface |
| Dependency direction | ❌ | Controller reaches Infrastructure directly; Application layer absent |
| Review format | ✅ | Merge-base diff used; findings use severity-coded stable IDs |
| Finding quality | ✅ | Each finding names location, cause, effect, consequence, and recommendation |
| Coverage | ✅ | Validation, security trace, tenant scope, connector, tests, Postman, QA hand-off all assessed |
| Practical maintainability | ❌ | Business logic and connection management in controller |
| Testability | ❌ | No use case; workflow cannot be unit-tested without HTTP context |
| Explanation | ❌ | No coherent explanation for why SQL belongs in the controller |

---

## Hand-offs

**Postman collection:** `POST /api/workitems/{id}/notes` is a new HTTP-testable endpoint. Once the corrected implementation is merged and confirmed, offer to add a request to `postman/Training.WorkItems.postman_collection.json`. _Not added without reviewer confirmation._

**QA notes:** Not applicable — this is not a Bug or User Story work item type.

---

## Validation

Self-review checklist filed in `.ai/reviews/lesson-14-self-review-checklist.md`.
