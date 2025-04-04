using System.Data;
using System.Net;
using System.Web;
using MongoDB.Driver;
using Nest;
using WebApi.Models;
using WebApi.Services.Repositories;
using WebApi.Shared;
using Microsoft.Data.SqlClient;
using WebApi.Services.Sql;


namespace WebApi.Services;

public class ApiService : IApiService
{
    private readonly ISqlServerProviderService _sqlProvider;
    private readonly ISqlService _sql;
    private readonly IApiRepository _apis;
    private readonly IJobRepository _jobs;
    private readonly IRoleRepository _roles;
    private readonly ISwarmRepository _swarm;
    private readonly SqlCredentialModel _user;
    private readonly SqlCredentialModel _userUm;

    public ApiService(
        ISqlServerProviderService sqlProvider,
        [FromKeyedServices("mssql")] ISqlService sql,
        IApiRepository apis,
        IJobRepository jobs,
        ISwarmRepository swarm,
        IRoleRepository roles)
    {
        _sqlProvider = sqlProvider;
        _sql = sql;
        _apis = apis;
        _jobs = jobs;
        _roles = roles;
        _swarm = swarm;
        _swarm.Key = "konductor-api";

        _user = _sqlProvider.GetCredentials(SqlUser.SqlApiUser);
        _userUm = _sqlProvider.GetCredentials(SqlUser.SqlApiUnmaskedUser);
    }

    public async Task<List<string>> GetUserRoleClaimsAsync(string username)
    {
        Task<RoleModel> AdminRoleTask = _roles.GetAsync(RoleNames.Admin);
        Task<RoleModel> AdvancedAnalyticsAdminRoleTask = _roles.GetAsync(RoleNames.AdvancedAnalytics);
        await Task.WhenAll(AdminRoleTask, AdvancedAnalyticsAdminRoleTask);
        RoleModel AdminRole = await AdminRoleTask;
        RoleModel AdvancedAnalyticsAdminRole = await AdvancedAnalyticsAdminRoleTask;

        if (AdminRole.ContainsUser(username) || AdvancedAnalyticsAdminRole.ContainsUser(username))
        {
            return ["admin"];
        }

        Task<(IReadOnlyList<ApiBsonDocument>, int)> apisTask = _apis.GetAsync();
        Task<(IReadOnlyList<SwarmJobBsonDocument>, int)> jobsTask = _jobs.GetAsync();

        await Task.WhenAll(apisTask, jobsTask);
        var (apis, _) = await apisTask;
        var (jobs, _) = await jobsTask;

        var roles =
            from api in apis
            where api.Users.Contains(username, StringComparer.OrdinalIgnoreCase)
            || string.Equals(api.CreatedBy, username, StringComparison.OrdinalIgnoreCase)
            select api.Slug;

        var apiAuthors =
            from api in apis
            where string.Equals(api.CreatedBy, username, StringComparison.OrdinalIgnoreCase)
            select api.Slug;

        var jobAuthors =
            from job in jobs
            where string.Equals(job.CreatedBy, username, StringComparison.OrdinalIgnoreCase)
            select job.Slug;

        return roles.Concat(apiAuthors).Concat(jobAuthors).Distinct().ToList();
    }

    public async Task<ResultModel> GetApiCallResultsAsync(ApiBsonDocument api, string? query)
    {
        var inputs = HttpUtility.ParseQueryString(query ?? string.Empty);
        SqlOptionModel options = GetSqlOptions(api);
        try
        {
            var results = await _sql.ExecuteStoredProcedureAsync(api.StoredProcedure, inputs, options);
            var metadata = from p in results.OutputParameters where p.Name.StartsWith("metadata_") select p;
            var metadataDictionary = metadata.ToDictionary(
                kvp => kvp.Name.Replace("metadata_", string.Empty),
                kvp => kvp.Value);
            return ResultModel.Success(results.ResultSets?.FirstOrDefault(), metadataDictionary);
        }
        catch (SqlException ex) when (ex.Number == -2)
        {
            return ResultModel.Failure(ErrorModel.Timeout());
        }
        catch (SqlException ex) when (ex.Number == 201)
        {
            return ResultModel.Failure(ErrorModel.BadRequest(ex.Message));
        }
        catch (SqlException ex) when (ex.Number == 999999)
        {
            return ResultModel.Failure(ErrorModel.BadRequest(ex.Message));
        }
        catch (FormatException ex)
        {
            return ResultModel.Failure(ErrorModel.BadRequest(ex.Message));
        }
        catch (Exception ex)
        {
            return ResultModel.Failure(ErrorModel.InternalServerError(ex.Message, ex));
        }
    }

    private SqlCredentialModel GetCredentials(ApiBsonDocument api) => api.Unmasked switch
    {
        true => _userUm,
        false => _user
    };

    private SqlOptionModel GetSqlOptions(ApiBsonDocument api) => new()
    {
        Database = api.StoredProcedure.Database,
        Credentials = GetCredentials(api),
        PoolSize = 100
    };

    public string? ExtractSlug(HttpContext context)
    {
        var path = context.Request.Path;
        if (path.StartsWithSegments("/api", out var remainingPath))
        {
            if (remainingPath.HasValue && !string.IsNullOrWhiteSpace(remainingPath.Value))
            {
                var slug = remainingPath.Value.TrimStart('/');

                if (slug.Contains('/'))
                {
                    return null;
                }
                return slug;
            }
        }
        return null;
    }

    public async Task<string?> GetAuthorAsync(HttpContext context, string? slug)
    {
        if (string.IsNullOrEmpty(slug))
        {
            return null;
        }
        if (string.Equals(context.Request.Method, HttpMethod.Get.ToString(), StringComparison.OrdinalIgnoreCase))
        {
            var api = await _apis.GetAsync(slug);
            return api?.CreatedBy;
        }
        if (string.Equals(context.Request.Method, HttpMethod.Post.ToString(), StringComparison.OrdinalIgnoreCase))
        {
            var job = await _jobs.GetAsync(slug);
            return job?.CreatedBy;
        }
        return null;
    }

    public async Task<List<SqlStoredProcedureParameterModel>> GetApiParamsAsync(ApiBsonDocument api)
    {
        SqlOptionModel options = GetSqlOptions(api);
        List<SqlStoredProcedureParameterModel> results = await _sql.GetStoredProcedureParametersAsync(api.StoredProcedure, options);
        return results;
    }

    public async Task<ResultModel> GetJobResultsAsync(SwarmJobBsonDocument job, Dictionary<string, object>? parameters)
    {
        var container = job.ToContainer();
        foreach (var p in parameters ?? [])
        {
            container.Environment[$"PARAM_{p.Key}"] = p.Value.ToString() ?? string.Empty;
        }
        try
        {
            return await _swarm.RunContainerAsync(job.ToContainer());
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.BadRequest)
        {
            return ResultModel.Failure(ErrorModel.BadRequest(string.Empty));
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.RequestTimeout)
        {
            return ResultModel.Failure(ErrorModel.Timeout());
        }
        catch (Exception ex)
        {
            return ResultModel.Failure(ErrorModel.InternalServerError(ex.Message + '\n' + ex.StackTrace, ex));
        }
    }

    //public async Task<List<string>> GetUserRoleClaimsAsync(string username)
    //{
    //    Task<Role> adminRoleTask = _roles.GetAsync(RoleNames.Admin);
    //    Task<Role> biapiAdminRoleTask = _roles.GetAsync(RoleNames.BiApiAdmin);
    //    await Task.WhenAll(adminRoleTask, biapiAdminRoleTask);
    //    Role admin = await adminRoleTask;
    //    Role biapiAdmin = await biapiAdminRoleTask;

    //    if (admin.ContainsUser(username) || biapiAdmin.ContainsUser(username))
    //    {
    //        return ["biadmin"];
    //    }

    //    Task<(IReadOnlyList<Api>, int)> apisTask = _apis.GetAsync();
    //    Task<(IReadOnlyList<SwarmJob>, int)> jobsTask = _jobs.GetAsync();

    //    await Task.WhenAll(apisTask, jobsTask);
    //    var (apis, _) = await apisTask;
    //    var (jobs, _) = await jobsTask;

    //    var roles =
    //        from api in apis
    //        where api.Users.Contains(username, StringComparer.OrdinalIgnoreCase)
    //        || string.Equals(api.CreatedBy, username, StringComparison.OrdinalIgnoreCase)
    //        select api.Slug;

    //    var apiAuthors =
    //        from api in apis
    //        where string.Equals(api.CreatedBy, username, StringComparison.OrdinalIgnoreCase)
    //        select api.Slug;

    //    var jobAuthors =
    //        from job in jobs
    //        where string.Equals(job.CreatedBy, username, StringComparison.OrdinalIgnoreCase)
    //        select job.Slug;

    //    return roles.Concat(apiAuthors).Concat(jobAuthors).Distinct().ToList();
    //}
}