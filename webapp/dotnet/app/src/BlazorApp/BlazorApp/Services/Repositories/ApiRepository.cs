using Microsoft.Extensions.Caching.Memory;
using BlazorApp.Models;
using BlazorApp.Services.Mongo;


namespace BlazorApp.Services.Repositories;

public class ApiRepository(IMongoService<ApiBsonDocument> apis, IMemoryCache cache) : IApiRepository
{
    private readonly IMongoService<ApiBsonDocument> _apis = apis;
    private readonly IMemoryCache _cache = cache;
    private readonly MemoryCacheEntryOptions _cacheOptions = new()
    {
        SlidingExpiration = TimeSpan.FromHours(6),
        AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1)
    };

    public async Task<(IReadOnlyList<ApiBsonDocument> items, int count)> GetAsync(
        string? createdBy = null,
        string? query = null,
        int page = 0,
        int pageSize = 0)
    {
        return await _apis.GetAsync(createdBy, query, page, pageSize);
    }

    public async Task<ApiBsonDocument?> GetAsync(string slug)
    {
        if (_cache.TryGetValue(slug, out ApiBsonDocument? api))
        {
            return api;
        }

        var (queried, _) = await GetAsync(query: slug, pageSize: 1);
        api = queried.FirstOrDefault();

        if (api is not null)
        {
            _cache.Set(slug, api, _cacheOptions);
        }
        return api;
    }

    public async Task CreateAsync(ApiBsonDocument api) => await _apis.CreateAsync(api);

    public async Task UpdateAsync(ApiBsonDocument api)
    {
        await _apis.UpdateAsync(api);
        Uncache(api.Slug);
    }

    public async Task DeleteAsync(ApiBsonDocument api)
    {
        await _apis.DeleteAsync(api);
        Uncache(api.Slug);
    }

    public async Task<bool> SlugIsTakenAsync(string slug) => await _apis.ExistsAsync(slug);

    public void Uncache(string? slug) => _cache.Remove(slug ?? string.Empty);
}
