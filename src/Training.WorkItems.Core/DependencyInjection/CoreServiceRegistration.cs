using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Training.WorkItems.Application.WorkItems.DependencyInjection;
using Training.WorkItems.Infrastructure.WorkItems.DependencyInjection;

namespace Training.WorkItems.Core.DependencyInjection;

public static class CoreServiceRegistration
{
    public static IServiceCollection AddTrainingWorkItemsCore(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddWorkItemsApplication();
        services.AddWorkItemsInfrastructure(configuration);

        return services;
    }
}
