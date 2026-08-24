namespace Training.WorkItems.Application.WorkItems.UseCases;

/// <summary>Command to add a text note to a work item.</summary>
public sealed record AddWorkItemNoteCommand(Guid WorkItemId, string NoteText);
