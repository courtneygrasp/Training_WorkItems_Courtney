using Training.WorkItems.Application.WorkItems.Services;
using Training.WorkItems.Domain.WorkItems.ValueTypes;

namespace Training.WorkItems.Infrastructure.WorkItems.Services;

public sealed class DefaultCurrentUserContext : ICurrentUserContext
{
    public TenantId TenantId { get; } = TenantId.Create(Guid.Parse("11111111-1111-1111-1111-111111111111"));
    public Guid UserId { get; } = Guid.Parse("22222222-2222-2222-2222-222222222222");
    public string? DisplayName { get; } = "Training User";
    public bool CanCloseWorkItems { get; } = true;
}
