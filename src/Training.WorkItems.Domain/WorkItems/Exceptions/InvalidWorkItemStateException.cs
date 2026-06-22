using Training.WorkItems.Domain.Common;
using Training.WorkItems.Domain.WorkItems.Enums;

namespace Training.WorkItems.Domain.WorkItems.Exceptions;

public sealed class InvalidWorkItemStateException : DomainException
{
    public InvalidWorkItemStateException(
        WorkItemStatus currentStatus,
        WorkItemStatus nextStatus)
        : base(
            errorCode: "work-item.invalid-status-transition",
            message: "The work item status transition is invalid.")
    {
        AddData(nameof(currentStatus), currentStatus.ToString());
        AddData(nameof(nextStatus), nextStatus.ToString());
    }
}
