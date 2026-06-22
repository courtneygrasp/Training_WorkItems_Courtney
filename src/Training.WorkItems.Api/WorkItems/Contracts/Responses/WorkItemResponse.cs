namespace Training.WorkItems.Api.WorkItems.Contracts.Responses;

public sealed record WorkItemResponse(
    Guid Id,
    string Title,
    string? Description,
    string Status,
    DateTimeOffset CreatedAt);
