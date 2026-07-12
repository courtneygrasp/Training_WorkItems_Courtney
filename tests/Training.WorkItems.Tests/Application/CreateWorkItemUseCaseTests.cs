using AwesomeAssertions;
using Training.WorkItems.Application.Common;
using Training.WorkItems.Application.WorkItems.Repositories;
using Training.WorkItems.Application.WorkItems.Services;
using Training.WorkItems.Application.WorkItems.UseCases;
using Training.WorkItems.Domain.WorkItems.Entities;
using Training.WorkItems.Domain.WorkItems.ValueTypes;

namespace Training.WorkItems.Tests.Application;

public sealed class CreateWorkItemUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_WithValidCommand_CreatesAndSavesWorkItem()
    {
        var tenantId = TenantId.Create(Guid.NewGuid());
        var repository = new InMemoryWorkItemRepository();
        var createdAt = new DateTimeOffset(2026, 6, 26, 10, 30, 0, TimeSpan.Zero);
        var useCase = CreateUseCase(repository, tenantId, createdAt);
        var command = new CreateWorkItemCommand("Fix login bug", "Users cannot sign in.");

        var result = await useCase.ExecuteAsync(command, CancellationToken.None);

        result.Succeeded.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Title.Should().Be("Fix login bug");
        result.Value.Description.Should().Be("Users cannot sign in.");
        result.Value.Status.Should().Be("New");
        result.Value.CreatedAt.Should().Be(createdAt);
        repository.Items.Should().ContainSingle();
    }

    [Fact]
    public async Task ExecuteAsync_WithBlankTitle_ReturnsInvalidResult()
    {
        var repository = new InMemoryWorkItemRepository();
        var useCase = CreateUseCase(repository);
        var command = new CreateWorkItemCommand(" ", "Description");

        var result = await useCase.ExecuteAsync(command, CancellationToken.None);

        result.Succeeded.Should().BeFalse();
        result.Value.Should().BeNull();
        result.ErrorMessage.Should().NotBeNullOrWhiteSpace();
        repository.Items.Should().BeEmpty();
    }

    [Fact]
    public async Task ExecuteAsync_UsesCurrentTenantForSavedWorkItem()
    {
        var tenantId = TenantId.Create(Guid.NewGuid());
        var repository = new InMemoryWorkItemRepository();
        var useCase = CreateUseCase(repository, tenantId);
        var command = new CreateWorkItemCommand("Fix login bug", null);

        await useCase.ExecuteAsync(command, CancellationToken.None);

        repository.Items.Should().ContainSingle();
        repository.Items.Single().TenantId.Should().Be(tenantId);
    }

    [Fact]
    public async Task ExecuteAsync_RepositoryReceivesCreatedWorkItem()
    {
        var repository = new InMemoryWorkItemRepository();
        var useCase = CreateUseCase(repository);
        var command = new CreateWorkItemCommand("Fix login bug", "Description");

        var result = await useCase.ExecuteAsync(command, CancellationToken.None);

        repository.Items.Should().ContainSingle();
        repository.Items.Single().Id.Value.Should().Be(result.Value!.Id);
        repository.Items.Single().Title.Value.Should().Be("Fix login bug");
    }

    private static CreateWorkItemUseCase CreateUseCase(
        InMemoryWorkItemRepository repository,
        TenantId? tenantId = null,
        DateTimeOffset? utcNow = null)
    {
        return new CreateWorkItemUseCase(
            repository,
            new StubCurrentUserContext(tenantId ?? TenantId.Create(Guid.NewGuid())),
            new StubSystemClock(utcNow ?? DateTimeOffset.UtcNow));
    }

    private sealed class InMemoryWorkItemRepository : IWorkItemRepository
    {
        private readonly List<WorkItem> _items = [];

        public IReadOnlyCollection<WorkItem> Items => _items;

        public Task AddAsync(WorkItem workItem, CancellationToken cancellationToken)
        {
            _items.Add(workItem);

            return Task.CompletedTask;
        }

        public Task<WorkItem?> GetByIdAsync(
            TenantId tenantId,
            WorkItemId workItemId,
            CancellationToken cancellationToken)
        {
            var workItem = _items.SingleOrDefault(x => x.TenantId == tenantId && x.Id == workItemId);

            return Task.FromResult(workItem);
        }

        public Task<IReadOnlyCollection<WorkItem>> ListAsync(
            TenantId tenantId,
            CancellationToken cancellationToken)
        {
            IReadOnlyCollection<WorkItem> results = _items
                .Where(x => x.TenantId == tenantId)
                .ToArray();
            return Task.FromResult(results);
        }

        public Task UpdateAsync(WorkItem workItem, CancellationToken cancellationToken) =>
            Task.CompletedTask;
    }

    private sealed class StubCurrentUserContext(TenantId tenantId) : ICurrentUserContext
    {
        public TenantId TenantId { get; } = tenantId;
        public Guid UserId { get; } = Guid.NewGuid();
        public string? DisplayName { get; } = "Test User";
        public bool CanCloseWorkItems { get; } = true;
    }

    private sealed class StubSystemClock(DateTimeOffset utcNow) : ISystemClock
    {
        public DateTimeOffset UtcNow { get; } = utcNow;
    }
}
