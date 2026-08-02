using Training.WorkItems.Domain.WorkItems.Entities;

namespace Training.WorkItems.Application.WorkItems.Repositories;

/// <summary>
/// Persists a status change and its audit record as a single durable operation.
/// The work item update and audit creation must commit together; implementations must
/// use a connector transaction or a transactional outbox to guarantee this.
/// </summary>
public interface IWorkItemStatusChangeRepository
{
    /// <summary>
    /// Persists the updated <paramref name="workItem"/> and the corresponding
    /// <paramref name="auditRecord"/> atomically.
    /// </summary>
    Task PersistAsync(
        WorkItem workItem,
        WorkItemAuditRecord auditRecord,
        CancellationToken cancellationToken);
}
