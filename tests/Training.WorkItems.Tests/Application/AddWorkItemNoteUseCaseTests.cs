using AwesomeAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Training.WorkItems.Application.Common;
using Training.WorkItems.Application.WorkItems.UseCases;
using Training.WorkItems.Domain.WorkItems.Entities;
using Training.WorkItems.Domain.WorkItems.ValueTypes;
using Training.WorkItems.Tests.Application.Time;
using Training.WorkItems.Tests.Application.WorkItems.Fakes;

namespace Training.WorkItems.Tests.Application;

public sealed class AddWorkItemNoteUseCaseTests
{
    private readonly InMemoryWorkItemRepository _workItems = new();
    private readonly InMemoryWorkItemNoteRepository _notes = new();
    private readonly FakeSystemClock _clock = new(new DateTimeOffset(2026, 8, 23, 10, 0, 0, TimeSpan.Zero));
    private readonly TenantId _tenantId = TenantId.Create(Guid.NewGuid());

    [Fact]
    public async Task ExecuteAsync_WithValidNoteAndExistingWorkItem_SavesNoteAndReturnsSuccess()
    {
        var workItem = await SeedWorkItemAsync(_tenantId);
        var useCase = CreateUseCase();
        var command = new AddWorkItemNoteCommand(workItem.Id.Value, "This is a note.");

        var result = await useCase.ExecuteAsync(command, CancellationToken.None);

        result.Succeeded.Should().BeTrue();
        result.Value!.NoteText.Should().Be("This is a note.");
        result.Value.WorkItemId.Should().Be(workItem.Id.Value);
        result.Value.CreatedAt.Should().Be(_clock.UtcNow);
        _notes.Notes.Should().ContainSingle();
    }

    [Fact]
    public async Task ExecuteAsync_WhenWorkItemNotFound_ReturnsNotFound()
    {
        var useCase = CreateUseCase();
        var command = new AddWorkItemNoteCommand(Guid.NewGuid(), "Note text.");

        var result = await useCase.ExecuteAsync(command, CancellationToken.None);

        result.Status.Should().Be(ResultStatus.NotFound);
        _notes.Notes.Should().BeEmpty();
    }

    [Fact]
    public async Task ExecuteAsync_WhenWorkItemBelongsToOtherTenant_ReturnsNotFound()
    {
        var otherTenant = TenantId.Create(Guid.NewGuid());
        var workItem = await SeedWorkItemAsync(otherTenant);
        var useCase = CreateUseCase();

        var command = new AddWorkItemNoteCommand(workItem.Id.Value, "Note text.");
        var result = await useCase.ExecuteAsync(command, CancellationToken.None);

        result.Status.Should().Be(ResultStatus.NotFound);
        _notes.Notes.Should().BeEmpty();
    }

    [Fact]
    public async Task ExecuteAsync_WithBlankNoteText_ReturnsInvalid()
    {
        var workItem = await SeedWorkItemAsync(_tenantId);
        var useCase = CreateUseCase();

        var command = new AddWorkItemNoteCommand(workItem.Id.Value, "   ");
        var result = await useCase.ExecuteAsync(command, CancellationToken.None);

        result.Status.Should().Be(ResultStatus.Invalid);
        _notes.Notes.Should().BeEmpty();
    }

    [Fact]
    public async Task ExecuteAsync_WithTooLongNoteText_ReturnsInvalid()
    {
        var workItem = await SeedWorkItemAsync(_tenantId);
        var useCase = CreateUseCase();

        var command = new AddWorkItemNoteCommand(workItem.Id.Value, new string('a', 2001));
        var result = await useCase.ExecuteAsync(command, CancellationToken.None);

        result.Status.Should().Be(ResultStatus.Invalid);
        _notes.Notes.Should().BeEmpty();
    }

    [Fact]
    public async Task ExecuteAsync_SavedNote_HasTenantScopedToCurrentUser()
    {
        var workItem = await SeedWorkItemAsync(_tenantId);
        var useCase = CreateUseCase();
        var command = new AddWorkItemNoteCommand(workItem.Id.Value, "A note.");

        await useCase.ExecuteAsync(command, CancellationToken.None);

        _notes.Notes.Single().TenantId.Should().Be(_tenantId);
    }

    private AddWorkItemNoteUseCase CreateUseCase() =>
        new(_workItems, _notes, new FakeCurrentUserContext(_tenantId), _clock,
            NullLogger<AddWorkItemNoteUseCase>.Instance);

    private async Task<WorkItem> SeedWorkItemAsync(TenantId tenantId)
    {
        var workItem = WorkItem.Create(
            WorkItemId.Create(Guid.NewGuid()),
            tenantId,
            WorkItemTitle.Create("Test item"),
            null,
            _clock.UtcNow);

        await _workItems.AddAsync(workItem, CancellationToken.None);
        return workItem;
    }
}
