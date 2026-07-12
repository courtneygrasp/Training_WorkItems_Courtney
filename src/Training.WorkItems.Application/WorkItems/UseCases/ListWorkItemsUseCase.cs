using Training.WorkItems.Application.Common;
using Training.WorkItems.Application.WorkItems.Mapping;
using Training.WorkItems.Application.WorkItems.Repositories;
using Training.WorkItems.Application.WorkItems.Services;

namespace Training.WorkItems.Application.WorkItems.UseCases;

public sealed class ListWorkItemsUseCase(
    IWorkItemRepository workItems,
    ICurrentUserContext currentUser) : IListWorkItemsUseCase
{
    public async Task<ApplicationResult<IReadOnlyCollection<WorkItemResult>>> ExecuteAsync(
        CancellationToken cancellationToken)
    {
        var results = await workItems.ListAsync(currentUser.TenantId, cancellationToken);

        return ApplicationResult<IReadOnlyCollection<WorkItemResult>>
            .Success(results.Select(w => w.ToResult()).ToList());
    }
}
