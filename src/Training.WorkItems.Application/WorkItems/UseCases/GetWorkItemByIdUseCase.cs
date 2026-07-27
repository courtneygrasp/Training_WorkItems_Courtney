using Microsoft.Extensions.Logging;
using Training.WorkItems.Application.Common;
using Training.WorkItems.Application.WorkItems.Mapping;
using Training.WorkItems.Application.WorkItems.Repositories;
using Training.WorkItems.Application.WorkItems.Services;
using Training.WorkItems.Domain.Common;
using Training.WorkItems.Domain.WorkItems.ValueTypes;

namespace Training.WorkItems.Application.WorkItems.UseCases;

public sealed class GetWorkItemByIdUseCase(
    IWorkItemRepository workItems,
    ICurrentUserContext currentUser,
    ILogger<GetWorkItemByIdUseCase> logger) : IGetWorkItemByIdUseCase
{
    public async Task<ApplicationResult<WorkItemResult>> ExecuteAsync(
        GetWorkItemByIdQuery query,
        CancellationToken cancellationToken)
    {
        Guard.NotNull(query);

        var workItemId = WorkItemId.Create(query.WorkItemId);
        var workItem = await workItems.GetByIdAsync(currentUser.TenantId, workItemId, cancellationToken);

        if (workItem is null)
        {
            logger.LogInformation(
                "Work item {WorkItemId} not found for tenant {TenantId}.",
                workItemId.Value,
                currentUser.TenantId.Value);

            return ApplicationResult<WorkItemResult>.NotFound();
        }

        return ApplicationResult<WorkItemResult>.Success(workItem.ToResult());
    }
}
