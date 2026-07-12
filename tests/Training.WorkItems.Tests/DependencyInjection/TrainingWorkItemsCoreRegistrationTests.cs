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

        services.AddSingleton<IConfiguration>(configuration);
        services.AddTrainingWorkItemsCore(configuration);

        var descriptor = services.FirstOrDefault(d => d.ServiceType == typeof(ICreateWorkItemUseCase));

        descriptor.Should().NotBeNull();
        descriptor!.ImplementationType.Should().Be(typeof(CreateWorkItemUseCase));
    }
}
