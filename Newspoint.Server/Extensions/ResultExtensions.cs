using Microsoft.AspNetCore.Mvc;
using Newspoint.Application.Services;

namespace Newspoint.Server.Extensions;

public static class ResultExtensions
{
    public static IActionResult ToActionResult(this ControllerBase controller, Result result)
    {
        if (result.Success)
            return controller.Ok(new { success = result.Success});

        return result.ErrorType switch
        {
            ResultErrorType.NotFound => controller.NotFound(new { success = result.Success, message = result.Message}),
            _ => controller.StatusCode(500, new { success = result.Success, message = result.Message})
        };
    }
    
    public static IActionResult ToActionResult<T>(this ControllerBase controller, Result<T> result)
    {
        if (result.Success)
            return controller.Ok(new { success = result.Success, data = result.Data});

        return result.ErrorType switch
        {
            ResultErrorType.NotFound => controller.NotFound(new { success = result.Success, message = result.Message}),
            _ => controller.StatusCode(500, new { success = result.Success, message = result.Message})
        };
    }
}