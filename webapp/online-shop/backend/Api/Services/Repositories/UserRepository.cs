using Api.Data;
using Microsoft.EntityFrameworkCore;
using Shared.Models;

namespace Api.Services.Repositories;

public class UserRepository(ApiDbContext context) : DbRepository<User>(context)
{
    public IQueryable<User> GetBaseQueryable() =>
        GetQueryable()
        .Include(u => u.UserAddresses!.Where(ua => !ua.IsDeleted))
        .ThenInclude(ua => ua.City)
        .ThenInclude(c => c.State);

    public async Task<User?> GetAsync(string slug, bool track = false) =>
        track ? await GetBaseQueryable().SingleOrDefaultAsync(u => u.Slug == slug) :
        await GetBaseQueryable().AsNoTracking().SingleOrDefaultAsync(u => u.Slug == slug);
}

