using System.Text;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using BlazorApp.Services;
using BlazorApp.Services.Auth;
using BlazorApp.Services.Repositories;
using BlazorApp.Services.Sql;
using BlazorApp.Services.Mongo;
using Mcrio.Configuration.Provider.Docker.Secrets;
using BlazorApp.Models;
using System.Reflection;
using System.Configuration;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.Extensions.Options;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Authentication;
using Bit.BlazorUI;
using MudBlazor.Services;
using MudBlazor;
using BlazorApp.Services.Elastic;


namespace BlazorApp.Shared;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class SwaggerExcludeAttribute : Attribute;

public static class StartupExtension
{
    public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddHttpContextAccessor();
        services.AddMemoryCache();
        services.AddBlazoredLocalStorage();

        services.AddKonductorHttpClient(config);
        services.AddAuthenticationServices(config);

        services.AddAntiforgery(o => o.HeaderName = "XSRF-TOKEN");
        services.AddControllersWithViews();
        services.AddRazorPages(options =>
        {
            options.Conventions.AllowAnonymousToPage("/biuser/login");
        });
        services.AddServerSideBlazor().AddCircuitOptions(option =>
        {
            option.DetailedErrors = true;
        });
        services.AddBitBlazorUIServices();
        services.AddBootstrapBlazor();
        services.AddMudServices(config =>
        {
            config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomRight;
            config.SnackbarConfiguration.ShowCloseIcon = true;
            config.SnackbarConfiguration.VisibleStateDuration = 10000;
            config.SnackbarConfiguration.HideTransitionDuration = 500;
            config.SnackbarConfiguration.ShowTransitionDuration = 500;
            config.SnackbarConfiguration.SnackbarVariant = Variant.Filled;
        });

        services.AddTransient<IAuthService, AuthService>();
        services.AddTransient<IRoleRepository, RoleRepository>();
        services.AddTransient<IJobRepository, JobRepository>();
        services.AddTransient<ISwarmRepository, SwarmRepository>();
        //services.AddTransient<IElasticService, ElasticService>();
        services.AddTransient<IOtpRepository, OtpRepository>();
        services.AddKeyedTransient<ISqlService, SqlServerService>("mssql");
        services.AddTransient<IApiLogRepository, ApiLogRepository>();
        services.AddTransient<IElasticService, ElasticService>();

        services.AddSingleton<IApiRepository, ApiRepository>();
        services.AddSingleton<ILdapRepository, LdapRepository>();
        
        services.AddSingleton(typeof(IMongoService<>), typeof(MongoService<>));
        services.AddSingleton<ISqlServerProviderService, SqlServerProviderService>();

        services.AddScoped<ProfileService>();
        services.AddScoped<ILdapService, LdapService>();
        services.AddTransient<ILdapService, LdapService>();

        return services;
    }

    public static IServiceCollection AddAuthenticationServices(this IServiceCollection services, IConfiguration config)
    {
        services
            .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(opt =>
            {
                opt.ExpireTimeSpan = TimeSpan.FromDays(2);
                opt.Cookie.Name = "token";
                opt.Cookie.SameSite = SameSiteMode.Strict;
                opt.LoginPath = "/login";
                opt.LogoutPath = "/logout";
                opt.AccessDeniedPath = "/forbidden";
                opt.Events = new CookieAuthenticationEvents
                {
                    OnValidatePrincipal = ctx =>
                    {
                        if (ctx.Principal?.Identity?.IsAuthenticated ?? false)
                        {
                            var claims = ctx.Principal.Claims;
                            if (claims is null)
                            {
                                ctx.RejectPrincipal();
                                return ctx.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                            }
                        }
                        return Task.CompletedTask;
                    }
                };
            });
        services.AddAuthorizationBuilder()
            .AddPolicy("advanced-analytics", policy =>
                policy
                .RequireAuthenticatedUser()
                .RequireRole(RoleNames.AdvancedAnalytics)
            );
        services.AddCascadingAuthenticationState();
        services.AddAntiforgery(o => o.HeaderName = "XSRF-TOKEN");
        services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                policy.WithOrigins("https://biapps.digikala.com", "https://www.biapps.digikala.com", "https://digikala.com", "https://www.digikala.com");
            });
        });

        return services;
    }

    public static IConfigurationBuilder AddConfigs(this IConfigurationBuilder builder, string envPrefix = "APPS_")
    {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        return builder
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{environment}.json", optional: true)
            .AddEnvironmentVariables()
            .AddIniFile("dev.ini", optional: true)
            .AddEnvironmentVariables(prefix: envPrefix)
            .AddDockerSecrets();
    }

    public static IServiceCollection AddKonductorHttpClient(this IServiceCollection services, IConfiguration config)
    {
        services.AddHttpClient("konductor-api", client =>
        {
            client.BaseAddress = new Uri(config["KONDUCTOR_ADDRESS_API"] ?? "http://172.20.65.42:10010");
        });
        return services;
    }
}