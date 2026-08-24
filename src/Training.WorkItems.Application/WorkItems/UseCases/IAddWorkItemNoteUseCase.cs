using Training.WorkItems.Application.Common;

namespace Training.WorkItems.Application.WorkItems.UseCases;

public interface IAddWorkItemNoteUseCase
{
    Task<ApplicationResult<AddWorkItemNoteResult>> ExecuteAsync(
        AddWorkItemNoteCommand command,
        CancellationToken cancellationToken);
}
