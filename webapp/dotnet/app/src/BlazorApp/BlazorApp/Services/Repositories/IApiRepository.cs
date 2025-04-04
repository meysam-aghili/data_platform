using BlazorApp.Models;


namespace BlazorApp.Services.Repositories;

public interface IApiRepository
{
    Task<(IReadOnlyList<ApiBsonDocument> items, int count)> GetAsync(
        string? createdBy = null,
        string? query = null,
        int page = 0,
        int pageSize = 0);
    Task<ApiBsonDocument?> GetAsync(string slug);
    Task CreateAsync(ApiBsonDocument api);
    Task UpdateAsync(ApiBsonDocument api);
    Task DeleteAsync(ApiBsonDocument api);
    Task<bool> SlugIsTakenAsync(string slug);
    void Uncache(string slug);
}
