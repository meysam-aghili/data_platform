using System.Collections.ObjectModel;
using BlazorApp.Models;


namespace BlazorApp.Services.Auth;

public interface ILdapService
{
    public string[] SearchBases { get; set; }
    Task<bool> AuthenticateAsync(AuthUserModel user);
    Task<LdapUserModel?> GetUserAsync(string sAMAccountName);
    Task<bool> ConnectAsync(string username, string password);
    void Dispose();
    Task<ReadOnlyCollection<LdapUserModel>> GetUsersAsync(IEnumerable<string> sAMAccountNames);
    Task<ReadOnlyCollection<LdapUserModel>> QueryAsync(string query, int maxResults = 10);
}
