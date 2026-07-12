using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Training.WorkItems.Api.WorkItems.Contracts.Requests;
using Training.WorkItems.Api.WorkItems.Contracts.Responses;
using Training.WorkItems.Application.WorkItems.UseCases;

namespace Training.WorkItems.Api.WorkItems;

[ApiController]
[Authorize]
[Route("api/workitems")]
public sealed class WorkItemsController(
    ICreateWorkItemUseCase createWorkItem,
    IChangeWorkItemStatusUseCase changeWorkItemStatus,
    IGetWorkItemByIdUseCase getWorkItemById,
    IListWorkItemsUseCase listWorkItems) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateWorkItemRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateWorkItemCommand(request.Title, request.Description);
        var result = await createWorkItem.ExecuteAsync(command, cancellationToken);

        if (!result.Succeeded)
        {
            return BadRequest(new { error = result.ErrorMessage, failures = result.Failures });
        }

        var response = ToResponse(result.Value!);

        return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetWorkItemByIdQuery(id);
        var result = await getWorkItemById.ExecuteAsync(query, cancellationToken);

        if (!result.Succeeded)
        {
            return NotFound();
        }

        return Ok(ToResponse(result.Value!));
    }

    [HttpGet]
    public async Task<IActionResult> List(CancellationToken cancellationToken)
    {
        var result = await listWorkItems.ExecuteAsync(cancellationToken);

        var responses = result.Value!.Select(ToResponse).ToList();

        return Ok(new ListWorkItemsResponse(responses, Page: 0, PageSize: responses.Count, TotalCount: responses.Count));
    }

    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> ChangeStatus(
        Guid id,
        [FromBody] ChangeWorkItemStatusRequest request,
        CancellationToken cancellationToken)
    {
        var command = new ChangeWorkItemStatusCommand(id, request.Status);
        var result = await changeWorkItemStatus.ExecuteAsync(command, cancellationToken);

        if (!result.Succeeded)
        {
            return BadRequest(new { error = result.ErrorMessage, failures = result.Failures });
        }

        return Ok(ToResponse(result.Value!));
    }

    private static WorkItemResponse ToResponse(WorkItemResult r) =>
        new(r.Id, r.Title, r.Description, r.Status, r.CreatedAt);
}
