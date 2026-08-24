namespace Training.WorkItems.Application.WorkItems.UseCases;

public sealed record AddWorkItemNoteCommand(Guid WorkItemId, string Content);
