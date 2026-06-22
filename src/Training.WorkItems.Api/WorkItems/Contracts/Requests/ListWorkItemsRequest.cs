using System.ComponentModel.DataAnnotations;

namespace Training.WorkItems.Api.WorkItems.Contracts.Requests;

public sealed record ListWorkItemsRequest
{
    [Range(1, 500)]
    public int PageSize { get; init; } = 50;

    [Range(0, int.MaxValue)]
    public int Page { get; init; } = 0;
}
