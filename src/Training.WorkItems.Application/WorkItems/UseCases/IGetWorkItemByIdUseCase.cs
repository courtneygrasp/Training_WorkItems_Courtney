using Training.WorkItems.Application.Common;

namespace Training.WorkItems.Application.WorkItems.UseCases;

public interface IGetWorkItemByIdUseCase
{
    Task<ApplicationResult<WorkItemResult>> ExecuteAsync(
        GetWorkItemByIdQuery query,
        CancellationToken cancellationToken);
}
