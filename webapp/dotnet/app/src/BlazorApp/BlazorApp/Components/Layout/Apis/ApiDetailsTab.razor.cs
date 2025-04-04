using BlazorApp.Models;
using BlazorApp.Services;
using BlazorApp.Services.Repositories;
using BlazorApp.Services.Auth;
using BlazorApp.Services.Sql;
using Bit.BlazorUI;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace BlazorApp.Components.Layout.Apis;

public partial class ApiDetailsTab
{
    private readonly ISnackbar _snack;
    private readonly ISqlService _sql;
    private readonly ILdapRepository _ldap;
    private readonly ISqlServerProviderService _bisql;
    private readonly IApiRepository _apis;

    private readonly string[] _ldapSearchBases = [
            LdapBaseModel.ITUsers
           
        ];

    private BitButtonClassStyles _btnStyles = new()
    {
        Primary = "margin-top: .5em; margin-bottom: .5em;",
        LoadingContainer = "margin-top: .5em; margin-bottom: .5em;",
        LoadingLabel = "margin-top: .5em; margin-bottom: .5em;"
    };

    public ApiDetailsTab(
        ISnackbar snack,
        ILdapRepository ldap,
        ISqlServerProviderService bisql,
        [FromKeyedServices("mssql")] ISqlService sql,
        IApiRepository apis)
    {
        _snack = snack;
        _sql = sql;
        _ldap = ldap;
        _bisql = bisql;
        _apis = apis;

        _ldap.SearchBases = _ldapSearchBases;
    }

    [Parameter]
    public ApiBsonDocument Api { get; set; }

    public IEnumerable<LdapUserModel> AuthorizedUsers { get; set; } = [];

    protected override void OnInitialized()
    {
        _sql.Options = new()
        {
            Credentials = _bisql.GetCredentials(SqlUser.SqlApiUser)
        };
    }

    protected override async Task OnParametersSetAsync()
    {
        AuthorizedUsers = [.. await _ldap.GetUsersAsync(Api.Users)];
    }

    private async Task UpdateApi()
    {
        Api.Users = [.. from user in AuthorizedUsers select user.Id.ToLower()];
        await _apis.UpdateAsync(Api);
        _snack.Add($"API {Api.Slug} updated.", Severity.Success);
    }
}
