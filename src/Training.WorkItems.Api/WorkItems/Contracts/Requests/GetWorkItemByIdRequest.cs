using System.ComponentModel.DataAnnotations;

namespace Training.WorkItems.Api.WorkItems.Contracts.Requests;

public sealed record GetWorkItemByIdRequest
{
    [Required]
    public required Guid Id { get; init; }
}
