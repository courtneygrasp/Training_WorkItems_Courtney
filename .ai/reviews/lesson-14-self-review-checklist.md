---
type: self-review-checklist
lesson: 14
feature: Add Work Item Note (corrected implementation)
---

# Self-Review Checklist — Add Work Item Note (Corrected)

Reviewed against the corrected implementation committed to the lesson-14 branch.

| Check | Evidence | Pass |
|---|---|---|
| Correct ownership | `AddWorkItemNoteUseCase` in Core.Application; `NoteContent` value type in Core.Domain; `AddWorkItemNoteRequest` with DataAnnotations in Api; Infrastructure SQL implementation deferred to lesson 15 | ✅ |
| Dependency direction | Controller → `IAddWorkItemNoteUseCase` (Application interface). Use case stub does not reference Infrastructure. Core.Domain has no framework or storage references. | ✅ |
| API boundary | Request/response contracts (`AddWorkItemNoteRequest`, `WorkItemNoteResponse`) are in Api. HTTP mapping (`CreatedAtAction`, `ToActionResult`) is in the controller. No business logic in the controller. | ✅ |
| Application workflow | Controller maps request → `AddWorkItemNoteCommand` and delegates to `IAddWorkItemNoteUseCase`. The use case (stub) is responsible for validation, existence check, and persistence orchestration. | ✅ |
| Domain discipline | `NoteContent` enforces the 2000-character invariant without framework or storage dependencies. | ✅ |
| Validation mechanism | Request-shape validation uses `[Required]` and `[StringLength]` DataAnnotations on `AddWorkItemNoteRequest`. Domain invariant enforced by `NoteContent.Create`. Application-level cross-input validation would use `IValidator<T>` if warranted. | ✅ |
| Tenant scoping | The use case will receive `ICurrentUserContext` (lesson 15 implementation). No direct claim parsing in the controller. | ✅ |
| Security — input-to-sink | `request.Content` flows to `NoteContent.Create` (validates length), then into the command, then to the use case. No string interpolation into SQL. Infrastructure connector (lesson 15) composes queries structurally. | ✅ |
| Observability | Logging deferred to lesson 15 full implementation; stub logs nothing, which is acceptable for a `NotImplementedException` stub. | ⚠️ Add logging in lesson 15. |
| Postman coverage | Offer to add `POST /api/workitems/{id}/notes` to the Postman collection after the lesson 15 implementation is complete and confirmed. | ⏳ Deferred to lesson 15. |
| Tests | Unit tests deferred to lesson 15 full implementation. No test coverage for the stub is required since it throws unconditionally. | ⏳ Deferred to lesson 15. |
| Naming/maintainability | Files follow existing naming conventions (`Add` prefix, `Command`/`Result`/`UseCase`/`IUseCase` suffixes). `NoteContent` mirrors `WorkItemTitle`. | ✅ |
