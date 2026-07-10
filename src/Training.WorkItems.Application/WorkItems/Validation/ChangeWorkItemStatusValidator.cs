using Training.WorkItems.Domain.Common.Validation;
using Training.WorkItems.Domain.WorkItems.Enums;

namespace Training.WorkItems.Application.WorkItems.Validation;

public sealed class ChangeWorkItemStatusValidator : IValidator<ChangeWorkItemStatusValidationContext>
{
    public ValidationResult Validate(ChangeWorkItemStatusValidationContext context)
    {
        var failures = new List<ValidationFailure>();

        if (context.WorkItem is null)
        {
            failures.Add(new ValidationFailure(
                "work-item.not-found",
                "workItem",
                null,
                null));

            return ValidationResult.Failure(failures);
        }

        if (context.WorkItem.TenantId != context.CurrentTenantId)
        {
            failures.Add(new ValidationFailure(
                "work-item.wrong-tenant",
                "tenantId",
                null,
                null));
        }

        if (context.WorkItem.Status == WorkItemStatus.Closed
            && context.RequestedStatus != WorkItemStatus.Closed)
        {
            failures.Add(new ValidationFailure(
                "work-item.closed",
                "status",
                null,
                null));
        }

        if (context.RequestedStatus == WorkItemStatus.Closed
            && !context.CanCloseWorkItems)
        {
            failures.Add(new ValidationFailure(
                "work-item.close-forbidden",
                "requestedStatus",
                null,
                null));
        }

        return failures.Count == 0
            ? ValidationResult.Success()
            : ValidationResult.Failure(failures);
    }
}
