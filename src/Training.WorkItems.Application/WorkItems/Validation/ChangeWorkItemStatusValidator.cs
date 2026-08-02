using Training.WorkItems.Domain.Common.Validation;
using Training.WorkItems.Domain.WorkItems.Enums;

namespace Training.WorkItems.Application.WorkItems.Validation;

/// <summary>
/// Validates cross-input workflow rules for a status change request.
/// Loading and tenant-scope concerns belong to the use case; domain transition
/// invariants belong to <c>IWorkItemStatusPolicy</c>.
/// </summary>
public sealed class ChangeWorkItemStatusValidator : IValidator<ChangeWorkItemStatusValidationContext>
{
    /// <inheritdoc />
    public ValidationResult Validate(ChangeWorkItemStatusValidationContext context)
    {
        var failures = new List<ValidationFailure>();

        if (context.RequestedStatus == WorkItemStatus.Closed && !context.CanCloseWorkItems)
        {
            failures.Add(new ValidationFailure(
                "work-item.close-forbidden",
                nameof(context.RequestedStatus),
                context.RequestedStatus,
                null));
        }

        return failures.Count == 0
            ? ValidationResult.Success()
            : ValidationResult.Failure(failures);
    }
}
