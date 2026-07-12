using AwesomeAssertions;
using Training.WorkItems.Domain.WorkItems.Entities;
using Training.WorkItems.Domain.WorkItems.Enums;
using Training.WorkItems.Domain.WorkItems.Exceptions;
using Training.WorkItems.Domain.WorkItems.Services;
using Training.WorkItems.Domain.WorkItems.ValueTypes;

namespace Training.WorkItems.Tests.Domain;

public sealed class WorkItemTests
{
    [Fact]
    public void Create_WithValidTitle_CreatesWorkItemWithNewStatus()
    {
        var workItem = CreateWorkItem("Fix login bug");

        workItem.Status.Should().Be(WorkItemStatus.New);
        workItem.Title.Value.Should().Be("Fix login bug");
    }

    [Fact]
    public void Create_WithBlankTitle_ThrowsException()
    {
        Action act = () => WorkItemTitle.Create(" ");

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Create_WithTooLongTitle_ThrowsException()
    {
        var title = new string('a', 121);

        Action act = () => WorkItemTitle.Create(title);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Rehydrate_WithGivenStatus_RestoresWorkItemWithThatStatus()
    {
        var id = WorkItemId.Create(Guid.NewGuid());
        var tenantId = TenantId.Create(Guid.NewGuid());
        var title = WorkItemTitle.Create("Fix login bug");
        var createdAt = DateTimeOffset.UtcNow;

        var workItem = WorkItem.Rehydrate(id, tenantId, title, "Some description", WorkItemStatus.InProgress, createdAt);

        workItem.Id.Should().Be(id);
        workItem.TenantId.Should().Be(tenantId);
        workItem.Title.Should().Be(title);
        workItem.Description.Should().Be("Some description");
        workItem.Status.Should().Be(WorkItemStatus.InProgress);
        workItem.CreatedAt.Should().Be(createdAt);
    }

    [Fact]
    public void ChangeTitle_WithNewTitle_UpdatesTitle()
    {
        var workItem = CreateWorkItem("Original Title");
        var newTitle = WorkItemTitle.Create("Updated Title");

        workItem.ChangeTitle(newTitle);

        workItem.Title.Value.Should().Be("Updated Title");
    }

    [Fact]
    public void ChangeDescription_WithNewDescription_UpdatesDescription()
    {
        var workItem = CreateWorkItem("Fix login bug");

        workItem.ChangeDescription("New description text");

        workItem.Description.Should().Be("New description text");
    }

    [Fact]
    public void ChangeDescription_WithNull_ClearsDescription()
    {
        var workItem = CreateWorkItem("Fix login bug");
        workItem.ChangeDescription("Some description");

        workItem.ChangeDescription(null);

        workItem.Description.Should().BeNull();
    }

    [Fact]
    public void ChangeStatus_WithValidTransition_ChangesStatus()
    {
        var workItem = CreateWorkItem("Fix login bug");
        var policy = new DefaultWorkItemStatusPolicy();

        workItem.ChangeStatus(WorkItemStatus.InProgress, policy);

        workItem.Status.Should().Be(WorkItemStatus.InProgress);
    }

    [Fact]
    public void ChangeStatus_WithInvalidTransition_ThrowsDomainException()
    {
        var workItem = CreateWorkItem("Fix login bug");
        var policy = new DefaultWorkItemStatusPolicy();

        Action act = () => workItem.ChangeStatus(WorkItemStatus.Completed, policy);

        act.Should().Throw<InvalidWorkItemStateException>();
    }

    private static WorkItem CreateWorkItem(string title)
    {
        return WorkItem.Create(
            WorkItemId.Create(Guid.NewGuid()),
            TenantId.Create(Guid.NewGuid()),
            WorkItemTitle.Create(title),
            description: "Example description",
            createdAt: DateTimeOffset.UtcNow);
    }
}
