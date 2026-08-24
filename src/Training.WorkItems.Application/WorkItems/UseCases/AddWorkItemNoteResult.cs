namespace Training.WorkItems.Application.WorkItems.UseCases;

/// <summary>Carries the outcome of a successful AddWorkItemNote operation.</summary>
public sealed record AddWorkItemNoteResult(
    Guid NoteId,
    Guid WorkItemId,
    string NoteText,
    DateTimeOffset CreatedAt);
