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
