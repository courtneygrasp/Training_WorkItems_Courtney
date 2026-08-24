namespace Training.WorkItems.Api.WorkItems.Contracts.Responses;

public sealed record WorkItemNoteResponse(
    Guid NoteId,
    Guid WorkItemId,
    string Content,
    DateTimeOffset CreatedAt);
