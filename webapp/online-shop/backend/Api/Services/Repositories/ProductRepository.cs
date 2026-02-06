using Api.Data;
using Microsoft.EntityFrameworkCore;
using Minio.DataModel;
using Shared.Models;
using System.ComponentModel;
using System.Linq.Expressions;

namespace Api.Services.Repositories;

public class ProductRepository(ApiDbContext context) : DbRepository<Product>(context)
{
    public IQueryable<Product> GetBaseQueryable() =>
        GetQueryable()
        .Where(p => p.ProductVariants!.Any())
        .Include(p => p.ProductVariants!.OrderBy(pv => pv.SortNumber))
        .ThenInclude(v => v.Color)
        .Include(p => p.ProductVariants!)
        .ThenInclude(v => v.Size)
        .Include(p => p.ProductVariants!)
        .ThenInclude(v => v.Images!.OrderBy(img => img.SortNumber))
        .Include(p => p.Category)
        .Include(p => p.Brand)
        .AsNoTracking()
        .AsSplitQuery();
        
    public override async Task<List<Product>> GetAsync() =>
        await GetBaseQueryable().ToListAsync();

    public override async Task<Product?> GetAsync(long id) =>
        await GetBaseQueryable().SingleOrDefaultAsync(r => r.Id == id);

    public async Task<Product?> GetAsync(string slug) =>
        await GetBaseQueryable().SingleOrDefaultAsync(r => r.Slug == slug);

    public async Task<PagedResult<Product>> GetAsync(
        int pageNumber = 1,
        int pageSize = 10,
        string sortBy = "slug",
        ListSortDirection sortDirection = ListSortDirection.Ascending,
        string? searchSlug = null,
        string? brand = null,
        string? color = null,
        bool? inStock = null,
        int? minPrice = null,
        int? maxPrice = null,
        List<long>? categoryIds = null)
    {
        var query = GetBaseQueryable();

        if (!string.IsNullOrWhiteSpace(searchSlug))
            query = query.Where(s => s.Slug.Contains(searchSlug));

        if (!string.IsNullOrWhiteSpace(brand))
        {
            var brands = brand
                .Split(",", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Where(b => b.Length > 3)
                .ToList();
            query = query.Where(s => brands.Contains(s.Brand.Title));
        }

        if (!string.IsNullOrWhiteSpace(color))
        {
            var colors = color
                .Split(",", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Where(c => c.Length > 1)
                .ToList();

            query = query.Where(p =>
                p.ProductVariants!.Any(v =>
                    colors.Contains(v.Color.Title)
                )
            );
        }

        if (inStock is not null && inStock.Value)
        {   
            query = query.Where(p => p.ProductVariants!.Any(v => v.Stock > 0));
        }

        if (minPrice is not null)
            query = query.Where(s => s.ProductVariants!.Min(b => b.Price) >= minPrice);

        if (maxPrice is not null)
            query = query.Where(s => s.ProductVariants!.Max(b => b.Price) <= maxPrice);

        if (categoryIds is not null && categoryIds.Count > 0)
            query = query.Where(s => categoryIds.Contains(s.CategoryId));

        var totalCount = await query.CountAsync();

        var inStockOrdered = query.OrderByDescending(p => p.ProductVariants!.Sum(v => v.Stock) > 0);
        var sortSelectors = new Dictionary<string, Expression<Func<Product, object>>>(StringComparer.OrdinalIgnoreCase)
        {
            ["slug"] = x => x.Slug,
            ["created_at"] = x => x.CreatedAt,
            ["price"] = x => x.ProductVariants!.Max(pv => pv.Price)
        };
        sortSelectors.TryGetValue(sortBy, out var sortSelector);
        sortSelector ??= x => x.Slug;
        query = sortDirection == ListSortDirection.Ascending
            ? inStockOrdered.ThenBy(sortSelector).ThenBy(p => p.Id)
            : inStockOrdered.ThenByDescending(sortSelector).ThenBy(p => p.Id);

        var data = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<Product>
        {
            Data = data,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }
}
