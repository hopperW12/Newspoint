using Microsoft.AspNetCore.Mvc;
using Newspoint.Application.Services;
using Newspoint.Server.Interfaces;

namespace Newspoint.Server.Extensions;

public static class ResultExtensions
{
    public static IActionResult ToActionResult(this ControllerBase controller, Result result)
    {
        if (result.Success)
            return controller.Ok(new Result { Success = result.Success });

        return result.ErrorType switch
        {
            ResultErrorType.NotFound => controller.NotFound(
                new Result { Success = result.Success, Message = result.Message }),
            _ => controller.StatusCode(
                500, 
                new Result { Success = result.Success, Message = result.Message })
        };
    }

    public static IActionResult ToActionResult<TSource, TDestination>(
        this ControllerBase controller, Result<TSource> result,
        IMapper<TSource, TDestination> mapper)
        where TDestination : IEntityDto
    {
        // Handle errors
        if (!result.Success)
            return result.ErrorType switch
            {
                ResultErrorType.NotFound => controller.NotFound(
                    new Result<TDestination> { Success = result.Success, Message = result.Message }),
                _ => controller.StatusCode(500,
                    new Result<TDestination> { Success = result.Success, Message = result.Message })
            };
        
        // Map data
        var mappedData = result.Data is not null ? mapper.Map(result.Data) : default;
        return controller.Ok(new Result<TDestination> { Success = true, Data = mappedData });

    }
}