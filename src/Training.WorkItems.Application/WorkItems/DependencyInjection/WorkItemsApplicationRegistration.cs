using Microsoft.Extensions.DependencyInjection;
using Training.WorkItems.Application.WorkItems.UseCases;

namespace Training.WorkItems.Application.WorkItems.DependencyInjection;

public static class WorkItemsApplicationRegistration
{
    public static IServiceCollection AddWorkItemsApplication(this IServiceCollection services)
    {
        services.AddScoped<ICreateWorkItemUseCase, CreateWorkItemUseCase>();

        return services;
    }
}
