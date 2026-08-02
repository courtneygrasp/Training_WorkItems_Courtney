using Training.WorkItems.Domain.WorkItems.Entities;
using Training.WorkItems.Domain.WorkItems.Enums;
using Training.WorkItems.Domain.WorkItems.ValueTypes;

namespace Training.WorkItems.Application.WorkItems.Validation;

/// <summary>
/// Provides the inputs needed to validate cross-input workflow rules for a status change request.
/// The work item is guaranteed non-null; null-check and tenant-scope are the use case's responsibility.
/// </summary>
public sealed record ChangeWorkItemStatusValidationContext(
    WorkItem WorkItem,
    TenantId CurrentTenantId,
    bool CanCloseWorkItems,
    WorkItemStatus RequestedStatus);
