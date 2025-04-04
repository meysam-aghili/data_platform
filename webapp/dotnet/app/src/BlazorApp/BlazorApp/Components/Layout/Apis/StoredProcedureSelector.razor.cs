using BlazorApp.Models;
using BlazorApp.Services;
using BlazorApp.Shared;
using BlazorApp.Services.Sql;
using Bit.BlazorUI;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace BlazorApp.Components.Layout.Apis;

public partial class StoredProcedureSelector
{
    private readonly ISnackbar _snack;
    private readonly ISqlService _sql;
    private readonly ISqlServerProviderService _bisql;
    private readonly ApiSqlServer[] _biapiServers = [
        ApiSqlServer.Warehouse
    ];
    private List<BitDropdownItem<string>> _serverOptions =>
        _biapiServers
        .Select(s => s.ToString())
        .ToDropdownItems();

    private readonly BitDropdownClassStyles _styles = new()
    {
        ScrollContainer = "max-height: 250px;",
    };

    public StoredProcedureSelector(
        ISnackbar snack,
        ISqlServerProviderService bisql,
        [FromKeyedServices("mssql")] ISqlService sql)
    {
        _snack = snack;
        _bisql = bisql;
        _sql = sql;
        _sql.Options = new()
        {
            Credentials = _bisql.GetCredentials(SqlUser.SqlApiUser)
        };
    }

    [Parameter]
    public ApiBsonDocument Api { get; set; }

    [Parameter]
    public EventCallback<ApiBsonDocument> ApiChanged { get; set; }

    private ICollection<BitDropdownItem<string>> _sps = [];
    private ICollection<BitDropdownItem<string>> _dbs = [];
    private bool _dbsLoading = true;
    private bool _spsLoading = true;

    protected override async Task OnParametersSetAsync()
    {
        var options = GetSqlOptions(Api.StoredProcedure.Database.Server, Api.StoredProcedure.Database.Database);
        var dbs = await _sql.GetDatabasesAsync(options);
        _dbs = dbs.ToDropdownItems();
        var sps = await _sql.GetStoredProceduresAsync("api", options);
        _sps = sps.ToDropdownItems();
        _dbsLoading = false;
        _spsLoading = false;
        //StateHasChanged();
    }

    private async Task ResetDatabasesAsync()
    {
        try
        {
            _dbsLoading = true;
            _dbs = [];
            _sps = [];
            Api.StoredProcedure.Database.Database = string.Empty;
            Api.StoredProcedure.StoredProcedure = string.Empty;
            StateHasChanged();
            var options = GetSqlOptions(Api.StoredProcedure.Database.Server);
            var dbs = await _sql.GetDatabasesAsync(options, includeSystemDbs: false);
            _dbs = dbs.ToDropdownItems();
        }
        catch
        {
            _snack.Add($"Couldn't fetch databases in {Api.StoredProcedure.Database.Server}...", Severity.Error);
        }
        _dbsLoading = false;
        //StateHasChanged();
    }

    private async Task ResetStoredProceduresAsync()
    {
        try
        {
            _spsLoading = true;
            _sps = [];
            //StateHasChanged();
            Api.StoredProcedure.StoredProcedure = string.Empty;
            var sps = await _sql.GetStoredProceduresAsync(
                "biapi",
                GetSqlOptions(
                    Api.StoredProcedure.Database.Server,
                    Api.StoredProcedure.Database.Database
                    )
                );
            _sps = sps.ToDropdownItems();
        }
        catch
        {
            _snack.Add($"Couldn't fetch stored-procedures in {Api.StoredProcedure.Database.Database}...", Severity.Error);
        }
        _spsLoading = false;
        //StateHasChanged();
    }

    private SqlOptionModel GetSqlOptions(string server, string db = "") => new()
    {
        Database = new()
        {
            Server = server,
            Database = db
        },
        Credentials = _bisql.GetCredentials(SqlUser.SqlApiUser)
    };
}
