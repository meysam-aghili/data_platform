using System.Text;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using WebApi.Services;
using WebApi.Services.Auth;
using WebApi.Services.Repositories;
using WebApi.Services.Sql;
using Mcrio.Configuration.Provider.Docker.Secrets;
using WebApi.Models;
using WebApi.Controllers;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;
using AspNetCoreRateLimit;
using System.Configuration;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.Extensions.Options;


namespace WebApi.Shared;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class SwaggerExcludeAttribute : Attribute;

public static class StartupExtension
{
    public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddHttpContextAccessor();
        services.AddControllers();
        services.AddMemoryCache();
        services.AddExceptionHandler<ExceptionHandler>();

        services.AddInMemoryRateLimiting();
        services.Configure<ClientRateLimitOptions>(config.GetSection("ClientRateLimiting"));
        services.Configure<ClientRateLimitPolicies>(config.GetSection("ClientRateLimitPolicies"));
        services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
        services.AddHttpLogging(options =>
        {
            options.RequestHeaders.Add("Origin");
            options.RequestHeaders.Add("X-Client-Id");
            options.ResponseHeaders.Add("Retry-After");
            options.LoggingFields = HttpLoggingFields.All;
        });

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1.0.0",
                Title = "api",
                Description = "Data Platform APIs."
            });
            options.DocInclusionPredicate((docName, apiDesc) =>
            {
                if (apiDesc.CustomAttributes().Any(attr => attr is SwaggerExcludeAttribute))
                {
                    return false;
                }
                return true;
            });
            //var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            //options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
        });

        services.AddKonductorHttpClient(config);
        services.AddAuthenticationServices(config);

        services.AddTransient<IAuthService, AuthService>();
        services.AddTransient<ILdapService, LdapService>();
        services.AddTransient<IApiService, ApiService>();
        services.AddTransient<IApiRepository, ApiRepository>();
        services.AddTransient<IJobRepository, JobRepository>();
        services.AddTransient<ISwarmRepository, SwarmRepository>();
        //services.AddTransient<IElasticService, ElasticService>();
        services.AddKeyedTransient<ISqlService, SqlServerService>("mssql");

        //services.AddSingleton<ILdapRepository, LdapRepository>();
        services.AddSingleton<IRoleRepository, RoleRepository>();
        services.AddSingleton(typeof(IMongoService<>), typeof(MongoService<>));
        services.AddSingleton<ISqlServerProviderService, SqlServerProviderService>();

        return services;
    }

    public static IServiceCollection AddAuthenticationServices(this IServiceCollection services, IConfiguration config)
    {
        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = config["AUTH_JWT_ISSUER"],
                    ValidAudience = config["AUTH_JWT_AUDIENCE"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(config["AUTH_JWT_KEY"] ?? throw new ConfigNotFoundException("AUTH_JWT_KEY"))
                        )
                };

                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        if (context.Exception is SecurityTokenMalformedException)
                        {
                            //LogHelper.LogRequest(logger, context.HttpContext, 0, ErrorModel.Unauthorized());
                        }

                        if (context.Exception is SecurityTokenExpiredException)
                        {
                            //LogHelper.LogRequest(logger, context.HttpContext, 0, ErrorModel.Unauthorized());
                            context.Response.Headers.Add("Token-Expired", "true");
                        }

                        return Task.CompletedTask;
                    }
                };
            });
        services.AddAuthorization();
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

    public static async Task UseCustomClientRateLimiting(this WebApplication app)
    {
        using (IServiceScope scope = app.Services.CreateScope())
        {
            IClientPolicyStore clientPolicyStore = scope.ServiceProvider
              .GetRequiredService<IClientPolicyStore>();
            await clientPolicyStore.SeedAsync();
        }
        app.UseClientRateLimiting();
    }
}