using System.Net.Mime;
using BlazorApp.Models;
using BlazorApp.Services.Auth;
using Microsoft.Extensions.Caching.Memory;


namespace BlazorApp.Services.Repositories;

public class LdapRepository : ILdapRepository
{
    private readonly ILdapService _ldap;
    private readonly IMemoryCache _cache;
    private string[] _searchBases = LdapBaseModel.AppsBases;

    public string[] SearchBases
    {
        get
        {
            return _searchBases;
        }
        set
        {
            _searchBases = value;
            _ldap.SearchBases = value;
        }
    }

    public LdapRepository(ILdapService ldap, IMemoryCache cache)
    {
        _cache = cache;
        _ldap = ldap;
        _ldap.SearchBases = SearchBases;
    }

    public async Task<LdapUserModel?> GetUserAsync(string sAMAccountName)
    {
        if (string.IsNullOrEmpty(sAMAccountName))
        {
            return null;
        }

        if (_cache.TryGetValue(sAMAccountName, out LdapUserModel? user))
        {
            return user;
        }

        user = await _ldap.GetUserAsync(sAMAccountName);
        _cache.Set(sAMAccountName, user, new MemoryCacheEntryOptions
        {
            SlidingExpiration = TimeSpan.FromHours(6),
            AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1)
        });
        return user;
    }

    public async Task<IReadOnlyList<LdapUserModel>> GetUsersAsync(IEnumerable<string> sAMAccountNames)
    {
        List<LdapUserModel> users = [];
        List<string> uncachedUsernames = [];

        foreach (var username in sAMAccountNames)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                continue;
            }
            if (_cache.TryGetValue(username, out LdapUserModel? user))
            {
                users.Add(user!);
                continue;
            }
            uncachedUsernames.Add(username);
        }

        List<LdapUserModel> uncachedUsers = [.. await _ldap.GetUsersAsync(sAMAccountNames)];

        foreach (var user in uncachedUsers)
        {
            _cache.Set(user.Id.ToLower(), user, new MemoryCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromHours(6),
                AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1)
            });
        }

        List<LdapUserModel> finalUsers = [.. users, .. uncachedUsers];

        return finalUsers.DistinctBy(u => u.Id).ToList().AsReadOnly();
    }

    public async Task<(byte[] img, string extension)> GetUserThumbnailAsync(string sAMAccountName)
    {
        var user = await GetUserAsync(sAMAccountName);
        if (user is null || user.ThumbnailPhoto is null)
        {
            return ([], string.Empty);
        }
        return (user.ThumbnailPhoto, GetContentType(user.ThumbnailPhoto));
    }

    private static string GetContentType(byte[] img)
    {
        if (img.Length > 4
            && img[0] == 0xFF
            && img[1] == 0xD8
            && img[2] == 0xFF)
        {
            return MediaTypeNames.Image.Jpeg;
        }
        if (img.Length > 8
            && img[0] == 0x89
            && img[1] == 0x50
            && img[2] == 0x4E
            && img[3] == 0x47
            && img[4] == 0x0D
            && img[5] == 0x0A
            && img[6] == 0x1A
            && img[7] == 0x0A)
        {
            return MediaTypeNames.Image.Png;
        }
        return MediaTypeNames.Application.Octet;
    }
}
