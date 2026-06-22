using Training.WorkItems.Domain.WorkItems.Enums;
using Training.WorkItems.Domain.WorkItems.Exceptions;
using Training.WorkItems.Domain.WorkItems.Services;
using Training.WorkItems.Domain.WorkItems.ValueTypes;

namespace Training.WorkItems.Domain.WorkItems.Entities;

public sealed class WorkItem
{
    private WorkItem(
        WorkItemId id,
        TenantId tenantId,
        WorkItemTitle title,
        string? description,
        WorkItemStatus status,
        DateTimeOffset createdAt)
    {
        Id = id;
        TenantId = tenantId;
        Title = title;
        Description = description;
        Status = status;
        CreatedAt = createdAt;
    }

    public WorkItemId Id { get; }
    public TenantId TenantId { get; }
    public WorkItemTitle Title { get; private set; }
    public string? Description { get; private set; }
    public WorkItemStatus Status { get; private set; }
    public DateTimeOffset CreatedAt { get; }

    public static WorkItem Create(
        WorkItemId id,
        TenantId tenantId,
        WorkItemTitle title,
        string? description,
        DateTimeOffset createdAt)
    {
        return new WorkItem(
            id,
            tenantId,
            title,
            description,
            WorkItemStatus.New,
            createdAt);
    }

    public void ChangeTitle(WorkItemTitle title)
    {
        Title = title;
    }

    public void ChangeDescription(string? description)
    {
        Description = description;
    }

    public void ChangeStatus(WorkItemStatus nextStatus, IWorkItemStatusPolicy statusPolicy)
    {
        if (!statusPolicy.CanTransition(Status, nextStatus))
        {
            throw new InvalidWorkItemStateException(Status, nextStatus);
        }

        Status = nextStatus;
    }
}
