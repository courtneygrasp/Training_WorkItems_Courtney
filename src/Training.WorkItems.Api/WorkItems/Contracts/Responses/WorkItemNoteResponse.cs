using System.Text.Json.Serialization;

namespace Training.WorkItems.Api.WorkItems.Contracts.Responses;

public sealed record WorkItemNoteResponse(
    [property: JsonPropertyName("noteId")] Guid NoteId,
    [property: JsonPropertyName("workItemId")] Guid WorkItemId,
    [property: JsonPropertyName("noteText")] string NoteText,
    [property: JsonPropertyName("createdAt")] DateTimeOffset CreatedAt);
