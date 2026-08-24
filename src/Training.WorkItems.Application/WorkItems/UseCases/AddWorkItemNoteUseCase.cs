using Microsoft.Extensions.Logging;
using Training.WorkItems.Application.Common;
using Training.WorkItems.Application.WorkItems.Repositories;
using Training.WorkItems.Application.WorkItems.Services;
using Training.WorkItems.Domain.Common;
using Training.WorkItems.Domain.WorkItems.Entities;
using Training.WorkItems.Domain.WorkItems.ValueTypes;

namespace Training.WorkItems.Application.WorkItems.UseCases;

public sealed class AddWorkItemNoteUseCase(
    IWorkItemRepository workItems,
    IWorkItemNoteRepository notes,
    ICurrentUserContext currentUser,
    ISystemClock clock,
    ILogger<AddWorkItemNoteUseCase> logger) : IAddWorkItemNoteUseCase
{
    public async Task<ApplicationResult<AddWorkItemNoteResult>> ExecuteAsync(
        AddWorkItemNoteCommand command,
        CancellationToken cancellationToken)
    {
        Guard.NotNull(command);

        WorkItemNoteText noteText;

        try
        {
            noteText = WorkItemNoteText.Create(command.NoteText);
        }
        catch (ArgumentException ex)
        {
            return ApplicationResult<AddWorkItemNoteResult>.Invalid(ex.Message);
        }

        var tenantId = currentUser.TenantId;
        var workItemId = WorkItemId.Create(command.WorkItemId);

        var workItem = await workItems.GetByIdAsync(tenantId, workItemId, cancellationToken);

        if (workItem is null)
        {
            logger.LogInformation(
                "Work item {WorkItemId} was not found for tenant {TenantId} while adding a note.",
                workItemId.Value,
                tenantId.Value);

            return ApplicationResult<AddWorkItemNoteResult>.NotFound();
        }

        var note = WorkItemNote.Create(
            workItemId,
            tenantId,
            currentUser.UserId,
            noteText,
            clock.UtcNow);

        await notes.AddAsync(note, cancellationToken);

        logger.LogInformation(
            "Note {NoteId} added to work item {WorkItemId} for tenant {TenantId}.",
            note.Id,
            note.WorkItemId.Value,
            note.TenantId.Value);

        return ApplicationResult<AddWorkItemNoteResult>.Success(
            new AddWorkItemNoteResult(note.Id, note.WorkItemId.Value, note.NoteText.Value, note.CreatedAt));
    }
}
