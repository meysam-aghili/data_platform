using Microsoft.Extensions.Caching.Memory;
using WebApi.Models;
using WebApi.Services;
using WebApi.Services.Repositories;


namespace WebApi.Services.Repositories;

public class JobRepository(IMongoService<SwarmJobBsonDocument> jobs, IMemoryCache cache) : IJobRepository
{
    private readonly IMemoryCache _cache = cache;
    private readonly IMongoService<SwarmJobBsonDocument> _jobs = jobs;
    private readonly MemoryCacheEntryOptions _cacheOptions = new()
    {
        SlidingExpiration = TimeSpan.FromHours(6),
        AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1)
    };

    public async Task<(IReadOnlyList<SwarmJobBsonDocument> items, int count)> GetAsync(
        string? createdBy = null,
        string? query = null,
        int page = 0,
        int pageSize = 0)
    {
        return await _jobs.GetAsync(createdBy, query, page, pageSize);
    }

    public async Task<SwarmJobBsonDocument?> GetAsync(string slug)
    {
        if (_cache.TryGetValue(slug, out SwarmJobBsonDocument? job))
        {
            return job;
        }

        job = await GetFromDatabaseAsync(slug);
        _cache.Set(slug, job, _cacheOptions);
        return job;
    }

    private async Task<SwarmJobBsonDocument?> GetFromDatabaseAsync(string slug) =>
        await Task.FromResult((
            from api in _jobs.Query()
            where api.Slug == slug
            select api
    ).FirstOrDefault());

    //public async Task CreateAsync(SwarmJobBsonDocument job) => await _jobs.CreateAsync(job);

    //public async Task UpdateAsync(SwarmJobBsonDocument job) => await _jobs.UpdateAsync(job);

    //public async Task DeleteAsync(SwarmJobBsonDocument job) => await _jobs.DeleteAsync(job);
}
