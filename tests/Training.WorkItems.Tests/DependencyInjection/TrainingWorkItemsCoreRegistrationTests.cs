using AwesomeAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Training.WorkItems.Application.WorkItems.UseCases;
using Training.WorkItems.Core.DependencyInjection;

namespace Training.WorkItems.Tests.DependencyInjection;

public sealed class TrainingWorkItemsCoreRegistrationTests
{
    [Fact]
    public void AddTrainingWorkItemsCore_RegistersCreateWorkItemUseCase()
    {
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder().Build();

        services.AddTrainingWorkItemsCore(configuration);

        using var provider = services.BuildServiceProvider();
        var useCase = provider.GetRequiredService<ICreateWorkItemUseCase>();

        useCase.Should().BeOfType<CreateWorkItemUseCase>();
    }
}
