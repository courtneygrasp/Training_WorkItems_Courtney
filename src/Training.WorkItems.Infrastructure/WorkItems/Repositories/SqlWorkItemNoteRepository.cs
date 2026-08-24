using Grasp.Core.Storage.Connectors;
using Grasp.Core.Storage.Connectors.Query;
using Microsoft.Extensions.Logging;
using Training.WorkItems.Application.WorkItems.Repositories;
using Training.WorkItems.Domain.WorkItems.Entities;

namespace Training.WorkItems.Infrastructure.WorkItems.Repositories;

public sealed class SqlWorkItemNoteRepository(
    IAsyncEntityConnector<WorkItemNoteStorageRecord> notes,
    ILogger<SqlWorkItemNoteRepository> logger) : IWorkItemNoteRepository
{
    public async Task AddAsync(WorkItemNote note, CancellationToken cancellationToken)
    {
        try
        {
            await notes.CreateAsync(note.ToStorageRecord(), cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "Repository failure creating note {NoteId} for work item {WorkItemId} in tenant {TenantId}.",
                note.Id,
                note.WorkItemId.Value,
                note.TenantId.Value);
            throw;
        }
    }
}
