using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Training.WorkItems.Api.WorkItems.Contracts.Requests;
using Training.WorkItems.Api.WorkItems.Contracts.Responses;
using Training.WorkItems.Application.WorkItems.UseCases;

namespace Training.WorkItems.Api.WorkItems.Controllers;

[ApiController]
[Authorize]
[Route("api/workitems")]
public sealed class WorkItemsController(
    ICreateWorkItemUseCase createWorkItem,
    IChangeWorkItemStatusUseCase changeWorkItemStatus,
    IGetWorkItemByIdUseCase getWorkItemById,
    IListWorkItemsUseCase listWorkItems,
    IAddWorkItemNoteUseCase addWorkItemNote) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateWorkItemRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateWorkItemCommand(request.Title, request.Description);
        var result = await createWorkItem.ExecuteAsync(command, cancellationToken);

        return result.ToActionResult(workItem =>
            CreatedAtAction(nameof(GetById), new { id = workItem.Id }, ToResponse(workItem)));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetWorkItemByIdQuery(id);
        var result = await getWorkItemById.ExecuteAsync(query, cancellationToken);

        return result.ToActionResult(workItem => Ok(ToResponse(workItem)));
    }

    [HttpGet]
    public async Task<IActionResult> List(CancellationToken cancellationToken)
    {
        var result = await listWorkItems.ExecuteAsync(cancellationToken);

        return result.ToActionResult(items =>
        {
            var responses = items.Select(ToResponse).ToList();
            return Ok(new ListWorkItemsResponse(responses, Page: 0, PageSize: responses.Count, TotalCount: responses.Count));
        });
    }

    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> ChangeStatus(
        Guid id,
        [FromBody] ChangeWorkItemStatusRequest request,
        CancellationToken cancellationToken)
    {
        var command = new ChangeWorkItemStatusCommand(id, request.Status);
        var result = await changeWorkItemStatus.ExecuteAsync(command, cancellationToken);

        return result.ToActionResult(workItem => Ok(ToResponse(workItem)));
    }

    [HttpPost("{id:guid}/notes")]
    public async Task<IActionResult> AddNote(
        Guid id,
        [FromBody] AddWorkItemNoteRequest request,
        CancellationToken cancellationToken)
    {
        var command = new AddWorkItemNoteCommand(id, request.Content);
        var result = await addWorkItemNote.ExecuteAsync(command, cancellationToken);

        return result.ToActionResult(note =>
            CreatedAtAction(nameof(AddNote), new { id }, ToNoteResponse(note)));
    }

    private static WorkItemResponse ToResponse(WorkItemResult r) =>
        new(r.Id, r.Title, r.Description, r.Status, r.CreatedAt);

    private static WorkItemNoteResponse ToNoteResponse(AddWorkItemNoteResult r) =>
        new(r.NoteId, r.WorkItemId, r.Content, r.CreatedAt);
}
