using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Training.WorkItems.Application.Common;
using Training.WorkItems.Application.WorkItems.Services;
using Training.WorkItems.Infrastructure.Authentication;
using Training.WorkItems.Infrastructure.WorkItems.Services;
using Training.WorkItems.Infrastructure.WorkItems.Storage;

namespace Training.WorkItems.Infrastructure.WorkItems.DependencyInjection;

public static class WorkItemsInfrastructureRegistration
{
    public static IServiceCollection AddWorkItemsInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddWorkItemStorage();
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserContext, HttpContextCurrentUserContext>();
        services.AddSingleton<ISystemClock, SystemClock>();

        return services;
    }
}
