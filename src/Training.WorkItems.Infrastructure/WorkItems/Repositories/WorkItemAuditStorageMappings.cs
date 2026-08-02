using Training.WorkItems.Domain.WorkItems.Entities;

namespace Training.WorkItems.Infrastructure.WorkItems.Repositories;

internal static class WorkItemAuditStorageMappings
{
    internal static WorkItemAuditStorageRecord ToStorageRecord(this WorkItemAuditRecord auditRecord) =>
        new()
        {
            AuditId = auditRecord.Id,
            WorkItemId = auditRecord.WorkItemId.Value,
            TenantId = auditRecord.TenantId.Value,
            UserId = auditRecord.UserId,
            Status = auditRecord.Status.ToString(),
            ChangedAt = auditRecord.ChangedAt
        };
}
