using Grasp.Core.Storage.Connectors;
using Grasp.Core.Storage.Connectors.Query;
using Microsoft.Extensions.Logging;
using Training.WorkItems.Application.WorkItems.Repositories;
using Training.WorkItems.Domain.WorkItems.Entities;

namespace Training.WorkItems.Infrastructure.WorkItems.Repositories;

public sealed class SqlWorkItemStatusChangeRepository(
    IAsyncEntityConnector<WorkItemStorageRecord> workItems,
    IAsyncEntityConnector<WorkItemAuditStorageRecord> auditRecords,
    ILogger<SqlWorkItemStatusChangeRepository> logger) : IWorkItemStatusChangeRepository
{
    public async Task PersistAsync(
        WorkItem workItem,
        WorkItemAuditRecord auditRecord,
        CancellationToken cancellationToken)
    {
        try
        {
            await workItems.UpdateAsync(workItem.ToStorageRecord(), cancellationToken);
            await auditRecords.CreateAsync(auditRecord.ToStorageRecord(), cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "Repository failure persisting status change for work item {WorkItemId} in tenant {TenantId}.",
                workItem.Id.Value,
                workItem.TenantId.Value);
            throw;
        }
    }
}
