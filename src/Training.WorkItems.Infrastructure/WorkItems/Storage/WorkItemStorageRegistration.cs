using Grasp.Core.Storage;
using Grasp.Core.Storage.Connectors;
using Microsoft.Extensions.DependencyInjection;
using Training.WorkItems.Application.WorkItems.Repositories;
using Training.WorkItems.Infrastructure.WorkItems.Repositories;

namespace Training.WorkItems.Infrastructure.WorkItems.Storage;

public static class WorkItemStorageRegistration
{
    private const string ResourceName = "WorkItemStorage";

    public static IServiceCollection AddWorkItemStorage(this IServiceCollection services)
    {
        services
            .AddGraspCoreStorage()
            .AddScoped(provider =>
                provider
                    .GetRequiredService<ConnectorFactory>()
                    .CreateAsync<WorkItemStorageRecord>(ResourceName))
            .AddScoped<IWorkItemRepository, SqlWorkItemRepository>();

        return services;
    }
}
