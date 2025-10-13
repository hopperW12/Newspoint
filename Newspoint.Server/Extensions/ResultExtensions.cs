using Microsoft.AspNetCore.Mvc;
using Newspoint.Application.Services;

namespace Newspoint.Server.Extensions;

public static class ResultExtensions
{
    public static IActionResult ToActionResult(this ControllerBase controller, Result result)
    {
        if (result.Success)
            return controller.Ok(new Result { Success = result.Success });

        return result.ErrorType switch
        {
            ResultErrorType.NotFound => controller.NotFound(new Result { Success = result.Success, Message = result.Message }),
            _ => controller.StatusCode(500, new Result{ Success = result.Success, Message = result.Message })
        };
    }

    public static IActionResult ToActionResult<T>(this ControllerBase controller, Result<T> result)
    {
        if (result.Success)
            return controller.Ok(new Result<T> { Success = result.Success, Data = result.Data });

        return result.ErrorType switch
        {
            ResultErrorType.NotFound => controller.NotFound(new Result<T> { Success = result.Success, Message = result.Message }),
            _ => controller.StatusCode(500, new Result<T> { Success = result.Success, Message = result.Message })
        };
    }
}