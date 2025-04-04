using System.Text.Json.Serialization;


namespace BlazorApp.Models;

public record AuthUserModel
(
    [property:JsonPropertyName("username")]
    string Username,

    [property:JsonPropertyName("password")]
    string Password
);

public record class JwtToken
{
    [JsonPropertyName("token")]
    public string Token { get; init; } = null!;

    [JsonPropertyName("type")]
    public string Type => "Bearer token";

    [JsonPropertyName("expires_at")]
    public DateTimeOffset ExpiresAt { get; init; }
}

public class AttrLdapAttribute(string name) : Attribute
{
    public string Name { get; set; } = name;
}

public record LdapUserModel
{
    [AttrLdap("distinguishedName")]
    [JsonPropertyName("distinguished_name")]
    public string? DistinguishedName { get; set; }

    [AttrLdap("cn")]
    [JsonPropertyName("common_name")]
    public string? CommonName { get; init; }

    [AttrLdap("company")]
    [JsonPropertyName("company")]
    public string? Company { get; init; }

    [AttrLdap("department")]
    [JsonPropertyName("department")]
    public string? Department { get; init; }

    [AttrLdap("displayName")]
    [JsonPropertyName("display_name")]
    public string? DisplayName { get; init; }

    [AttrLdap("faDisplayName")]
    [JsonPropertyName("persian_display_name")]
    public string? PersianDisplayName { get; init; }

    [AttrLdap("gender")]
    public string? Gender { get; init; }

    [AttrLdap("mail")]
    [JsonPropertyName("email")]
    public string? Email { get; init; }

    [AttrLdap("name")]
    [JsonPropertyName("name")]
    public string? Name { get; init; }

    [AttrLdap("title")]
    [JsonPropertyName("title")]
    public string? Title { get; init; }

    [AttrLdap("sAMAccountName")]
    [JsonPropertyName("sam_id")]
    public string Id { get; init; }

    [AttrLdap("thumbnailPhoto")]
    [JsonPropertyName("thumbnail_photo")]
    public byte[]? ThumbnailPhoto { get; init; }
}

public static class LdapBaseModel
{
    public static readonly string Root = "DC=dataplatform,DC=COM";
    public static readonly string ITUsers = "OU=IT,DC=dataplatform,DC=COM";
    public static readonly string[] AppsBases = [ITUsers];
}

public class UserLoginModel
{
    public string? Username { get; set; }
    public string? Password { get; set; }
}