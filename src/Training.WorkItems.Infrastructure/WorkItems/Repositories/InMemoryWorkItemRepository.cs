using Training.WorkItems.Application.WorkItems.Repositories;
using Training.WorkItems.Domain.WorkItems.Entities;
using Training.WorkItems.Domain.WorkItems.ValueTypes;

namespace Training.WorkItems.Infrastructure.WorkItems.Repositories;

public sealed class InMemoryWorkItemRepository : IWorkItemRepository
{
    private readonly List<WorkItem> _items = [];

    public Task AddAsync(WorkItem workItem, CancellationToken cancellationToken)
    {
        _items.Add(workItem);

        return Task.CompletedTask;
    }

    public Task<WorkItem?> GetByIdAsync(
        TenantId tenantId,
        WorkItemId workItemId,
        CancellationToken cancellationToken)
    {
        var workItem = _items.SingleOrDefault(x => x.TenantId == tenantId && x.Id == workItemId);

        return Task.FromResult(workItem);
    }

    public Task UpdateAsync(WorkItem workItem, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
