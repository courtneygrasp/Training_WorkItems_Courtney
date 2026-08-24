using System.ComponentModel.DataAnnotations;

namespace Training.WorkItems.Api.WorkItems.Contracts.Requests;

public sealed record AddWorkItemNoteRequest
{
    [Required(AllowEmptyStrings = false)]
    [StringLength(2000)]
    public required string Content { get; init; }
}
