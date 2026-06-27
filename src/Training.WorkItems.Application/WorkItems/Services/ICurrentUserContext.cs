using Training.WorkItems.Domain.WorkItems.ValueTypes;

namespace Training.WorkItems.Application.WorkItems.Services;

public interface ICurrentUserContext
{
    TenantId TenantId { get; }
    Guid UserId { get; }
    string? DisplayName { get; }
}
