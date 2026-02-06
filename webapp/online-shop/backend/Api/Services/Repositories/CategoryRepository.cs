using Api.Data;
using Microsoft.EntityFrameworkCore;
using Shared.Models;

namespace Api.Services.Repositories;

public class CategoryRepository(ApiDbContext context): DbRepository<Category>(context)
{
    public IQueryable<Category> GetBaseQueryable() =>
        GetQueryable()
        .AsNoTracking()
        .AsSplitQuery();
    public async Task<Category?> GetAsync(string slug) =>
        await GetBaseQueryable().SingleOrDefaultAsync(r => r.Slug == slug);
}
