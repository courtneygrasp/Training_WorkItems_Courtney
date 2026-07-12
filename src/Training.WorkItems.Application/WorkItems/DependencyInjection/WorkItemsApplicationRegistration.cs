using Microsoft.Extensions.DependencyInjection;
using Training.WorkItems.Application.WorkItems.UseCases;
using Training.WorkItems.Application.WorkItems.Validation;
using Training.WorkItems.Domain.Common.Validation;
using Training.WorkItems.Domain.WorkItems.Services;

namespace Training.WorkItems.Application.WorkItems.DependencyInjection;

public static class WorkItemsApplicationRegistration
{
    public static IServiceCollection AddWorkItemsApplication(this IServiceCollection services)
    {
        services.AddScoped<ICreateWorkItemUseCase, CreateWorkItemUseCase>();
        services.AddScoped<IChangeWorkItemStatusUseCase, ChangeWorkItemStatusUseCase>();
        services.AddScoped<IGetWorkItemByIdUseCase, GetWorkItemByIdUseCase>();
        services.AddScoped<IListWorkItemsUseCase, ListWorkItemsUseCase>();

        services.AddSingleton<IWorkItemStatusPolicy, DefaultWorkItemStatusPolicy>();
        services.AddSingleton<IValidator<ChangeWorkItemStatusValidationContext>, ChangeWorkItemStatusValidator>();

        return services;
    }
}
