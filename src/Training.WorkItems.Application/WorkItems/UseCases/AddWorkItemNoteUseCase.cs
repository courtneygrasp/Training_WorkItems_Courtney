using Training.WorkItems.Application.Common;
using Training.WorkItems.Domain.Common;

namespace Training.WorkItems.Application.WorkItems.UseCases;

public sealed class AddWorkItemNoteUseCase : IAddWorkItemNoteUseCase
{
    public Task<ApplicationResult<AddWorkItemNoteResult>> ExecuteAsync(
        AddWorkItemNoteCommand command,
        CancellationToken cancellationToken)
    {
        Guard.NotNull(command);

        // Full implementation — repository, domain entity, and logging — is the lesson 15 capstone.
        throw new NotImplementedException("AddWorkItemNote storage implementation deferred to lesson 15.");
    }
}
