using Training.WorkItems.Domain.WorkItems.Enums;

namespace Training.WorkItems.Application.WorkItems.UseCases;

public sealed record ChangeWorkItemStatusCommand(Guid WorkItemId, WorkItemStatus RequestedStatus);
