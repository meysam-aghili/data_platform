using Api.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Shared.Models;

namespace Api.Services.Repositories;

public abstract class DbRepository<T>(ApiDbContext context) where T : Base
{
    private readonly ApiDbContext _context = context;

    public IQueryable<T> GetQueryable() => 
        _context.Set<T>().AsQueryable<T>();
    
    public virtual async Task<List<T>> GetAsync() => 
        await GetQueryable().AsNoTracking().ToListAsync();

    public virtual async Task<T?> GetAsync(long id) =>
        await GetQueryable().AsNoTracking().SingleOrDefaultAsync(r => r.Id == id);

    public async Task<IDbContextTransaction> BeginTransaction() =>
        await _context.Database.BeginTransactionAsync();

    public async Task<T> AddAsync(T model, bool saveChanges = true)
    {
        await _context.Set<T>().AddAsync(model);
        if (saveChanges)
            await _context.SaveChangesAsync();
        return model;
    }

    public async Task<List<T>> AddRangeAsync(List<T> models, bool saveChanges = true)
    {
        await _context.Set<T>().AddRangeAsync(models);
        if (saveChanges)
            await _context.SaveChangesAsync();
        return models;
    }

    public async Task<T> UpdateAsync(T model, bool saveChanges = true)
    {
        _context.Set<T>().Update(model);
        if (saveChanges)
            await _context.SaveChangesAsync();
        return model;
    }

    public async Task<List<T>> UpdateRangeAsync(List<T> models, bool saveChanges = true)
    {
        _context.Set<T>().UpdateRange(models);
        if (saveChanges)
            await _context.SaveChangesAsync();
        return models;
    }

    public async Task<T> RemoveAsync(T model, bool saveChanges = true)
    {
        _context.Set<T>().Remove(model);
        if (saveChanges)
            await _context.SaveChangesAsync();
        return model;
    }

    public async Task<List<T>> RemoveRangeAsync(List<T> models, bool saveChanges = true)
    {
        _context.Set<T>().RemoveRange(models);
        if (saveChanges)
            await _context.SaveChangesAsync();
        return models;
    }

    public async Task<int> ExecAsync(string query, bool saveChanges = true)
    {
        var result = await _context.Database.ExecuteSqlRawAsync(query);
        if (saveChanges)
            await _context.SaveChangesAsync();
        return result;
    }
}
