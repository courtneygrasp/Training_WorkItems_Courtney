using AwesomeAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Training.WorkItems.Api.WorkItems.Controllers;
using Training.WorkItems.Api.WorkItems.Contracts.Requests;
using Training.WorkItems.Api.WorkItems.Contracts.Responses;
using Training.WorkItems.Application.Common;
using Training.WorkItems.Application.WorkItems.UseCases;
using Training.WorkItems.Domain.WorkItems.Enums;

namespace Training.WorkItems.Tests.Api;

public sealed class WorkItemsControllerTests
{
    private readonly Mock<ICreateWorkItemUseCase> _createWorkItem = new(MockBehavior.Strict);
    private readonly Mock<IChangeWorkItemStatusUseCase> _changeWorkItemStatus = new(MockBehavior.Strict);
    private readonly Mock<IGetWorkItemByIdUseCase> _getWorkItemById = new(MockBehavior.Strict);
    private readonly Mock<IListWorkItemsUseCase> _listWorkItems = new(MockBehavior.Strict);
    private readonly Mock<IAddWorkItemNoteUseCase> _addWorkItemNote = new(MockBehavior.Strict);

    private WorkItemsController CreateController() =>
        new(_createWorkItem.Object, _changeWorkItemStatus.Object, _getWorkItemById.Object, _listWorkItems.Object, _addWorkItemNote.Object);

    [Fact]
    public async Task Create_WhenUseCaseSucceeds_Returns201Created()
    {
        var workItemId = Guid.NewGuid();
        _createWorkItem
            .Setup(x => x.ExecuteAsync(It.IsAny<CreateWorkItemCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ApplicationResult<WorkItemResult>.Success(
                new WorkItemResult(workItemId, "Fix login bug", null, "New", DateTimeOffset.UtcNow)));

        var actionResult = await CreateController().Create(
            new CreateWorkItemRequest { Title = "Fix login bug" },
            CancellationToken.None);

        actionResult.Should().BeOfType<CreatedAtActionResult>()
            .Which.StatusCode.Should().Be(201);
    }

    [Fact]
    public async Task Create_WhenUseCaseFails_Returns400BadRequest()
    {
        _createWorkItem
            .Setup(x => x.ExecuteAsync(It.IsAny<CreateWorkItemCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ApplicationResult<WorkItemResult>.Invalid("Title is invalid."));

        var actionResult = await CreateController().Create(
            new CreateWorkItemRequest { Title = " " },
            CancellationToken.None);

        actionResult.Should().BeOfType<BadRequestObjectResult>()
            .Which.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task GetById_WhenFound_Returns200Ok()
    {
        var workItemId = Guid.NewGuid();
        _getWorkItemById
            .Setup(x => x.ExecuteAsync(It.IsAny<GetWorkItemByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ApplicationResult<WorkItemResult>.Success(
                new WorkItemResult(workItemId, "Fix login bug", null, "New", DateTimeOffset.UtcNow)));

        var actionResult = await CreateController().GetById(workItemId, CancellationToken.None);

        actionResult.Should().BeOfType<OkObjectResult>()
            .Which.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task GetById_WhenNotFound_Returns404NotFound()
    {
        _getWorkItemById
            .Setup(x => x.ExecuteAsync(It.IsAny<GetWorkItemByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ApplicationResult<WorkItemResult>.NotFound());

        var actionResult = await CreateController().GetById(Guid.NewGuid(), CancellationToken.None);

        actionResult.Should().BeOfType<NotFoundResult>()
            .Which.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task List_Returns200Ok_WithWorkItemCollection()
    {
        IReadOnlyCollection<WorkItemResult> items =
        [
            new WorkItemResult(Guid.NewGuid(), "Item 1", null, "New", DateTimeOffset.UtcNow),
            new WorkItemResult(Guid.NewGuid(), "Item 2", "Desc", "InProgress", DateTimeOffset.UtcNow)
        ];
        _listWorkItems
            .Setup(x => x.ExecuteAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(ApplicationResult<IReadOnlyCollection<WorkItemResult>>.Success(items));

        var actionResult = await CreateController().List(CancellationToken.None);

        actionResult.Should().BeOfType<OkObjectResult>()
            .Which.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task ChangeStatus_WhenSucceeds_Returns200Ok()
    {
        var workItemId = Guid.NewGuid();
        _changeWorkItemStatus
            .Setup(x => x.ExecuteAsync(It.IsAny<ChangeWorkItemStatusCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ApplicationResult<WorkItemResult>.Success(
                new WorkItemResult(workItemId, "Fix login bug", null, "InProgress", DateTimeOffset.UtcNow)));

        var actionResult = await CreateController().ChangeStatus(
            workItemId,
            new ChangeWorkItemStatusRequest { Status = WorkItemStatus.InProgress },
            CancellationToken.None);

        actionResult.Should().BeOfType<OkObjectResult>()
            .Which.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task ChangeStatus_WhenInvalid_Returns400BadRequest()
    {
        _changeWorkItemStatus
            .Setup(x => x.ExecuteAsync(It.IsAny<ChangeWorkItemStatusCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ApplicationResult<WorkItemResult>.Invalid("Cannot transition."));

        var actionResult = await CreateController().ChangeStatus(
            Guid.NewGuid(),
            new ChangeWorkItemStatusRequest { Status = WorkItemStatus.Closed },
            CancellationToken.None);

        actionResult.Should().BeOfType<BadRequestObjectResult>()
            .Which.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task ChangeStatus_WhenNotFound_Returns404NotFound()
    {
        _changeWorkItemStatus
            .Setup(x => x.ExecuteAsync(It.IsAny<ChangeWorkItemStatusCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ApplicationResult<WorkItemResult>.NotFound());

        var actionResult = await CreateController().ChangeStatus(
            Guid.NewGuid(),
            new ChangeWorkItemStatusRequest { Status = WorkItemStatus.InProgress },
            CancellationToken.None);

        actionResult.Should().BeOfType<NotFoundResult>()
            .Which.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task ChangeStatus_WhenForbidden_Returns403Forbidden()
    {
        _changeWorkItemStatus
            .Setup(x => x.ExecuteAsync(It.IsAny<ChangeWorkItemStatusCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ApplicationResult<WorkItemResult>.Forbidden());

        var actionResult = await CreateController().ChangeStatus(
            Guid.NewGuid(),
            new ChangeWorkItemStatusRequest { Status = WorkItemStatus.Closed },
            CancellationToken.None);

        actionResult.Should().BeOfType<ForbidResult>();
    }

    [Fact]
    public async Task AddNote_WhenUseCaseSucceeds_Returns201Created()
    {
        var workItemId = Guid.NewGuid();
        var noteId = Guid.NewGuid();
        _addWorkItemNote
            .Setup(x => x.ExecuteAsync(It.IsAny<AddWorkItemNoteCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ApplicationResult<AddWorkItemNoteResult>.Success(
                new AddWorkItemNoteResult(noteId, workItemId, "A note.", DateTimeOffset.UtcNow)));

        var actionResult = await CreateController().AddNote(
            workItemId,
            new AddWorkItemNoteRequest { Content = "A note." },
            CancellationToken.None);

        actionResult.Result.Should().BeOfType<CreatedAtActionResult>()
            .Which.StatusCode.Should().Be(201);
    }

    [Fact]
    public async Task AddNote_WhenWorkItemNotFound_Returns404NotFound()
    {
        _addWorkItemNote
            .Setup(x => x.ExecuteAsync(It.IsAny<AddWorkItemNoteCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ApplicationResult<AddWorkItemNoteResult>.NotFound());

        var actionResult = await CreateController().AddNote(
            Guid.NewGuid(),
            new AddWorkItemNoteRequest { Content = "A note." },
            CancellationToken.None);

        actionResult.Result.Should().BeOfType<NotFoundResult>()
            .Which.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task AddNote_WhenNoteTextInvalid_Returns400BadRequest()
    {
        _addWorkItemNote
            .Setup(x => x.ExecuteAsync(It.IsAny<AddWorkItemNoteCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ApplicationResult<AddWorkItemNoteResult>.Invalid("Note text cannot exceed 2000 characters."));

        var actionResult = await CreateController().AddNote(
            Guid.NewGuid(),
            new AddWorkItemNoteRequest { Content = new string('a', 2001) },
            CancellationToken.None);

        actionResult.Result.Should().BeOfType<BadRequestObjectResult>()
            .Which.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task AddNote_ResponseContainsCorrectNoteText()
    {
        var workItemId = Guid.NewGuid();
        var noteId = Guid.NewGuid();
        var createdAt = DateTimeOffset.UtcNow;
        _addWorkItemNote
            .Setup(x => x.ExecuteAsync(It.IsAny<AddWorkItemNoteCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ApplicationResult<AddWorkItemNoteResult>.Success(
                new AddWorkItemNoteResult(noteId, workItemId, "A note.", createdAt)));

        var actionResult = await CreateController().AddNote(
            workItemId,
            new AddWorkItemNoteRequest { Content = "A note." },
            CancellationToken.None);

        var created = actionResult.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
        var response = created.Value.Should().BeOfType<WorkItemNoteResponse>().Subject;
        response.NoteId.Should().Be(noteId);
        response.WorkItemId.Should().Be(workItemId);
        response.NoteText.Should().Be("A note.");
        response.CreatedAt.Should().Be(createdAt);
    }
}
