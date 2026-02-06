using Api.Shared;
using System.Diagnostics;
using System.Net;

namespace Api.Middlewares;

public class MainMiddleware(RequestDelegate next, ILogger<MainMiddleware> logger)
{
    private readonly ILogger<MainMiddleware> _logger = logger;
    private readonly RequestDelegate _next = next;
    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        try
        {
            string? username = context.User.Identity?.Name;
            await _next(context);
        }
        catch (Exception)
        {
            stopwatch.Stop();
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            var (logMessage, logParams) = LogHelpers.GetLog(context, stopwatch.ElapsedMilliseconds);
            _logger.LogError(logMessage, logParams);
            throw;
        }
        finally
        {
            stopwatch.Stop();
            var (logMessage, logParams) = LogHelpers.GetLog(context, stopwatch.ElapsedMilliseconds);
            _logger.LogInformation(logMessage, logParams);
        }
    }
}
