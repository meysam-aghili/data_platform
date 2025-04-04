using System.Net;
using Microsoft.AspNetCore.Mvc;
using WebApi.Shared;
using WebApi.Models;


namespace WebApi.Controllers;

[ApiController]
public abstract class BaseController : ControllerBase
{
    private readonly ILogger<BaseController> _logger;

    protected BaseController(ILogger<BaseController> logger, IConfiguration config)
    {
        _logger = logger;
    }

    protected async Task<IActionResult> HandleSuccess(ResultModel result, long durationMs)
    {
        HttpContext.Response.StatusCode = (int)HttpStatusCode.OK;
        LogHelper.LogRequest(_logger, HttpContext, durationMs);
        return Ok(
            new ResponseModel(
                durationMs,
                result.Value?.Count,
                DateTimeOffset.Now,
                result.Metadata,
                result.Value
            )
        );
    }

    protected async Task<IActionResult> HandleFailure(ResultModel result, long durationMs = 0)
    {
        int statusCode = result.Error!.StatusCode;
        HttpContext.Response.StatusCode = 404;//result.Error!.StatusCode;
        var message =
            result.Error!.Status == HttpStatusCode.InternalServerError
            ? "Oops! Something went wrong..."
            : result.Error.Message;
        LogHelper.LogRequest(_logger, HttpContext, durationMs, result.Error);
        return StatusCode(result.Error!.StatusCode,
            new ErrorResponseModel(
                statusCode,
                statusCode.ToString(),
                message,
                DateTimeOffset.Now,
                durationMs
            )
        );
    }

    protected async Task<IActionResult> Handle(ResultModel result, long durationMs)
    {
        if (result.IsSuccess)
        {
            return await HandleSuccess(result, durationMs);
        }
        return await HandleFailure(result, durationMs);
    }

    protected async Task<IActionResult> HandleUnauthorized() => await HandleFailure(ResultModel.Failure(ErrorModel.Unauthorized()));
}
