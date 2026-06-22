using Training.WorkItems.Domain.WorkItems.Enums;

namespace Training.WorkItems.Domain.WorkItems.Services;

public interface IWorkItemStatusPolicy
{
    bool CanTransition(WorkItemStatus currentStatus, WorkItemStatus nextStatus);
}
