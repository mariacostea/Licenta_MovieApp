using Microsoft.AspNetCore.Mvc;

namespace MobyLabWebProgramming.Core.Responses;

public static class ServiceResponseExtensions
{
    public static IActionResult FromServiceResponse(this ControllerBase controller, ServiceResponse response)
        => response.IsOk ? controller.Ok() : controller.BadRequest(response.Error);

    public static IActionResult FromServiceResponse<T>(this ControllerBase controller, ServiceResponse<T> response)
        => response.IsOk ? controller.Ok(response.Result) : controller.BadRequest(response.Error);
}