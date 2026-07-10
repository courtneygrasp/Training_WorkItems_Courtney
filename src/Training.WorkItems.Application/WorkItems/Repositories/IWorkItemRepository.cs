using Training.WorkItems.Domain.WorkItems.Entities;
using Training.WorkItems.Domain.WorkItems.ValueTypes;

namespace Training.WorkItems.Application.WorkItems.Repositories;

public interface IWorkItemRepository
{
    Task AddAsync(WorkItem workItem, CancellationToken cancellationToken);

    Task<WorkItem?> GetByIdAsync(
        TenantId tenantId,
        WorkItemId workItemId,
        CancellationToken cancellationToken);

    Task UpdateAsync(WorkItem workItem, CancellationToken cancellationToken);
}
