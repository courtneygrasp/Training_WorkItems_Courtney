using AwesomeAssertions;
using Training.WorkItems.Application.WorkItems.Validation;
using Training.WorkItems.Domain.WorkItems.Entities;
using Training.WorkItems.Domain.WorkItems.Enums;
using Training.WorkItems.Domain.WorkItems.Services;
using Training.WorkItems.Domain.WorkItems.ValueTypes;

namespace Training.WorkItems.Tests.Domain.Validation;

public sealed class ChangeWorkItemStatusValidatorTests
{
    private readonly ChangeWorkItemStatusValidator _validator = new();
    private readonly TenantId _tenantId = TenantId.Create(Guid.NewGuid());

    [Fact]
    public void Validate_WhenRequestingCloseWithoutPermission_ReturnsCloseForbiddenFailure()
    {
        var workItem = CreateWorkItem(_tenantId, WorkItemStatus.InProgress);

        var context = new ChangeWorkItemStatusValidationContext(
            WorkItem: workItem,
            CurrentTenantId: _tenantId,
            CanCloseWorkItems: false,
            RequestedStatus: WorkItemStatus.Closed);

        var result = _validator.Validate(context);

        result.Succeeded.Should().BeFalse();
        result.Failures.Should().ContainSingle(f => f.ErrorCode == "work-item.close-forbidden");
    }

    [Fact]
    public void Validate_WhenAllRulesPass_ReturnsSuccess()
    {
        var workItem = CreateWorkItem(_tenantId, WorkItemStatus.New);

        var context = new ChangeWorkItemStatusValidationContext(
            WorkItem: workItem,
            CurrentTenantId: _tenantId,
            CanCloseWorkItems: true,
            RequestedStatus: WorkItemStatus.InProgress);

        var result = _validator.Validate(context);

        result.Succeeded.Should().BeTrue();
        result.Failures.Should().BeEmpty();
    }

    [Fact]
    public void Validate_WhenNotRequestingClose_ReturnsSuccessRegardlessOfPermission()
    {
        var workItem = CreateWorkItem(_tenantId, WorkItemStatus.New);

        var context = new ChangeWorkItemStatusValidationContext(
            WorkItem: workItem,
            CurrentTenantId: _tenantId,
            CanCloseWorkItems: false,
            RequestedStatus: WorkItemStatus.InProgress);

        var result = _validator.Validate(context);

        result.Succeeded.Should().BeTrue();
    }

    private static WorkItem CreateWorkItem(TenantId tenantId, WorkItemStatus status)
    {
        var policy = new DefaultWorkItemStatusPolicy();
        var workItem = WorkItem.Create(
            WorkItemId.Create(Guid.NewGuid()),
            tenantId,
            WorkItemTitle.Create("Test Item"),
            null,
            DateTimeOffset.UtcNow);

        if (status == WorkItemStatus.InProgress)
        {
            workItem.ChangeStatus(WorkItemStatus.InProgress, policy);
        }
        else if (status == WorkItemStatus.Completed)
        {
            workItem.ChangeStatus(WorkItemStatus.InProgress, policy);
            workItem.ChangeStatus(WorkItemStatus.Completed, policy);
        }
        else if (status == WorkItemStatus.Closed)
        {
            workItem.ChangeStatus(WorkItemStatus.InProgress, policy);
            workItem.ChangeStatus(WorkItemStatus.Closed, policy);
        }

        return workItem;
    }
}
