using System.Data;
using WebApi.Models;


namespace WebApi.Services.Repositories;

public interface IRoleRepository
{
    //Task<List<Role>> GetRolesAsync();
    //Task<List<Role>> GetRolesAsync(string query);
    //Task<(IReadOnlyList<Role>, int)> GetRolesAsync(int page = 0, int pageSize = 10);
    //Task<(IReadOnlyList<Role>, int)> GetRolesAsync(string query, int page = 0, int pageSize = 10);
    //Task<Role> CreateRoleAsync(string slug, IEnumerable<BILdapUser> users);
    //Task UpdateRoleAsync(Role role);
    //Task DeleteRoleAsync(Role role);
    //Task<IReadOnlyList<Role>> GetUserRolesAsync(string username);
    Task<RoleModel> GetAsync(string slug);
    //Task<IReadOnlyList<RoleModel>> GetAsync(IEnumerable<string> roleSlugs);
}