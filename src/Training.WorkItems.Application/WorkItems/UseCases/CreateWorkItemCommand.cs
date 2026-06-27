namespace Training.WorkItems.Application.WorkItems.UseCases;

public sealed record CreateWorkItemCommand(
    string Title,
    string? Description);
