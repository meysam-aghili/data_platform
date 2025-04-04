using System.Diagnostics;
using System.Net;
using WebApi.Models;
using WebApi.Services;
using WebApi.Shared;


namespace WebApi.Middlewares;

public class ApiMiddleware
{
    private readonly ILogger<ApiMiddleware> _logger;
    private readonly IApiService _apiService;
    private readonly RequestDelegate _next;

    public ApiMiddleware(
        RequestDelegate next,
        ILogger<ApiMiddleware> logger,
        IApiService apiService,
        IConfiguration config)
    {
        _logger = logger;
        _apiService = apiService;
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            string? slug = _apiService.ExtractSlug(context);
            if (slug is not null)
            {
                Task<string?> authorTask = _apiService.GetAuthorAsync(context, slug);

                if (context.User.Identity?.IsAuthenticated ?? false)
                {
                    if (context.User.IsInRole(RoleNames.Admin) || context.User.IsInRole(slug))
                    {
                        await _next(context);
                        stopwatch.Stop();
                    }
                    else
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                        await context.Response.WriteAsync("Forbidden");
                        stopwatch.Stop();
                        LogHelper.LogRequest(_logger, context, stopwatch.ElapsedMilliseconds, ErrorModel.Forbidden());
                        return;
                    }
                }
                else
                {
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    await context.Response.WriteAsync("Unauthorized. Obtain a new token.");
                    stopwatch.Stop();
                    LogHelper.LogRequest(_logger, context, stopwatch.ElapsedMilliseconds, ErrorModel.Unauthorized());
                    return;
                }
            }
            else if (context.Request.Path.StartsWithSegments("/token", out var tokenRemainingPath))
            {
                await _next(context);
            }
            else
            {
                await _next(context);
                if (context.Response.StatusCode >= 300)
                {
                    LogHelper.LogRequest(_logger, context, stopwatch.ElapsedMilliseconds, ErrorModel.InternalServerError(null));
                    return;
                }
            }
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            LogHelper.LogRequest(_logger, context, stopwatch.ElapsedMilliseconds, ErrorModel.InternalServerError(null));

            throw;
        }
    }
}
