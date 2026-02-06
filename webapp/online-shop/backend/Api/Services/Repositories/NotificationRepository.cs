using Api.Data;
using Microsoft.EntityFrameworkCore;
using Shared.Models;

namespace Api.Services.Repositories;

public class NotificationRepository(ApiDbContext context) : DbRepository<Notification>(context)
{
    public IQueryable<Notification> GetBaseQueryable() =>
        GetQueryable()
        .AsNoTracking();

    public async Task<Notification?> GetLastRecentAsync(string to) =>
        await GetBaseQueryable()
        .Where(n => n.To == to && n.CreatedAt >= DateTime.Now.AddMinutes(-5))
        .OrderByDescending(n => n.CreatedAt)
        .FirstOrDefaultAsync();
}
