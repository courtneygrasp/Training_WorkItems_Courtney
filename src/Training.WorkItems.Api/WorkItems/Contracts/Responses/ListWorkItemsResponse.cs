namespace Training.WorkItems.Api.WorkItems.Contracts.Responses;

public sealed record ListWorkItemsResponse(
    IReadOnlyCollection<WorkItemResponse> Items,
    int Page,
    int PageSize,
    int TotalCount);
