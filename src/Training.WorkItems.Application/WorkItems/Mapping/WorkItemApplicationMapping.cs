using Training.WorkItems.Application.WorkItems.UseCases;
using Training.WorkItems.Domain.WorkItems.Entities;

namespace Training.WorkItems.Application.WorkItems.Mapping;

public static class WorkItemApplicationMapping
{
    public static WorkItemResult ToResult(this WorkItem workItem)
    {
        return new WorkItemResult(
            workItem.Id.Value,
            workItem.Title.Value,
            workItem.Description,
            workItem.Status.ToString(),
            workItem.CreatedAt);
    }
}
