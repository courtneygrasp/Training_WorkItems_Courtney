using AwesomeAssertions;
using Training.WorkItems.Domain.WorkItems.ValueTypes;

namespace Training.WorkItems.Tests.Domain;

public sealed class WorkItemNoteTextTests
{
    [Fact]
    public void Create_WithValidText_ReturnsValue()
    {
        var text = WorkItemNoteText.Create("This is a note.");

        text.Value.Should().Be("This is a note.");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithBlankText_ThrowsArgumentException(string value)
    {
        var act = () => WorkItemNoteText.Create(value);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Create_WithTextExceeding2000Characters_ThrowsArgumentException()
    {
        var longText = new string('a', 2001);

        var act = () => WorkItemNoteText.Create(longText);

        act.Should().Throw<ArgumentException>()
            .WithMessage("*2000*");
    }

    [Fact]
    public void Create_WithExactly2000Characters_Succeeds()
    {
        var maxText = new string('a', 2000);

        var act = () => WorkItemNoteText.Create(maxText);

        act.Should().NotThrow();
    }

    [Fact]
    public void ToString_ReturnsRawValue()
    {
        var text = WorkItemNoteText.Create("Note content.");

        text.ToString().Should().Be("Note content.");
    }
}
