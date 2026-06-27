using Training.WorkItems.Application.Common;

namespace Training.WorkItems.Application.WorkItems.UseCases;

public interface ICreateWorkItemUseCase
{
    Task<ApplicationResult<WorkItemResult>> ExecuteAsync(
        CreateWorkItemCommand command,
        CancellationToken cancellationToken);
}
