using Training.WorkItems.Application.Common;

namespace Training.WorkItems.Application.WorkItems.UseCases;

public interface IChangeWorkItemStatusUseCase
{
    Task<ApplicationResult<WorkItemResult>> ExecuteAsync(
        ChangeWorkItemStatusCommand command,
        CancellationToken cancellationToken);
}
