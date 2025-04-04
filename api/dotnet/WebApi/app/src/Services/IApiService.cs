using WebApi.Models;


namespace WebApi.Services;

public interface IApiService
{
    Task<ResultModel> GetApiCallResultsAsync(ApiBsonDocument api, string? query);
    Task<ResultModel> GetJobResultsAsync(SwarmJobBsonDocument job, Dictionary<string, object>? payload);
    Task<List<SqlStoredProcedureParameterModel>> GetApiParamsAsync(ApiBsonDocument api);
    Task<List<string>> GetUserRoleClaimsAsync(string username);
    Task<string?> GetAuthorAsync(HttpContext context, string? slug);
    string? ExtractSlug(HttpContext context);
}
