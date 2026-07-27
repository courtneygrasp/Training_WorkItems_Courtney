using Microsoft.Extensions.Logging;
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
    IValidator<ChangeWorkItemStatusValidationContext> validator,
    ILogger<ChangeWorkItemStatusUseCase> logger) : IChangeWorkItemStatusUseCase
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
            return MapFailuresToResult(validationResult.Failures, workItemId.Value);
        }

        workItem!.ChangeStatus(command.RequestedStatus, statusPolicy);

        await workItems.UpdateAsync(workItem, cancellationToken);

        logger.LogInformation(
            "Work item {WorkItemId} status changed to {Status}.",
            workItem.Id.Value,
            command.RequestedStatus);

        return ApplicationResult<WorkItemResult>.Success(workItem.ToResult());
    }

    private ApplicationResult<WorkItemResult> MapFailuresToResult(
        IReadOnlyList<ValidationFailure> failures,
        Guid workItemId)
    {
        if (failures.Any(f => f.ErrorCode is "work-item.not-found" or "work-item.wrong-tenant"))
        {
            logger.LogInformation(
                "Work item {WorkItemId} not found or inaccessible for tenant {TenantId}.",
                workItemId,
                currentUser.TenantId.Value);

            return ApplicationResult<WorkItemResult>.NotFound();
        }

        if (failures.Any(f => f.ErrorCode == "work-item.close-forbidden"))
        {
            logger.LogWarning(
                "User {UserId} attempted to close work item {WorkItemId} without close permission.",
                currentUser.UserId,
                workItemId);

            return ApplicationResult<WorkItemResult>.Forbidden();
        }

        logger.LogInformation(
            "Invalid status transition requested for work item {WorkItemId}.",
            workItemId);

        return ApplicationResult<WorkItemResult>.Invalid(failures);
    }
}
