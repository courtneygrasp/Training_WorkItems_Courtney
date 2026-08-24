using System.ComponentModel.DataAnnotations;

namespace Training.WorkItems.Infrastructure.WorkItems.Repositories;

public sealed record WorkItemNoteStorageRecord
{
    [Key]
    public required Guid NoteId { get; init; }
    public required Guid WorkItemId { get; init; }
    public required Guid TenantId { get; init; }
    public required Guid AuthorId { get; init; }
    public required string NoteText { get; init; }
    public required DateTimeOffset CreatedAt { get; init; }
}
