using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Training.WorkItems.Application.WorkItems.Services;
using Training.WorkItems.Domain.WorkItems.ValueTypes;

namespace Training.WorkItems.Infrastructure.Authentication;

public sealed class HttpContextCurrentUserContext(IHttpContextAccessor httpContextAccessor) : ICurrentUserContext
{
    private ClaimsPrincipal User =>
        httpContextAccessor.HttpContext?.User
        ?? throw new InvalidOperationException("No active HTTP context.");

    public TenantId TenantId
    {
        get
        {
            var value = User.FindFirstValue("tenant_id");

            return value is not null && Guid.TryParse(value, out var guid)
                ? TenantId.Create(guid)
                : throw new InvalidOperationException("Authenticated user has no tenant_id claim.");
        }
    }

    public Guid UserId
    {
        get
        {
            var value = User.FindFirstValue(ClaimTypes.NameIdentifier);

            return value is not null && Guid.TryParse(value, out var guid)
                ? guid
                : throw new InvalidOperationException("Authenticated user has no NameIdentifier claim.");
        }
    }

    public string? DisplayName => User.FindFirstValue(ClaimTypes.Name);

    public bool CanCloseWorkItems
    {
        get
        {
            var value = User.FindFirstValue("can_close_work_items");
            return value is not null && bool.TryParse(value, out var result) && result;
        }
    }
}
