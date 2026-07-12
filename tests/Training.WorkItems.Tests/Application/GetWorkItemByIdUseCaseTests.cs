using AwesomeAssertions;
using Training.WorkItems.Application.WorkItems.Repositories;
using Training.WorkItems.Application.WorkItems.Services;
using Training.WorkItems.Application.WorkItems.UseCases;
using Training.WorkItems.Domain.WorkItems.Entities;
using Training.WorkItems.Domain.WorkItems.ValueTypes;

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

        result.Succeeded.Should().BeFalse();
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
            new StubCurrentUserContext(tenantId ?? TenantId.Create(Guid.NewGuid())));
    }

    private static WorkItem CreateWorkItem(TenantId tenantId) =>
        WorkItem.Create(
            WorkItemId.Create(Guid.NewGuid()),
            tenantId,
            WorkItemTitle.Create("Test Item"),
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
