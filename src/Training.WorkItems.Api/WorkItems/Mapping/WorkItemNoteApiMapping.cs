using Training.WorkItems.Api.WorkItems.Contracts.Responses;
using Training.WorkItems.Application.WorkItems.UseCases;

namespace Training.WorkItems.Api.WorkItems.Mapping;

internal static class WorkItemNoteApiMapping
{
    internal static WorkItemNoteResponse ToApiResponse(this AddWorkItemNoteResult result) =>
        new(result.NoteId, result.WorkItemId, result.NoteText, result.CreatedAt);
}
