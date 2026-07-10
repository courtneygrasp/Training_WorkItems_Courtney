using Training.WorkItems.Domain.WorkItems.Entities;
using Training.WorkItems.Domain.WorkItems.Enums;
using Training.WorkItems.Domain.WorkItems.ValueTypes;

namespace Training.WorkItems.Application.WorkItems.Validation;

public sealed record ChangeWorkItemStatusValidationContext(
    WorkItem? WorkItem,
    TenantId CurrentTenantId,
    bool CanCloseWorkItems,
    WorkItemStatus RequestedStatus);
