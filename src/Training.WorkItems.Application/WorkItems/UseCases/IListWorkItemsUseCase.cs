using Training.WorkItems.Application.Common;

namespace Training.WorkItems.Application.WorkItems.UseCases;

public interface IListWorkItemsUseCase
{
    Task<ApplicationResult<IReadOnlyCollection<WorkItemResult>>> ExecuteAsync(
        CancellationToken cancellationToken);
}
