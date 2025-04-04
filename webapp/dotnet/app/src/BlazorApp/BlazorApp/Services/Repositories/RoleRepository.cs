using System.Data;
using BlazorApp.Models;
using BlazorApp.Shared;
using BlazorApp.Services.Mongo;


namespace BlazorApp.Services.Repositories;

public class RoleRepository(IMongoService<RoleModel> roles) : IRoleRepository
{

    public async Task<RoleModel> GetAsync(string slug) => (
        await Task.FromResult(
            from r in roles.Query()
            where r.Slug == slug
            select r
        )
    ).First()
    ?? throw new ArgumentException($"No matching role `{slug}` found in the database.");

    public async Task<IReadOnlyList<RoleModel>> GetAsync(IEnumerable<string> roleNames)
    {
        var items = await Task.FromResult(
            from r in roles.Query()
            where roleNames.Contains(r.Slug)
            select r);
        return items.ToList().AsReadOnly();
    }

    public async Task<RoleModel> CreateRoleAsync(string slug, IEnumerable<LdapUserModel> users)
    {
        var role = new RoleModel
        {
            Slug = slug.ToSlug(),
            Users = (from user in users select user.Id).Distinct().ToList()
        };
        await roles.CreateAsync(role);
        return role;
    }

    public async Task UpdateRoleAsync(RoleModel role)
        => await roles.UpdateAsync(role);

    public async Task DeleteRoleAsync(RoleModel role)
        => await roles.DeleteAsync(role.Id.ToString());

    //public async Task<List<Role>> GetRolesAsync()
    //    => await Task.FromResult(roles.Query().ToList());

    public async Task<List<RoleModel>> GetRolesAsync(string query)
    {
        IQueryable<RoleModel>? items =
            from role in roles.Query()
            where (role.Slug ?? string.Empty).Contains(query)
            select role;

        return await Task.FromResult(items.ToList());
    }

    public async Task<(IReadOnlyList<RoleModel>, int)> GetRolesAsync(int page = 0, int pageSize = 10)
    {
        IQueryable<RoleModel>? itemsQuery =
            from item in roles.Query()
            orderby item.UpdatedAt descending, item.CreatedAt descending
            select item;

        Task<List<RoleModel>> itemsTask = Task.FromResult(
            itemsQuery
            .Skip(pageSize * page)
            .Take(pageSize)
            .ToList()
            );

        Task<int> countTask = Task.FromResult(itemsQuery.Count());

        await Task.WhenAll(itemsTask, countTask);
        List<RoleModel> items = await itemsTask;
        int count = await countTask;

        return (items, count);
    }

    public async Task<(IReadOnlyList<RoleModel>, int)> GetRolesAsync(string query, int page = 0, int pageSize = 10)
    {
        IQueryable<RoleModel>? itemsQuery =
            from item in roles.Query()
            orderby item.UpdatedAt descending, item.CreatedAt descending
            select item;

        if (!string.IsNullOrWhiteSpace(query))
        {
            itemsQuery =
                from item in itemsQuery
                where (item.Slug ?? string.Empty).Contains(query)
                select item;
        }

        Task<List<RoleModel>> itemsTask = Task.FromResult(
            itemsQuery
            .Skip(pageSize * page)
            .Take(pageSize)
            .ToList()
            );
        Task<int> countTask = Task.FromResult(itemsQuery.Count());

        await Task.WhenAll(itemsTask, countTask);

        List<RoleModel> items = await itemsTask;
        int count = await countTask;

        return (items, count);
    }

    public async Task<IReadOnlyList<RoleModel>> GetUserRolesAsync(string username) =>
        await Task.FromResult((
            from role in roles.Query().ToList()
            where role.Users.Contains(username, StringComparer.OrdinalIgnoreCase)
            select role
        )
        .ToList()
        .AsReadOnly());

}
