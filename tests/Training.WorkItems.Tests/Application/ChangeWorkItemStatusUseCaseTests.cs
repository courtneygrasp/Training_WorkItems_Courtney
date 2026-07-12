using AwesomeAssertions;
using Training.WorkItems.Application.WorkItems.Repositories;
using Training.WorkItems.Application.WorkItems.UseCases;
using Training.WorkItems.Application.WorkItems.Validation;
using Training.WorkItems.Domain.WorkItems.Entities;
using Training.WorkItems.Domain.WorkItems.Enums;
using Training.WorkItems.Domain.WorkItems.Services;
using Training.WorkItems.Domain.WorkItems.ValueTypes;
using Training.WorkItems.Tests.Application.WorkItems.Fakes;

namespace Training.WorkItems.Tests.Application;

public sealed class ChangeWorkItemStatusUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_WithNullCommand_ThrowsArgumentNullException()
    {
        var useCase = CreateUseCase(new InMemoryWorkItemRepository());

        var act = async () => await useCase.ExecuteAsync(null!, CancellationToken.None);

        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task ExecuteAsync_WhenWorkItemNotFound_ReturnsInvalidResult()
    {
        var useCase = CreateUseCase(new InMemoryWorkItemRepository());
        var command = new ChangeWorkItemStatusCommand(Guid.NewGuid(), WorkItemStatus.InProgress);

        var result = await useCase.ExecuteAsync(command, CancellationToken.None);

        result.Succeeded.Should().BeFalse();
        result.Failures.Should().ContainSingle(f => f.ErrorCode == "work-item.not-found");
    }

    [Fact]
    public async Task ExecuteAsync_WhenUserCannotCloseWorkItems_ReturnsCloseForbiddenFailure()
    {
        var tenantId = TenantId.Create(Guid.NewGuid());
        var repository = new InMemoryWorkItemRepository();
        var workItem = CreateInProgressWorkItem(tenantId);
        await repository.AddAsync(workItem, CancellationToken.None);

        var useCase = CreateUseCase(repository, tenantId, canCloseWorkItems: false);
        var command = new ChangeWorkItemStatusCommand(workItem.Id.Value, WorkItemStatus.Closed);

        var result = await useCase.ExecuteAsync(command, CancellationToken.None);

        result.Succeeded.Should().BeFalse();
        result.Failures.Should().ContainSingle(f => f.ErrorCode == "work-item.close-forbidden");
    }

    [Fact]
    public async Task ExecuteAsync_WhenValidTransition_ChangesStatusAndReturnsSuccess()
    {
        var tenantId = TenantId.Create(Guid.NewGuid());
        var repository = new InMemoryWorkItemRepository();
        var workItem = CreateNewWorkItem(tenantId);
        await repository.AddAsync(workItem, CancellationToken.None);

        var useCase = CreateUseCase(repository, tenantId);
        var command = new ChangeWorkItemStatusCommand(workItem.Id.Value, WorkItemStatus.InProgress);

        var result = await useCase.ExecuteAsync(command, CancellationToken.None);

        result.Succeeded.Should().BeTrue();
        result.Value!.Status.Should().Be("InProgress");
    }

    [Fact]
    public async Task ExecuteAsync_WhenValidatorReturnsMultipleFailures_AllFailuresReturned()
    {
        var tenantId = TenantId.Create(Guid.NewGuid());
        var otherTenantId = TenantId.Create(Guid.NewGuid());

        // Use an ID-only repository so the item is returned even though tenant differs,
        // allowing both wrong-tenant and closed failures to accumulate.
        var repository = new FindByIdOnlyRepository();
        var workItem = CreateClosedWorkItem(otherTenantId);
        await repository.AddAsync(workItem, CancellationToken.None);

        var useCase = CreateUseCase(repository, tenantId);
        var command = new ChangeWorkItemStatusCommand(workItem.Id.Value, WorkItemStatus.InProgress);

        var result = await useCase.ExecuteAsync(command, CancellationToken.None);

        result.Succeeded.Should().BeFalse();
        result.Failures.Should().HaveCountGreaterThan(1);
        result.Failures.Should().Contain(f => f.ErrorCode == "work-item.wrong-tenant");
        result.Failures.Should().Contain(f => f.ErrorCode == "work-item.closed");
    }

    private static ChangeWorkItemStatusUseCase CreateUseCase(
        IWorkItemRepository repository,
        TenantId? tenantId = null,
        bool canCloseWorkItems = true)
    {
        return new ChangeWorkItemStatusUseCase(
            repository,
            new FakeCurrentUserContext(tenantId ?? TenantId.Create(Guid.NewGuid()), canCloseWorkItems),
            new DefaultWorkItemStatusPolicy(),
            new ChangeWorkItemStatusValidator());
    }

    private static WorkItem CreateNewWorkItem(TenantId tenantId)
    {
        return WorkItem.Create(
            WorkItemId.Create(Guid.NewGuid()),
            tenantId,
            WorkItemTitle.Create("Test Item"),
            null,
            DateTimeOffset.UtcNow);
    }

    private static WorkItem CreateInProgressWorkItem(TenantId tenantId)
    {
        var workItem = CreateNewWorkItem(tenantId);
        workItem.ChangeStatus(WorkItemStatus.InProgress, new DefaultWorkItemStatusPolicy());
        return workItem;
    }

    private static WorkItem CreateClosedWorkItem(TenantId tenantId)
    {
        var workItem = CreateInProgressWorkItem(tenantId);
        workItem.ChangeStatus(WorkItemStatus.Closed, new DefaultWorkItemStatusPolicy());
        return workItem;
    }

    // Finds items by ID only — used to test validator paths that require crossing tenant boundaries.
    private sealed class FindByIdOnlyRepository : IWorkItemRepository
    {
        private readonly List<WorkItem> _items = [];

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
            var workItem = _items.SingleOrDefault(x => x.Id == workItemId);
            return Task.FromResult(workItem);
        }

        public Task<IReadOnlyCollection<WorkItem>> ListAsync(
            TenantId tenantId,
            CancellationToken cancellationToken) =>
            Task.FromResult<IReadOnlyCollection<WorkItem>>(Array.Empty<WorkItem>());

        public Task UpdateAsync(WorkItem workItem, CancellationToken cancellationToken) =>
            Task.CompletedTask;
    }
}
