using WebApi.Models;


namespace WebApi.Services.Repositories;

public interface IApiRepository
{
    Task<(IReadOnlyList<ApiBsonDocument> items, int count)> GetAsync(
        string? createdBy = null,
        string? query = null,
        int page = 0,
        int pageSize = 0);
    Task<ApiBsonDocument?> GetAsync(string slug);
    //Task CreateAsync(Api api);
    //Task UpdateAsync(Api api);
    //Task DeleteAsync(Api api);
    //Task<bool> SlugIsTakenAsync(string slug);
    void Uncache(string slug);
}
