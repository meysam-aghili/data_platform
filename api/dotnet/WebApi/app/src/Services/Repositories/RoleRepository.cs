using System.Data;
using WebApi.Shared;
using WebApi.Models;


namespace WebApi.Services.Repositories;

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

    //public async Task<IReadOnlyList<RoleModel>> GetAsync(IEnumerable<string> roleNames)
    //{
    //    var items = await Task.FromResult(
    //        from r in roles.Query()
    //        where roleNames.Contains(r.Slug)
    //        select r);
    //    return items.ToList().AsReadOnly();
    //}

    //public async Task<Role> CreateRoleAsync(string slug, IEnumerable<BILdapUser> users)
    //{
    //    var role = new Role
    //    {
    //        Slug = slug.ToSlug(),
    //        Users = (from user in users select user.Id).Distinct().ToList()
    //    };
    //    await roles.CreateAsync(role);
    //    return role;
    //}

    //public async Task UpdateRoleAsync(Role role)
    //    => await roles.UpdateAsync(role);

    //public async Task DeleteRoleAsync(Role role)
    //    => await roles.DeleteAsync(role.Id.ToString());

    //public async Task<List<Role>> GetRolesAsync()
    //    => await Task.FromResult(roles.Query().ToList());

    //public async Task<List<Role>> GetRolesAsync(string query)
    //{
    //    IQueryable<Role>? items =
    //        from role in roles.Query()
    //        where (role.Slug ?? string.Empty).Contains(query)
    //        select role;

    //    return await Task.FromResult(items.ToList());
    //}

    //public async Task<(IReadOnlyList<Role>, int)> GetRolesAsync(int page = 0, int pageSize = 10)
    //{
    //    IQueryable<Role>? itemsQuery =
    //        from item in roles.Query()
    //        orderby item.UpdatedAt descending, item.CreatedAt descending
    //        select item;

    //    Task<List<Role>> itemsTask = Task.FromResult(
    //        itemsQuery
    //        .Skip(pageSize * page)
    //        .Take(pageSize)
    //        .ToList()
    //        );

    //    Task<int> countTask = Task.FromResult(itemsQuery.Count());

    //    await Task.WhenAll(itemsTask, countTask);
    //    List<Role> items = await itemsTask;
    //    int count = await countTask;

    //    return (items, count);
    //}

    //public async Task<(IReadOnlyList<Role>, int)> GetRolesAsync(string query, int page = 0, int pageSize = 10)
    //{
    //    IQueryable<Role>? itemsQuery =
    //        from item in roles.Query()
    //        orderby item.UpdatedAt descending, item.CreatedAt descending
    //        select item;

    //    if (!string.IsNullOrWhiteSpace(query))
    //    {
    //        itemsQuery =
    //            from item in itemsQuery
    //            where (item.Slug ?? string.Empty).Contains(query)
    //            select item;
    //    }

    //    Task<List<Role>> itemsTask = Task.FromResult(
    //        itemsQuery
    //        .Skip(pageSize * page)
    //        .Take(pageSize)
    //        .ToList()
    //        );
    //    Task<int> countTask = Task.FromResult(itemsQuery.Count());

    //    await Task.WhenAll(itemsTask, countTask);

    //    List<Role> items = await itemsTask;
    //    int count = await countTask;

    //    return (items, count);
    //}

    //public async Task<IReadOnlyList<Role>> GetUserRolesAsync(string username) =>
    //    await Task.FromResult((
    //        from role in roles.Query().ToList()
    //        where role.Users.Contains(username, StringComparer.OrdinalIgnoreCase)
    //        select role
    //    )
    //    .ToList()
    //    .AsReadOnly());

}
