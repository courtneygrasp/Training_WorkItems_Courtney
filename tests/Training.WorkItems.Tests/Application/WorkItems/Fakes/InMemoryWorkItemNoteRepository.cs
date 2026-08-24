using Training.WorkItems.Application.WorkItems.Repositories;
using Training.WorkItems.Domain.WorkItems.Entities;

namespace Training.WorkItems.Tests.Application.WorkItems.Fakes;

public sealed class InMemoryWorkItemNoteRepository : IWorkItemNoteRepository
{
    private readonly List<WorkItemNote> _notes = [];

    public IReadOnlyCollection<WorkItemNote> Notes => _notes;

    public Task AddAsync(WorkItemNote note, CancellationToken cancellationToken)
    {
        _notes.Add(note);
        return Task.CompletedTask;
    }
}
