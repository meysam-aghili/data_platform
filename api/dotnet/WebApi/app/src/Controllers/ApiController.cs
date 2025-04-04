using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Models;
using System.Diagnostics;
using WebApi.Services;
using WebApi.Services.Repositories;


namespace WebApi.Controllers;

[Route("api")]
[ApiController]
public class ApiController : BaseController
{
    private readonly IApiRepository _apis;
    private readonly IApiService _apiService;
    private readonly IJobRepository _jobs;
    private readonly ISwarmRepository _swarm;
    //private readonly JsonSerializerOptions _serializerOptions = new()
    //{
    //    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
    //    WriteIndented = true
    //};

    public ApiController(
        ILogger<ApiController> logger,
        IConfiguration config,
        IApiRepository apis,
        IJobRepository jobs,
        IApiService apiService,
        ISwarmRepository swarm
        ) : base(logger,
            //biapi,
            config)
    {
        _apis = apis;
        _apiService = apiService;
        _jobs = jobs;
        _swarm = swarm;
        _swarm.Key = "api";
    }

    [HttpGet("{apiSlug}")]
    [Authorize]
    public async Task<IActionResult> GetSpResults(string apiSlug)
    {
        var stopwatch = Stopwatch.StartNew();
        var slug = apiSlug.ToLower();
        var api = await _apis.GetAsync(slug);
        if (api is null)
        {
            stopwatch.Stop();
            return await HandleFailure(ResultModel.Failure(ErrorModel.NotFound(slug)), stopwatch.ElapsedMilliseconds);
        }
        var results = await _apiService.GetApiCallResultsAsync(api, HttpContext.Request.QueryString.Value);
        stopwatch.Stop();
        return await Handle(results, stopwatch.ElapsedMilliseconds);
    }

    [HttpGet("{apiSlug}/params")]
    public async Task<IActionResult> GetSpParameters(string apiSlug)
    {
        var stopwatch = Stopwatch.StartNew();
        var slug = apiSlug.ToLower();
        var api = await _apis.GetAsync(slug);
        if (api is null)
        {
            stopwatch.Stop();
            return await HandleFailure(ResultModel.Failure(ErrorModel.NotFound(slug)), stopwatch.ElapsedMilliseconds);
        }
        var results = await _apiService.GetApiParamsAsync(api);
        return Ok(results);
    }

    [HttpPost("job/{jobSlug}")]
    [Authorize]
    public async Task<IActionResult> PostExecuteJobAsync(
        string jobSlug,
        [FromBody(EmptyBodyBehavior = Microsoft.AspNetCore.Mvc.ModelBinding.EmptyBodyBehavior.Allow)]
        Dictionary<string, object>? payload)
    {
        var stopwatch = Stopwatch.StartNew();
        var slug = jobSlug.ToLower();
        var job = await _jobs.GetAsync(slug);
        if (job is null)
        {
            stopwatch.Stop();
            return await HandleFailure(ResultModel.Failure(ErrorModel.NotFound(slug)), stopwatch.ElapsedMilliseconds);
        }
        var results = await _apiService.GetJobResultsAsync(job, payload);
        stopwatch.Stop();
        return await Handle(results, stopwatch.ElapsedMilliseconds);
    }

    [HttpPut("{apiSlug}/update-cache")]
    public IActionResult UpdateCache(string apiSlug)
    {
        _apis.Uncache(apiSlug);
        return NoContent();
    }
}
