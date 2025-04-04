using BlazorApp.Models;


namespace BlazorApp.Services.Repositories;

public interface IJobRepository
{
    public Task<(IReadOnlyList<SwarmJobBsonDocument> items, int count)> GetAsync(
        string? createdBy = null,
        string? query = null,
        int page = 0,
        int pageSize = 0);
    Task<SwarmJobBsonDocument?> GetAsync(string slug);
    //Task CreateAsync(SwarmJobBsonDocument job);
    //Task UpdateAsync(SwarmJobBsonDocument job);
    //Task DeleteAsync(SwarmJobBsonDocument job);
}
