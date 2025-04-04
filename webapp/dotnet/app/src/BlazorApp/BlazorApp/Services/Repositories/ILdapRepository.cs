using BlazorApp.Models;

namespace BlazorApp.Services.Repositories;

public interface ILdapRepository
{
    public string[] SearchBases { get; set; }

    Task<LdapUserModel?> GetUserAsync(string sAMAccountName);

    Task<IReadOnlyList<LdapUserModel>> GetUsersAsync(IEnumerable<string> sAMAccountNames);

    Task<(byte[] img, string extension)> GetUserThumbnailAsync(string sAMAccountName);
}
