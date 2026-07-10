using Training.WorkItems.Domain.WorkItems.Enums;

namespace Training.WorkItems.Domain.WorkItems.Services;

public sealed class DefaultWorkItemStatusPolicy : IWorkItemStatusPolicy
{
    public bool CanTransition(WorkItemStatus currentStatus, WorkItemStatus nextStatus)
    {
        if (currentStatus == nextStatus)
        {
            return true;
        }

        return (currentStatus, nextStatus) switch
        {
            (WorkItemStatus.New, WorkItemStatus.InProgress) => true,
            (WorkItemStatus.InProgress, WorkItemStatus.Completed) => true,
            (WorkItemStatus.InProgress, WorkItemStatus.Closed) => true,
            (WorkItemStatus.Completed, WorkItemStatus.Closed) => true,
            _ => false
        };
    }
}
