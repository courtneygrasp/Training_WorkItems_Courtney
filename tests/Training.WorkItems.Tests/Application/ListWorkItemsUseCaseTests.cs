using AwesomeAssertions;
using Training.WorkItems.Application.WorkItems.UseCases;
using Training.WorkItems.Domain.WorkItems.Entities;
using Training.WorkItems.Domain.WorkItems.ValueTypes;
using Training.WorkItems.Tests.Application.WorkItems.Fakes;

namespace Training.WorkItems.Tests.Application;

public sealed class ListWorkItemsUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_ReturnsTenantWorkItemsOnly()
    {
        var tenantA = TenantId.Create(Guid.NewGuid());
        var tenantB = TenantId.Create(Guid.NewGuid());
        var repository = new InMemoryWorkItemRepository();

        await repository.AddAsync(CreateWorkItem(tenantA, "Item A1"), CancellationToken.None);
        await repository.AddAsync(CreateWorkItem(tenantA, "Item A2"), CancellationToken.None);
        await repository.AddAsync(CreateWorkItem(tenantB, "Item B1"), CancellationToken.None);

        var useCase = CreateUseCase(repository, tenantA);

        var result = await useCase.ExecuteAsync(CancellationToken.None);

        result.Succeeded.Should().BeTrue();
        result.Value!.Should().HaveCount(2);
        result.Value.Should().AllSatisfy(r => r.Title.Should().StartWith("Item A"));
    }

    [Fact]
    public async Task ExecuteAsync_WhenNoItemsForTenant_ReturnsEmptyList()
    {
        var repository = new InMemoryWorkItemRepository();
        var useCase = CreateUseCase(repository);

        var result = await useCase.ExecuteAsync(CancellationToken.None);

        result.Succeeded.Should().BeTrue();
        result.Value!.Should().BeEmpty();
    }

    [Fact]
    public async Task ExecuteAsync_UsesCurrentTenantIdForQuery()
    {
        var tenantId = TenantId.Create(Guid.NewGuid());
        var repository = new InMemoryWorkItemRepository();
        await repository.AddAsync(CreateWorkItem(tenantId, "My Item"), CancellationToken.None);

        var useCase = CreateUseCase(repository, tenantId);

        var result = await useCase.ExecuteAsync(CancellationToken.None);

        result.Value!.Should().ContainSingle(r => r.Title == "My Item");
    }

    private static ListWorkItemsUseCase CreateUseCase(
        InMemoryWorkItemRepository repository,
        TenantId? tenantId = null)
    {
        return new ListWorkItemsUseCase(
            repository,
            new FakeCurrentUserContext(tenantId ?? TenantId.Create(Guid.NewGuid())));
    }

    private static WorkItem CreateWorkItem(TenantId tenantId, string title) =>
        WorkItem.Create(
            WorkItemId.Create(Guid.NewGuid()),
            tenantId,
            WorkItemTitle.Create(title),
            null,
            DateTimeOffset.UtcNow);
}
