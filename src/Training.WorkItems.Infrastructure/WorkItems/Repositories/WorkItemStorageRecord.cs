namespace Training.WorkItems.Infrastructure.WorkItems.Repositories;

public sealed record WorkItemStorageRecord
{
    public required Guid WorkItemId { get; init; }
    public required Guid TenantId { get; init; }
    public required string Title { get; init; }
    public string? Description { get; init; }
    public required string Status { get; init; }
    public required DateTimeOffset CreatedAt { get; init; }
}
