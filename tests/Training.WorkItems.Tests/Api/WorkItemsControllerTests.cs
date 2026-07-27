using AwesomeAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Training.WorkItems.Api.WorkItems.Controllers;
using Training.WorkItems.Api.WorkItems.Contracts.Requests;
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

    private WorkItemsController CreateController() =>
        new(_createWorkItem.Object, _changeWorkItemStatus.Object, _getWorkItemById.Object, _listWorkItems.Object);

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
}
