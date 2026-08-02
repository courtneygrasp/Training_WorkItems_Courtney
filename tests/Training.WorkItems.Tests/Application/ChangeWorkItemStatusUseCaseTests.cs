using AwesomeAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Training.WorkItems.Application.Common;
using Training.WorkItems.Application.WorkItems.Repositories;
using Training.WorkItems.Application.WorkItems.UseCases;
using Training.WorkItems.Application.WorkItems.Validation;
using Training.WorkItems.Domain.WorkItems.Entities;
using Training.WorkItems.Domain.WorkItems.Enums;
using Training.WorkItems.Domain.WorkItems.Exceptions;
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
    public async Task ExecuteAsync_WhenWorkItemNotFound_ReturnsNotFoundResult()
    {
        var useCase = CreateUseCase(new InMemoryWorkItemRepository());
        var command = new ChangeWorkItemStatusCommand(Guid.NewGuid(), WorkItemStatus.InProgress);

        var result = await useCase.ExecuteAsync(command, CancellationToken.None);

        result.Status.Should().Be(ResultStatus.NotFound);
    }

    [Fact]
    public async Task ExecuteAsync_WhenWorkItemBelongsToDifferentTenant_ReturnsNotFoundResult()
    {
        var tenantId = TenantId.Create(Guid.NewGuid());
        var otherTenantId = TenantId.Create(Guid.NewGuid());
        var repository = new InMemoryWorkItemRepository();
        var workItem = CreateNewWorkItem(otherTenantId);
        await repository.AddAsync(workItem, CancellationToken.None);

        var useCase = CreateUseCase(repository, tenantId);
        var command = new ChangeWorkItemStatusCommand(workItem.Id.Value, WorkItemStatus.InProgress);

        var result = await useCase.ExecuteAsync(command, CancellationToken.None);

        // Tenant-scoped load returns null for items belonging to another tenant.
        result.Status.Should().Be(ResultStatus.NotFound);
    }

    [Fact]
    public async Task ExecuteAsync_WhenUserCannotCloseWorkItems_ThrowsWorkItemStatusChangeValidationException()
    {
        var tenantId = TenantId.Create(Guid.NewGuid());
        var repository = new InMemoryWorkItemRepository();
        var workItem = CreateInProgressWorkItem(tenantId);
        await repository.AddAsync(workItem, CancellationToken.None);

        var useCase = CreateUseCase(repository, tenantId, canCloseWorkItems: false);
        var command = new ChangeWorkItemStatusCommand(workItem.Id.Value, WorkItemStatus.Closed);

        var act = async () => await useCase.ExecuteAsync(command, CancellationToken.None);

        await act.Should().ThrowAsync<WorkItemStatusChangeValidationException>()
            .Where(ex => ex.ErrorCode == "work-item.close-forbidden");
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
    public async Task ExecuteAsync_WhenValidTransition_PersistsAuditRecord()
    {
        var tenantId = TenantId.Create(Guid.NewGuid());
        var repository = new InMemoryWorkItemRepository();
        var statusChangeRepository = new InMemoryWorkItemStatusChangeRepository();
        var workItem = CreateNewWorkItem(tenantId);
        await repository.AddAsync(workItem, CancellationToken.None);

        var useCase = CreateUseCase(repository, tenantId, statusChangeRepo: statusChangeRepository);
        var command = new ChangeWorkItemStatusCommand(workItem.Id.Value, WorkItemStatus.InProgress);

        await useCase.ExecuteAsync(command, CancellationToken.None);

        statusChangeRepository.AuditRecords.Should().ContainSingle();
        var auditRecord = statusChangeRepository.AuditRecords.Single();
        auditRecord.WorkItemId.Should().Be(workItem.Id);
        auditRecord.TenantId.Should().Be(workItem.TenantId);
        auditRecord.Status.Should().Be(WorkItemStatus.InProgress);
    }

    [Fact]
    public async Task ExecuteAsync_WhenInvalidTransition_ReturnsInvalidResult()
    {
        var tenantId = TenantId.Create(Guid.NewGuid());
        var repository = new InMemoryWorkItemRepository();
        var workItem = CreateClosedWorkItem(tenantId);
        await repository.AddAsync(workItem, CancellationToken.None);

        var useCase = CreateUseCase(repository, tenantId);
        var command = new ChangeWorkItemStatusCommand(workItem.Id.Value, WorkItemStatus.InProgress);

        var result = await useCase.ExecuteAsync(command, CancellationToken.None);

        result.Status.Should().Be(ResultStatus.Invalid);
        result.ErrorMessage.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task ExecuteAsync_WhenInvalidTransition_DoesNotPersistAuditRecord()
    {
        var tenantId = TenantId.Create(Guid.NewGuid());
        var repository = new InMemoryWorkItemRepository();
        var statusChangeRepository = new InMemoryWorkItemStatusChangeRepository();
        var workItem = CreateClosedWorkItem(tenantId);
        await repository.AddAsync(workItem, CancellationToken.None);

        var useCase = CreateUseCase(repository, tenantId, statusChangeRepo: statusChangeRepository);
        var command = new ChangeWorkItemStatusCommand(workItem.Id.Value, WorkItemStatus.InProgress);

        await useCase.ExecuteAsync(command, CancellationToken.None);

        statusChangeRepository.AuditRecords.Should().BeEmpty();
    }

    private static ChangeWorkItemStatusUseCase CreateUseCase(
        IWorkItemRepository repository,
        TenantId? tenantId = null,
        bool canCloseWorkItems = true,
        InMemoryWorkItemStatusChangeRepository? statusChangeRepo = null)
    {
        return new ChangeWorkItemStatusUseCase(
            repository,
            statusChangeRepo ?? new InMemoryWorkItemStatusChangeRepository(),
            new FakeCurrentUserContext(tenantId ?? TenantId.Create(Guid.NewGuid()), canCloseWorkItems),
            new DefaultWorkItemStatusPolicy(),
            new ChangeWorkItemStatusValidator(),
            NullLogger<ChangeWorkItemStatusUseCase>.Instance);
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
}
