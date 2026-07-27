using Grasp.Core.Storage.Connectors;
using Grasp.Core.Storage.Connectors.Query;
using Microsoft.Extensions.Logging;
using Training.WorkItems.Application.WorkItems.Repositories;
using Training.WorkItems.Domain.WorkItems.Entities;
using Training.WorkItems.Domain.WorkItems.ValueTypes;

namespace Training.WorkItems.Infrastructure.WorkItems.Repositories;

public sealed class SqlWorkItemRepository(
    IAsyncEntityConnector<WorkItemStorageRecord> workItems,
    ILogger<SqlWorkItemRepository> logger) : IWorkItemRepository
{
    public async Task AddAsync(WorkItem workItem, CancellationToken cancellationToken)
    {
        try
        {
            await workItems.CreateAsync(workItem.ToStorageRecord(), cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "Repository failure creating work item {WorkItemId} for tenant {TenantId}.",
                workItem.Id.Value,
                workItem.TenantId.Value);
            throw;
        }
    }

    public async Task<WorkItem?> GetByIdAsync(
        TenantId tenantId,
        WorkItemId workItemId,
        CancellationToken cancellationToken)
    {
        try
        {
            var record = await workItems.FirstOrDefaultAsync(
                x => x.TenantId == tenantId.Value && x.WorkItemId == workItemId.Value,
                cancellationToken);

            return record?.ToDomain();
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "Repository failure fetching work item {WorkItemId} for tenant {TenantId}.",
                workItemId.Value,
                tenantId.Value);
            throw;
        }
    }

    public async Task<IReadOnlyCollection<WorkItem>> ListAsync(
        TenantId tenantId,
        CancellationToken cancellationToken)
    {
        try
        {
            var records = await workItems
                .Where(x => x.TenantId == tenantId.Value)
                .ToListAsync(cancellationToken);

            return records
                .Select(x => x.ToDomain())
                .ToArray();
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "Repository failure listing work items for tenant {TenantId}.",
                tenantId.Value);
            throw;
        }
    }

    public async Task UpdateAsync(WorkItem workItem, CancellationToken cancellationToken)
    {
        try
        {
            await workItems.UpdateAsync(workItem.ToStorageRecord(), cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "Repository failure updating work item {WorkItemId} for tenant {TenantId}.",
                workItem.Id.Value,
                workItem.TenantId.Value);
            throw;
        }
    }
}
