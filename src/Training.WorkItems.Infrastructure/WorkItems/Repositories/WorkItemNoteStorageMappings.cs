using Training.WorkItems.Domain.WorkItems.Entities;

namespace Training.WorkItems.Infrastructure.WorkItems.Repositories;

internal static class WorkItemNoteStorageMappings
{
    internal static WorkItemNoteStorageRecord ToStorageRecord(this WorkItemNote note) =>
        new()
        {
            NoteId = note.Id,
            WorkItemId = note.WorkItemId.Value,
            TenantId = note.TenantId.Value,
            AuthorId = note.AuthorId,
            NoteText = note.NoteText.Value,
            CreatedAt = note.CreatedAt
        };
}
