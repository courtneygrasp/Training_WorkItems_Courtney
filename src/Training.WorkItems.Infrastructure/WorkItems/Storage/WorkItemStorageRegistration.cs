using Grasp.Core.Storage;
using Grasp.Core.Storage.Connectors;
using Microsoft.Extensions.DependencyInjection;
using Training.WorkItems.Application.WorkItems.Repositories;
using Training.WorkItems.Infrastructure.WorkItems.Repositories;

namespace Training.WorkItems.Infrastructure.WorkItems.Storage;

public static class WorkItemStorageRegistration
{
    private const string ResourceName = "WorkItemStorage";
    private const string AuditResourceName = "WorkItemAuditStorage";

    public static IServiceCollection AddWorkItemStorage(this IServiceCollection services)
    {
        services
            .AddGraspCoreStorage()
            .AddScoped(provider =>
                provider
                    .GetRequiredService<ConnectorFactory>()
                    .CreateAsync<WorkItemStorageRecord>(ResourceName))
            .AddScoped(provider =>
                provider
                    .GetRequiredService<ConnectorFactory>()
                    .CreateAsync<WorkItemAuditStorageRecord>(AuditResourceName))
            .AddScoped<IWorkItemRepository, SqlWorkItemRepository>()
            .AddScoped<IWorkItemStatusChangeRepository, SqlWorkItemStatusChangeRepository>();

        return services;
    }
}
