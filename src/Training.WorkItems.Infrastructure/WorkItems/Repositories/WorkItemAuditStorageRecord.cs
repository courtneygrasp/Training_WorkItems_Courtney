using System.ComponentModel.DataAnnotations;

namespace Training.WorkItems.Infrastructure.WorkItems.Repositories;

public sealed record WorkItemAuditStorageRecord
{
    [Key]
    public required Guid AuditId { get; init; }
    public required Guid WorkItemId { get; init; }
    public required Guid TenantId { get; init; }
    public required Guid UserId { get; init; }
    public required string Status { get; init; }
    public required DateTimeOffset ChangedAt { get; init; }
}
