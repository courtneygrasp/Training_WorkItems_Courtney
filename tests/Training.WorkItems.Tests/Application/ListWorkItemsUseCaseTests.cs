using AwesomeAssertions;
using Training.WorkItems.Application.WorkItems.Repositories;
using Training.WorkItems.Application.WorkItems.Services;
using Training.WorkItems.Application.WorkItems.UseCases;
using Training.WorkItems.Domain.WorkItems.Entities;
using Training.WorkItems.Domain.WorkItems.ValueTypes;

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
            new StubCurrentUserContext(tenantId ?? TenantId.Create(Guid.NewGuid())));
    }

    private static WorkItem CreateWorkItem(TenantId tenantId, string title) =>
        WorkItem.Create(
            WorkItemId.Create(Guid.NewGuid()),
            tenantId,
            WorkItemTitle.Create(title),
            null,
            DateTimeOffset.UtcNow);

    private sealed class InMemoryWorkItemRepository : IWorkItemRepository
    {
        private readonly List<WorkItem> _items = [];

        public Task AddAsync(WorkItem workItem, CancellationToken cancellationToken)
        {
            _items.Add(workItem);
            return Task.CompletedTask;
        }

        public Task<WorkItem?> GetByIdAsync(TenantId tenantId, WorkItemId workItemId, CancellationToken cancellationToken)
        {
            var workItem = _items.SingleOrDefault(x => x.TenantId == tenantId && x.Id == workItemId);
            return Task.FromResult(workItem);
        }

        public Task<IReadOnlyCollection<WorkItem>> ListAsync(TenantId tenantId, CancellationToken cancellationToken)
        {
            IReadOnlyCollection<WorkItem> results = _items.Where(x => x.TenantId == tenantId).ToArray();
            return Task.FromResult(results);
        }

        public Task UpdateAsync(WorkItem workItem, CancellationToken cancellationToken) => Task.CompletedTask;
    }

    private sealed class StubCurrentUserContext(TenantId tenantId) : ICurrentUserContext
    {
        public TenantId TenantId { get; } = tenantId;
        public Guid UserId { get; } = Guid.NewGuid();
        public string? DisplayName { get; } = "Test User";
        public bool CanCloseWorkItems { get; } = true;
    }
}
