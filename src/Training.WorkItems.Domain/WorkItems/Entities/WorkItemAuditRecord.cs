using Training.WorkItems.Domain.WorkItems.Enums;
using Training.WorkItems.Domain.WorkItems.ValueTypes;

namespace Training.WorkItems.Domain.WorkItems.Entities;

/// <summary>Records a status change made to a work item for audit purposes.</summary>
public sealed class WorkItemAuditRecord
{
    private WorkItemAuditRecord(
        Guid id,
        WorkItemId workItemId,
        TenantId tenantId,
        Guid userId,
        WorkItemStatus status,
        DateTimeOffset changedAt)
    {
        Id = id;
        WorkItemId = workItemId;
        TenantId = tenantId;
        UserId = userId;
        Status = status;
        ChangedAt = changedAt;
    }

    /// <summary>Gets the unique identifier for this audit record.</summary>
    public Guid Id { get; }

    /// <summary>Gets the work item this audit record belongs to.</summary>
    public WorkItemId WorkItemId { get; }

    /// <summary>Gets the tenant that owns the work item.</summary>
    public TenantId TenantId { get; }

    /// <summary>Gets the ID of the user who changed the status.</summary>
    public Guid UserId { get; }

    /// <summary>Gets the status the work item was changed to.</summary>
    public WorkItemStatus Status { get; }

    /// <summary>Gets the UTC time the status change was recorded.</summary>
    public DateTimeOffset ChangedAt { get; }

    /// <summary>Creates a new audit record for the given status change.</summary>
    public static WorkItemAuditRecord Create(
        WorkItemId workItemId,
        TenantId tenantId,
        Guid userId,
        WorkItemStatus status)
    {
        return new WorkItemAuditRecord(
            Guid.NewGuid(),
            workItemId,
            tenantId,
            userId,
            status,
            DateTimeOffset.UtcNow);
    }
}
