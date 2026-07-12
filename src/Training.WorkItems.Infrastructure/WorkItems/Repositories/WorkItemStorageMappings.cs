using Training.WorkItems.Domain.WorkItems.Entities;
using Training.WorkItems.Domain.WorkItems.Enums;
using Training.WorkItems.Domain.WorkItems.ValueTypes;

namespace Training.WorkItems.Infrastructure.WorkItems.Repositories;

internal static class WorkItemStorageMappings
{
    internal static WorkItemStorageRecord ToStorageRecord(this WorkItem workItem) =>
        new()
        {
            WorkItemId = workItem.Id.Value,
            TenantId = workItem.TenantId.Value,
            Title = workItem.Title.Value,
            Description = workItem.Description,
            Status = workItem.Status.ToString(),
            CreatedAt = workItem.CreatedAt
        };

    internal static WorkItem ToDomain(this WorkItemStorageRecord record) =>
        WorkItem.Rehydrate(
            WorkItemId.Create(record.WorkItemId),
            TenantId.Create(record.TenantId),
            WorkItemTitle.Create(record.Title),
            record.Description,
            Enum.Parse<WorkItemStatus>(record.Status),
            record.CreatedAt);
}
