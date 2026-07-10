using System.ComponentModel.DataAnnotations;
using Training.WorkItems.Domain.WorkItems.Enums;

namespace Training.WorkItems.Api.WorkItems.Contracts.Requests;

public sealed record ChangeWorkItemStatusRequest
{
    [Required]
    public required WorkItemStatus Status { get; init; }
}
