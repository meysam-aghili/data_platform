using BlazorApp.Services;
using BlazorApp.Models;
using BlazorApp.Services.Auth;
using BlazorApp.Services.Repositories;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Bit.BlazorUI;
using System.Diagnostics;

namespace BlazorApp.Components.Layout.Apis;

public partial class ApiListTab(IApiRepository repo, IRoleRepository roles)
{
    private readonly IApiRepository _repo = repo;
    private readonly IRoleRepository _roles = roles;

    private readonly int[] pageSizeOptions = [10, 25, 50];
    private readonly BitButtonClassStyles _btnStyles = new()
    {
        Icon = "font-size: small; margin-right: 0.4em;"
    };

    private MudTable<ApiBsonDocument> _tbl = default!;
    private string _query = default!;
    private IReadOnlyList<RoleModel> _adminRoles;
    
    private bool _isAdmin => (
        from role in _adminRoles
        where role.ContainsUser(User.Id.ToLower())
        select role)
        .Any();

    [CascadingParameter]
    public BitTheme? Theme { get; set; }

    [CascadingParameter]
    public LdapUserModel User { get; set; } = null!;


    [Parameter]
    public ApiBsonDocument? Selected { get; set; }

    [Parameter]
    public EventCallback<ApiBsonDocument> SelectedChanged { get; set; }

    [Parameter]
    public EventCallback<bool> OnCreateClick { get; set; }

    [Parameter]
    public EventCallback<bool> OnDeleteClick { get; set; }

    protected override async Task OnParametersSetAsync()
    {
        _adminRoles = await _roles.GetAsync([RoleNames.Admin]) ?? [];
    }

    protected async Task<TableData<ApiBsonDocument>> LoadPageAsync(TableState state, CancellationToken token)
    {
        var (items, count) = await _repo.GetAsync(
            createdBy: _isAdmin ? null : User.Id.ToLower(),
            query: _query,
            page: state.Page,
            pageSize: state.PageSize
        );
        return new TableData<ApiBsonDocument>
        {
            Items = items,
            TotalItems = count
        };
    }

    private void Search(string query)
    {
        _query = query;
        _tbl.ReloadServerData();
    }

    private string SelectedRowClass(ApiBsonDocument api, int rowNumber) =>
        Selected is not null && Selected.Id == api.Id
        ? "selected"
        : string.Empty;

    public async Task CreateCallback()
    {
        Trace.WriteLine("Create!");
        await OnCreateClick.InvokeAsync(true);
    }

    public async Task DeleteCallback()
    {
        Trace.WriteLine("Delete!");
        await OnDeleteClick.InvokeAsync(true);
    }
}
