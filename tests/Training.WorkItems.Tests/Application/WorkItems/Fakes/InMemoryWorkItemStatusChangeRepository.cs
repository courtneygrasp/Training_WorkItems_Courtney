using Training.WorkItems.Application.WorkItems.Repositories;
using Training.WorkItems.Domain.WorkItems.Entities;

namespace Training.WorkItems.Tests.Application.WorkItems.Fakes;

public sealed class InMemoryWorkItemStatusChangeRepository : IWorkItemStatusChangeRepository
{
    private readonly List<WorkItemAuditRecord> _auditRecords = [];

    public IReadOnlyCollection<WorkItemAuditRecord> AuditRecords => _auditRecords;

    public Task PersistAsync(
        WorkItem workItem,
        WorkItemAuditRecord auditRecord,
        CancellationToken cancellationToken)
    {
        _auditRecords.Add(auditRecord);
        return Task.CompletedTask;
    }
}
