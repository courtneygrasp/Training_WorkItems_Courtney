using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Training.WorkItems.Application.Common;
using Training.WorkItems.Application.WorkItems.Repositories;
using Training.WorkItems.Application.WorkItems.Services;
using Training.WorkItems.Infrastructure.WorkItems.Repositories;
using Training.WorkItems.Infrastructure.WorkItems.Services;

namespace Training.WorkItems.Infrastructure.WorkItems.DependencyInjection;

public static class WorkItemsInfrastructureRegistration
{
    public static IServiceCollection AddWorkItemsInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<IWorkItemRepository, InMemoryWorkItemRepository>();
        services.AddScoped<ICurrentUserContext, DefaultCurrentUserContext>();
        services.AddSingleton<ISystemClock, SystemClock>();

        return services;
    }
}
