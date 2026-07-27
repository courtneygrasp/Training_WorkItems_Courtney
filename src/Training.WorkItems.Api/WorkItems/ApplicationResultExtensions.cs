using Microsoft.AspNetCore.Mvc;
using Training.WorkItems.Application.Common;

namespace Training.WorkItems.Api.WorkItems;

internal static class ApplicationResultExtensions
{
    public static IActionResult ToActionResult<T>(
        this ApplicationResult<T> result,
        Func<T, IActionResult> onSuccess)
    {
        return result.Status switch
        {
            ResultStatus.Success => onSuccess(result.Value!),
            ResultStatus.Invalid => new BadRequestObjectResult(
                new { error = result.ErrorMessage, failures = result.Failures }),
            ResultStatus.NotFound => new NotFoundResult(),
            ResultStatus.Forbidden => new ForbidResult(),
            _ => new StatusCodeResult(StatusCodes.Status500InternalServerError)
        };
    }
}
