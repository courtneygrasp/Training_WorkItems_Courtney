using AwesomeAssertions;
using Training.WorkItems.Domain.WorkItems.ValueTypes;

namespace Training.WorkItems.Tests.Domain;

public sealed class WorkItemTitleTests
{
    [Fact]
    public void Create_WithValidTitle_ReturnsWorkItemTitle()
    {
        var title = WorkItemTitle.Create("Fix login bug");

        title.Value.Should().Be("Fix login bug");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithBlankTitle_ThrowsArgumentException(string blank)
    {
        var act = () => WorkItemTitle.Create(blank);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Create_WithTitleExceeding120Characters_ThrowsArgumentException()
    {
        var longTitle = new string('a', 121);

        var act = () => WorkItemTitle.Create(longTitle);

        act.Should().Throw<ArgumentException>()
            .WithMessage("*120*");
    }

    [Fact]
    public void Create_WithTitleAtExactly120Characters_ReturnsWorkItemTitle()
    {
        var title = WorkItemTitle.Create(new string('a', 120));

        title.Value.Length.Should().Be(120);
    }
}
