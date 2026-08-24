namespace Training.WorkItems.Application.WorkItems.UseCases;

public sealed record AddWorkItemNoteResult(
    Guid NoteId,
    Guid WorkItemId,
    string Content,
    DateTimeOffset CreatedAt);
