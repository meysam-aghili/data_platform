using Api.Data;
using Microsoft.EntityFrameworkCore;
using Shared.Models;

namespace Api.Services.Repositories;

public class CityRepository(ApiDbContext context) : DbRepository<City>(context)
{
    public IQueryable<City> GetBaseQueryable() =>
        GetQueryable()
        .Include(c => c.State)
        .AsNoTracking()
        .AsSplitQuery();
    public override async Task<List<City>> GetAsync() =>
        await GetBaseQueryable().ToListAsync();

    public async Task<City?> GetAsync(string slug, string stateSlug) =>
        await GetBaseQueryable().SingleOrDefaultAsync(c => c.Slug == slug && c.State.Slug == stateSlug);
}
