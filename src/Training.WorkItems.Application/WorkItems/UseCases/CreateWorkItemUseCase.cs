using Training.WorkItems.Application.Common;
using Training.WorkItems.Application.WorkItems.Mapping;
using Training.WorkItems.Application.WorkItems.Repositories;
using Training.WorkItems.Application.WorkItems.Services;
using Training.WorkItems.Domain.WorkItems.Entities;
using Training.WorkItems.Domain.WorkItems.ValueTypes;

namespace Training.WorkItems.Application.WorkItems.UseCases;

public sealed class CreateWorkItemUseCase(
    IWorkItemRepository workItems,
    ICurrentUserContext currentUser,
    ISystemClock clock) : ICreateWorkItemUseCase
{
    public async Task<ApplicationResult<WorkItemResult>> ExecuteAsync(
        CreateWorkItemCommand command,
        CancellationToken cancellationToken)
    {
        WorkItemTitle title;

        try
        {
            title = WorkItemTitle.Create(command.Title);
        }
        catch (ArgumentException ex)
        {
            return ApplicationResult<WorkItemResult>.Invalid(ex.Message);
        }

        var workItem = WorkItem.Create(
            WorkItemId.Create(Guid.NewGuid()),
            currentUser.TenantId,
            title,
            command.Description,
            clock.UtcNow);

        await workItems.AddAsync(workItem, cancellationToken);

        return ApplicationResult<WorkItemResult>.Success(workItem.ToResult());
    }
}
