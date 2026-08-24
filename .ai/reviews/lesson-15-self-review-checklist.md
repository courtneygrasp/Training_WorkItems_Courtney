---
type: self-review-checklist
lesson: 15
feature: AddWorkItemNote (capstone)
---

# Self-Review Checklist — AddWorkItemNote Capstone

| Check | Evidence | Pass |
|---|---|---|
| Correct ownership | `WorkItemNoteText` + `WorkItemNote` in Domain; `IAddWorkItemNoteUseCase`/`AddWorkItemNoteUseCase`/`IWorkItemNoteRepository` in Application; `SqlWorkItemNoteRepository`/`WorkItemNoteStorageRecord` in Infrastructure; request/response/mapping/controller in Api | ✅ |
| Dependency direction | Api → Application (via `IAddWorkItemNoteUseCase`). Application → Domain (via `WorkItemNote`, `WorkItemNoteText`). Application never references Infrastructure. Infrastructure implements `IWorkItemNoteRepository`. | ✅ |
| API boundary | `AddWorkItemNoteRequest` uses `[Required]`/`[StringLength]`. `WorkItemNoteResponse` pins wire names with `[JsonPropertyName]`. Action returns `Task<ActionResult<WorkItemNoteResponse>>`. No business logic in controller. | ✅ |
| Application workflow | Use case validates text, checks work item ownership via `IWorkItemRepository.GetByIdAsync(tenantId, ...)`, creates `WorkItemNote`, saves, logs, returns result. Single responsibility. | ✅ |
| Domain discipline | `WorkItemNoteText` enforces the 2000-char invariant. `WorkItemNote` is a clean domain entity with no framework references. | ✅ |
| Infrastructure discipline | `SqlWorkItemNoteRepository` uses connector-backed persistence (`IAsyncEntityConnector<WorkItemNoteStorageRecord>`). No ad-hoc SQL. Errors logged before rethrow. | ✅ |
| Connector usage | `IAsyncEntityConnector<WorkItemNoteStorageRecord>` registered via `ConnectorFactory`. Consistent with `WorkItemStorageRecord` and `WorkItemAuditStorageRecord`. | ✅ |
| Validation mechanism | Request shape: `[Required]`/`[StringLength]` DataAnnotations. Domain invariant: `WorkItemNoteText.Create` (single input). No `IValidator<T>` needed (no multi-input cross-field rule). | ✅ |
| Tenant scoping | Use case reads `currentUser.TenantId` from `ICurrentUserContext`. `GetByIdAsync` filters by tenant — a foreign work item returns `NotFound()`. No claim parsing in controller. | ✅ |
| Security — input-to-sink | `request.Content` → `AddWorkItemNoteCommand.NoteText` → `WorkItemNoteText.Create` (validated, max 2000) → `WorkItemNote.NoteText` → `note.ToStorageRecord()` → connector (structural query). No string interpolation into SQL. | ✅ |
| Observability | Use case logs `Information` on success and on not-found. `SqlWorkItemNoteRepository` logs `Error` on exception before rethrow. | ✅ |
| Tests | `WorkItemNoteTextTests` (5), `AddWorkItemNoteUseCaseTests` (6), `WorkItemsControllerTests` +4, `WorkItemInfrastructureTests` +2. All pass. | ✅ |
| Database schema | `dbo.WorkItemNoteStorageRecord` with FK to `WorkItemStorageRecord`, `NVARCHAR(2000)` for `NoteText`, and a covering index on `(WorkItemId, TenantId)`. | ✅ |
| Postman coverage | `POST /api/workitems/{{workItemId}}/notes` request added to `postman/Training.WorkItems.postman_collection.json`. | ✅ |
| ARCHITECTURE.md | AddWorkItemNote feature flow documented. | ✅ |
