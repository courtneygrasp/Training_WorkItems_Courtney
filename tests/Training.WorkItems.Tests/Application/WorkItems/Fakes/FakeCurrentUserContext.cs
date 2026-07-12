using Training.WorkItems.Application.WorkItems.Services;
using Training.WorkItems.Domain.WorkItems.ValueTypes;

namespace Training.WorkItems.Tests.Application.WorkItems.Fakes;

public sealed class FakeCurrentUserContext(TenantId tenantId, bool canCloseWorkItems = true) : ICurrentUserContext
{
    public TenantId TenantId { get; } = tenantId;
    public Guid UserId { get; } = Guid.NewGuid();
    public string? DisplayName { get; } = "Test User";
    public bool CanCloseWorkItems { get; } = canCloseWorkItems;
}
