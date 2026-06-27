namespace Training.WorkItems.Application.WorkItems.UseCases;

public sealed record WorkItemResult(
    Guid Id,
    string Title,
    string? Description,
    string Status,
    DateTimeOffset CreatedAt);
