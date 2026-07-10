using Microsoft.AspNetCore.Mvc;
using Training.WorkItems.Api.WorkItems.Contracts.Requests;
using Training.WorkItems.Api.WorkItems.Contracts.Responses;
using Training.WorkItems.Application.WorkItems.UseCases;

namespace Training.WorkItems.Api.WorkItems;

[ApiController]
[Route("api/workitems")]
public sealed class WorkItemsController(
    ICreateWorkItemUseCase createWorkItem,
    IChangeWorkItemStatusUseCase changeWorkItemStatus) : ControllerBase
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

        var response = new WorkItemResponse(
            result.Value!.Id,
            result.Value.Title,
            result.Value.Description,
            result.Value.Status,
            result.Value.CreatedAt);

        return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
    }

    [HttpGet("{id:guid}")]
    public IActionResult GetById(Guid id)
    {
        return NotFound();
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

        var response = new WorkItemResponse(
            result.Value!.Id,
            result.Value.Title,
            result.Value.Description,
            result.Value.Status,
            result.Value.CreatedAt);

        return Ok(response);
    }
}
