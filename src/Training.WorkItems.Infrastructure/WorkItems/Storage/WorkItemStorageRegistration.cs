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
    private const string NoteResourceName = "WorkItemNoteStorage";

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
            .AddScoped(provider =>
                provider
                    .GetRequiredService<ConnectorFactory>()
                    .CreateAsync<WorkItemNoteStorageRecord>(NoteResourceName))
            .AddScoped<IWorkItemRepository, SqlWorkItemRepository>()
            .AddScoped<IWorkItemStatusChangeRepository, SqlWorkItemStatusChangeRepository>()
            .AddScoped<IWorkItemNoteRepository, SqlWorkItemNoteRepository>();

        return services;
    }
}
