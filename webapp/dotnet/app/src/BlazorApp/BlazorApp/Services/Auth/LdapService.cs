using Novell.Directory.Ldap;
using SharpCompress.Common;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using BlazorApp.Models;
using BlazorApp.Shared;
using BlazorApp.Shared;


namespace BlazorApp.Services.Auth;

public class LdapService(IConfiguration config) : ILdapService
{
    private readonly string _ldapServer = config["LDAP_SERVER"] ?? throw new ConfigNotFoundException("LDAP_SERVER");
    private readonly int _ldapPort = int.Parse(config["LDAP_PORT"] ?? "636");
    private readonly string _searchDn = config["LDAP_SEARCH_DN"] ?? throw new ConfigNotFoundException("LDAP_SEARCH_DN");
    private readonly string _searchDnPass = config["LDAP_SEARCH_DN_PASS"] ?? throw new ConfigNotFoundException("LDAP_SEARCH_DN_PASS");
    private readonly string[] _attributes =
    [
        "distinguishedName", "cn", "company", "department",
        "displayName", "faDisplayName", "gender", "mail",
        "name", "title", "sAMAccountName", "thumbnailPhoto"
    ];
    private readonly string _defaultSearchBase = LdapBaseModel.Root;
    public string[] SearchBases { get; set; } = [];
    private string[] Bases => SearchBases.Length > 0 ? SearchBases : [_defaultSearchBase];
    private readonly LdapConnection _ldap = new()
    {
        SecureSocketLayer = true
    };

    public async Task<bool> ConnectAsync(string username, string password)
    {
        _ldap.UserDefinedServerCertValidationDelegate += (sender, certificate, chain, errors) => true; // beacuase of self signed ca
        await _ldap.ConnectAsync(_ldapServer, _ldapPort);
        await _ldap.BindAsync(username, password);
        return _ldap.Bound;
    }

    public void Dispose() => _ldap.Disconnect();

    private async Task<ReadOnlyCollection<LdapUserModel>> ExecuteQuery(
        string ldapQuery,
        string searchBase,
        LdapSearchConstraints? constraints = null)
    {
        try
        {
            await ConnectAsync(_searchDn, _searchDnPass);
            var searchResults = await _ldap.SearchAsync(
                searchBase,
                LdapConnection.ScopeSub,
                ldapQuery,
                _attributes,
                false,
                constraints ?? new LdapSearchConstraints
                {
                    ReferralFollowing = true,
                    MaxResults = 10
                });
            var users = new List<LdapUserModel>();
            while (await searchResults.HasMoreAsync())
            {
                var entry = await searchResults.NextAsync();
                if (entry != null)
                {
                    LdapUserModel LdapUser = entry.ToLdapUserModel();
                    LdapUser.DistinguishedName = entry.Dn;
                    users.Add(LdapUser);
                }
            }
            return users.AsReadOnly();
        }
        catch (LdapException ex) when (ex.ResultCode == 4)
        {
            // This is the case when the search term is too broad.
            return ReadOnlyCollection<LdapUserModel>.Empty.AsReadOnly();
        }
        catch
        {
            throw;
        }
        finally
        {
            Dispose();
        }
    }

    public async Task<bool> AuthenticateAsync(AuthUserModel user)
    {
        LdapUserModel? LdapUser = default;
        try
        {
            LdapUser = await GetUserAsync(user.Username);
            if (LdapUser is null)
            {
                return false;
            }
        }
        catch
        {
            return false;
        }
        try
        {
            await ConnectAsync(LdapUser.DistinguishedName!, user.Password);
            Dispose();
            return true;
        }
        catch
        {
            Dispose();
            return false;
        }
        finally
        {
            Dispose();
        }
    }

    public async Task<LdapUserModel?> GetUserAsync(string sAMAccountName)
    {
        var query = $"(uid={sAMAccountName})";
        var tasks = from sb in Bases select ExecuteQuery(query, sb);
        await Task.WhenAll(tasks);
        List<LdapUserModel> hits = [];
        foreach (var task in tasks)
        {
            hits.AddRange(await task);
        }
        return hits.Single();
    }

    public async Task<ReadOnlyCollection<LdapUserModel>> GetUsersAsync(IEnumerable<string> sAMAccountNames)
    {
        if (!sAMAccountNames.Any())
        {
            return ReadOnlyCollection<LdapUserModel>.Empty;
        }
        var query = "(&(objectClass=user)"
            + "(|"
            + string.Join(string.Empty, from sAMAccountName in sAMAccountNames select $"(sAMAccountName={sAMAccountName})")
            + "))";
        var tasks = from sb in Bases select ExecuteQuery(query, sb);
        await Task.WhenAll(tasks);
        List<LdapUserModel> hits = [];
        foreach (var task in tasks)
        {
            hits.AddRange(await task);
        }
        return hits.AsReadOnly();
    }

    public async Task<ReadOnlyCollection<LdapUserModel>> QueryAsync(string term, int maxResults = 10)
    {
        var query = $"(&(objectClass=user)(|(sAMAccountName=*{term}*)(cn=*{term}*)))";
        var tasks = from sb in Bases select ExecuteQuery(query, sb);
        await Task.WhenAll(tasks);
        List<LdapUserModel> hits = [];
        foreach (var task in tasks)
        {
            hits.AddRange(await task);
        }
        return hits.AsReadOnly();
    }
}
