using System.ComponentModel.DataAnnotations;

namespace Training.WorkItems.Api.WorkItems.Contracts.Requests;

public sealed record CreateWorkItemRequest
{
    [Required(AllowEmptyStrings = false)]
    [StringLength(120)]
    public required string Title { get; init; }

    [StringLength(4000)]
    public string? Description { get; init; }
}
