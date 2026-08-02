using Microsoft.Extensions.Logging;
using Training.WorkItems.Application.Common;
using Training.WorkItems.Application.WorkItems.Mapping;
using Training.WorkItems.Application.WorkItems.Repositories;
using Training.WorkItems.Application.WorkItems.Services;
using Training.WorkItems.Application.WorkItems.Validation;
using Training.WorkItems.Domain.Common;
using Training.WorkItems.Domain.Common.Validation;
using Training.WorkItems.Domain.WorkItems.Entities;
using Training.WorkItems.Domain.WorkItems.Exceptions;
using Training.WorkItems.Domain.WorkItems.Services;
using Training.WorkItems.Domain.WorkItems.ValueTypes;

namespace Training.WorkItems.Application.WorkItems.UseCases;

public sealed class ChangeWorkItemStatusUseCase(
    IWorkItemRepository workItems,
    IWorkItemStatusChangeRepository statusChanges,
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

        var tenantId = currentUser.TenantId;
        var workItemId = WorkItemId.Create(command.WorkItemId);

        var workItem = await workItems.GetByIdAsync(tenantId, workItemId, cancellationToken);

        if (workItem is null)
        {
            logger.LogInformation(
                "Work item {WorkItemId} was not found for tenant {TenantId} during a status change.",
                workItemId.Value,
                tenantId.Value);

            return ApplicationResult<WorkItemResult>.NotFound();
        }

        var validation = validator.Validate(new ChangeWorkItemStatusValidationContext(
            WorkItem: workItem,
            CurrentTenantId: tenantId,
            CanCloseWorkItems: currentUser.CanCloseWorkItems,
            RequestedStatus: command.RequestedStatus));

        validation.ThrowValidation(failure =>
            new WorkItemStatusChangeValidationException(
                errorCode: failure.ErrorCode,
                target: failure.PropertyName));

        try
        {
            workItem.ChangeStatus(command.RequestedStatus, statusPolicy);
        }
        catch (InvalidWorkItemStateException ex)
        {
            logger.LogInformation(
                "Rejected status transition for work item {WorkItemId} in tenant {TenantId} to {Status}.",
                workItem.Id.Value,
                workItem.TenantId.Value,
                command.RequestedStatus);

            return ApplicationResult<WorkItemResult>.Invalid(ex.Message);
        }

        var auditRecord = WorkItemAuditRecord.Create(
            workItem.Id,
            workItem.TenantId,
            currentUser.UserId,
            workItem.Status);

        await statusChanges.PersistAsync(workItem, auditRecord, cancellationToken);

        logger.LogInformation(
            "Changed work item {WorkItemId} status to {Status} for tenant {TenantId}.",
            workItem.Id.Value,
            workItem.Status,
            workItem.TenantId.Value);

        return ApplicationResult<WorkItemResult>.Success(workItem.ToResult());
    }
}
