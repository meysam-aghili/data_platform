using BlazorApp.Components;
using BlazorApp.Shared;


var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddConfigs();
builder.Services.AddServices(builder.Configuration);

//builder.Services.Configure<RouteOptions>(options =>
//{
//    options.LowercaseUrls = true;
//});

//var webSocketOptions = new WebSocketOptions
//{
//    KeepAliveInterval = TimeSpan.FromMinutes(2)
//};


builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();


var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    //app.UseHsts();
}

//app.UseHttpsRedirection();

app.UseStaticFiles();
//app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
//app.MapControllerRoute(
//name: "default",
//pattern: "{controller=Home}/{action=Index}/{id?}");
//app.MapReverseProxy();
//app.MapRazorPages();
//app.MapBlazorHub();
//app.MapFallbackToPage("/_Host");


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

await app.RunAsync();
