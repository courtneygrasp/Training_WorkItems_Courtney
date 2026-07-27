using AwesomeAssertions;
using Training.WorkItems.Application.WorkItems.UseCases;
using Training.WorkItems.Domain.WorkItems.Entities;
using Training.WorkItems.Domain.WorkItems.ValueTypes;
using Training.WorkItems.Tests.Application.WorkItems.Fakes;

namespace Training.WorkItems.Tests.Application;

public sealed class GetWorkItemByIdUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_UsesCurrentTenantToFetchWorkItem()
    {
        var tenantId = TenantId.Create(Guid.NewGuid());
        var repository = new InMemoryWorkItemRepository();
        var workItem = CreateWorkItem(tenantId);
        await repository.AddAsync(workItem, CancellationToken.None);

        var useCase = CreateUseCase(repository, tenantId);
        var query = new GetWorkItemByIdQuery(workItem.Id.Value);

        var result = await useCase.ExecuteAsync(query, CancellationToken.None);

        result.Succeeded.Should().BeTrue();
        result.Value!.Id.Should().Be(workItem.Id.Value);
    }

    [Fact]
    public async Task ExecuteAsync_WhenWorkItemBelongsToDifferentTenant_ReturnsNotFound()
    {
        var ownerTenantId = TenantId.Create(Guid.NewGuid());
        var callerTenantId = TenantId.Create(Guid.NewGuid());
        var repository = new InMemoryWorkItemRepository();
        var workItem = CreateWorkItem(ownerTenantId);
        await repository.AddAsync(workItem, CancellationToken.None);

        var useCase = CreateUseCase(repository, callerTenantId);
        var query = new GetWorkItemByIdQuery(workItem.Id.Value);

        var result = await useCase.ExecuteAsync(query, CancellationToken.None);

        result.Succeeded.Should().BeFalse();
    }

    [Fact]
    public async Task ExecuteAsync_WhenWorkItemDoesNotExist_ReturnsNotFound()
    {
        var repository = new InMemoryWorkItemRepository();
        var useCase = CreateUseCase(repository);
        var query = new GetWorkItemByIdQuery(Guid.NewGuid());

        var result = await useCase.ExecuteAsync(query, CancellationToken.None);

        result.Status.Should().Be(Training.WorkItems.Application.Common.ResultStatus.NotFound);
    }

    [Fact]
    public async Task ExecuteAsync_WithNullQuery_ThrowsArgumentNullException()
    {
        var useCase = CreateUseCase(new InMemoryWorkItemRepository());

        var act = async () => await useCase.ExecuteAsync(null!, CancellationToken.None);

        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    private static GetWorkItemByIdUseCase CreateUseCase(
        InMemoryWorkItemRepository repository,
        TenantId? tenantId = null)
    {
        return new GetWorkItemByIdUseCase(
            repository,
            new FakeCurrentUserContext(tenantId ?? TenantId.Create(Guid.NewGuid())),
            Microsoft.Extensions.Logging.Abstractions.NullLogger<GetWorkItemByIdUseCase>.Instance);
    }

    private static WorkItem CreateWorkItem(TenantId tenantId) =>
        WorkItem.Create(
            WorkItemId.Create(Guid.NewGuid()),
            tenantId,
            WorkItemTitle.Create("Test Item"),
            null,
            DateTimeOffset.UtcNow);
}
