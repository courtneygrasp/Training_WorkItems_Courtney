using Training.WorkItems.Application.Common;
using Training.WorkItems.Application.WorkItems.Mapping;
using Training.WorkItems.Application.WorkItems.Repositories;
using Training.WorkItems.Application.WorkItems.Services;
using Training.WorkItems.Application.WorkItems.Validation;
using Training.WorkItems.Domain.Common;
using Training.WorkItems.Domain.Common.Validation;
using Training.WorkItems.Domain.WorkItems.Services;
using Training.WorkItems.Domain.WorkItems.ValueTypes;

namespace Training.WorkItems.Application.WorkItems.UseCases;

public sealed class ChangeWorkItemStatusUseCase(
    IWorkItemRepository workItems,
    ICurrentUserContext currentUser,
    IWorkItemStatusPolicy statusPolicy,
    IValidator<ChangeWorkItemStatusValidationContext> validator) : IChangeWorkItemStatusUseCase
{
    public async Task<ApplicationResult<WorkItemResult>> ExecuteAsync(
        ChangeWorkItemStatusCommand command,
        CancellationToken cancellationToken)
    {
        Guard.NotNull(command);

        var workItemId = WorkItemId.Create(command.WorkItemId);
        var workItem = await workItems.GetByIdAsync(currentUser.TenantId, workItemId, cancellationToken);

        var context = new ChangeWorkItemStatusValidationContext(
            workItem,
            currentUser.TenantId,
            currentUser.CanCloseWorkItems,
            command.RequestedStatus);

        var validationResult = validator.Validate(context);

        if (!validationResult.Succeeded)
        {
            return ApplicationResult<WorkItemResult>.Invalid(validationResult.Failures);
        }

        workItem!.ChangeStatus(command.RequestedStatus, statusPolicy);

        await workItems.UpdateAsync(workItem, cancellationToken);

        return ApplicationResult<WorkItemResult>.Success(workItem.ToResult());
    }
}
