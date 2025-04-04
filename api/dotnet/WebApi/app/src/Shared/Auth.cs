using WebApi.Models;
using Novell.Directory.Ldap;


namespace WebApi.Shared;

public static class LdapExtension
{
    public static LdapUserModel ToLdapUserModel(this LdapEntry entry) => new()
    {
        DistinguishedName = entry.GetAttributeOrNull(
            BiObjectExtensions.GetPropertyAttribute<LdapUserModel, AttrLdapAttribute>(
                nameof(LdapUserModel.DistinguishedName)
                )?.Name ?? string.Empty)?.StringValue ?? string.Empty,

        CommonName = entry.GetAttributeOrNull(
            BiObjectExtensions.GetPropertyAttribute<LdapUserModel, AttrLdapAttribute>(
                nameof(LdapUserModel.CommonName)
                )?.Name ?? string.Empty)?.StringValue ?? string.Empty,

        Company = entry.GetAttributeOrNull(
            BiObjectExtensions.GetPropertyAttribute<LdapUserModel, AttrLdapAttribute>(
                nameof(LdapUserModel.Company)
                )?.Name ?? string.Empty)?.StringValue ?? string.Empty,

        Department = entry.GetAttributeOrNull(
            BiObjectExtensions.GetPropertyAttribute<LdapUserModel, AttrLdapAttribute>(
                nameof(LdapUserModel.Department)
                )?.Name ?? string.Empty)?.StringValue ?? string.Empty,

        DisplayName = entry.GetAttributeOrNull(
            BiObjectExtensions.GetPropertyAttribute<LdapUserModel, AttrLdapAttribute>(
                nameof(LdapUserModel.DisplayName)
                )?.Name ?? string.Empty)?.StringValue ?? string.Empty,

        PersianDisplayName = entry.GetAttributeOrNull(
            BiObjectExtensions.GetPropertyAttribute<LdapUserModel, AttrLdapAttribute>(
                nameof(LdapUserModel.PersianDisplayName)
                )?.Name ?? string.Empty)?.StringValue ?? string.Empty,

        Gender = entry.GetAttributeOrNull(
            BiObjectExtensions.GetPropertyAttribute<LdapUserModel, AttrLdapAttribute>(
                nameof(LdapUserModel.Gender)
                )?.Name ?? string.Empty)?.StringValue ?? string.Empty,

        Email = entry.GetAttributeOrNull(
            BiObjectExtensions.GetPropertyAttribute<LdapUserModel, AttrLdapAttribute>(
                nameof(LdapUserModel.Email)
                )?.Name ?? string.Empty)?.StringValue ?? string.Empty,

        Name = entry.GetAttributeOrNull(
            BiObjectExtensions.GetPropertyAttribute<LdapUserModel, AttrLdapAttribute>(
                nameof(LdapUserModel.Name)
                )?.Name ?? string.Empty)?.StringValue ?? string.Empty,

        Title = entry.GetAttributeOrNull(
            BiObjectExtensions.GetPropertyAttribute<LdapUserModel, AttrLdapAttribute>(
                nameof(LdapUserModel.Title)
                )?.Name ?? string.Empty)?.StringValue ?? string.Empty,

        Id = entry.GetAttributeOrNull(
            BiObjectExtensions.GetPropertyAttribute<LdapUserModel, AttrLdapAttribute>(
                nameof(LdapUserModel.Id)
                )?.Name ?? string.Empty)?.StringValue ?? string.Empty,

        ThumbnailPhoto = entry.GetAttributeOrNull(
            BiObjectExtensions.GetPropertyAttribute<LdapUserModel, AttrLdapAttribute>(
                nameof(LdapUserModel.ThumbnailPhoto)
                )?.Name ?? string.Empty)?.ByteValue ?? [],
    };
}

