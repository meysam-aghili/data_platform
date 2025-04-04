using System.Data;
using BlazorApp.Models;


namespace BlazorApp.Services.Repositories;

public interface IRoleRepository
{
    //Task<List<Role>> GetRolesAsync();
    //Task<List<Role>> GetRolesAsync(string query);
    Task<(IReadOnlyList<RoleModel>, int)> GetRolesAsync(int page = 0, int pageSize = 10);
    Task<(IReadOnlyList<RoleModel>, int)> GetRolesAsync(string query, int page = 0, int pageSize = 10);
    Task<RoleModel> CreateRoleAsync(string slug, IEnumerable<LdapUserModel> users);
    Task UpdateRoleAsync(RoleModel role);
    Task DeleteRoleAsync(RoleModel role);
    Task<IReadOnlyList<RoleModel>> GetUserRolesAsync(string username);
    Task<RoleModel> GetAsync(string slug);
    Task<IReadOnlyList<RoleModel>> GetAsync(IEnumerable<string> roleSlugs);
}