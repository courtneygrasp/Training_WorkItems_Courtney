using Grasp.Core.Storage.Connectors;
using Grasp.Core.Storage.Connectors.Query;
using Training.WorkItems.Application.WorkItems.Repositories;
using Training.WorkItems.Domain.WorkItems.Entities;
using Training.WorkItems.Domain.WorkItems.ValueTypes;

namespace Training.WorkItems.Infrastructure.WorkItems.Repositories;

public sealed class SqlWorkItemRepository(
    IAsyncEntityConnector<WorkItemStorageRecord> workItems) : IWorkItemRepository
{
    public async Task AddAsync(WorkItem workItem, CancellationToken cancellationToken)
    {
        await workItems.CreateAsync(workItem.ToStorageRecord(), cancellationToken);
    }

    public async Task<WorkItem?> GetByIdAsync(
        TenantId tenantId,
        WorkItemId workItemId,
        CancellationToken cancellationToken)
    {
        var record = await workItems.FirstOrDefaultAsync(
            x => x.TenantId == tenantId.Value && x.WorkItemId == workItemId.Value,
            cancellationToken);

        return record?.ToDomain();
    }

    public async Task<IReadOnlyCollection<WorkItem>> ListAsync(
        TenantId tenantId,
        CancellationToken cancellationToken)
    {
        var records = await workItems
            .Where(x => x.TenantId == tenantId.Value)
            .ToListAsync(cancellationToken);

        return records
            .Select(x => x.ToDomain())
            .ToArray();
    }

    public async Task UpdateAsync(WorkItem workItem, CancellationToken cancellationToken)
    {
        await workItems.UpdateAsync(workItem.ToStorageRecord(), cancellationToken);
    }
}
