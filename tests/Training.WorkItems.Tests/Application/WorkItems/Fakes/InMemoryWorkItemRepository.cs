using Training.WorkItems.Application.WorkItems.Repositories;
using Training.WorkItems.Domain.WorkItems.Entities;
using Training.WorkItems.Domain.WorkItems.ValueTypes;

namespace Training.WorkItems.Tests.Application.WorkItems.Fakes;

public sealed class InMemoryWorkItemRepository : IWorkItemRepository
{
    private readonly List<WorkItem> _items = [];

    public IReadOnlyCollection<WorkItem> Items => _items;

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
        var result = _items.SingleOrDefault(x => x.TenantId == tenantId && x.Id == workItemId);
        return Task.FromResult(result);
    }

    public Task<IReadOnlyCollection<WorkItem>> ListAsync(
        TenantId tenantId,
        CancellationToken cancellationToken)
    {
        IReadOnlyCollection<WorkItem> result = _items
            .Where(x => x.TenantId == tenantId)
            .ToArray();
        return Task.FromResult(result);
    }

    public Task UpdateAsync(WorkItem workItem, CancellationToken cancellationToken) =>
        Task.CompletedTask;
}
