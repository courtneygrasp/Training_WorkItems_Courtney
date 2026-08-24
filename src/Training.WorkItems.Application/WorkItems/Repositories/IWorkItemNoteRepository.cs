using Training.WorkItems.Domain.WorkItems.Entities;

namespace Training.WorkItems.Application.WorkItems.Repositories;

/// <summary>Persistence abstraction for work item notes.</summary>
public interface IWorkItemNoteRepository
{
    /// <summary>Persists a new note.</summary>
    Task AddAsync(WorkItemNote note, CancellationToken cancellationToken);
}
