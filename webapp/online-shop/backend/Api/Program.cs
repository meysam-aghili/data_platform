using System.Diagnostics;
using System.Text.Json.Serialization;
using Mcrio.Configuration.Provider.Docker.Secrets;
using Serilog;
using System.Text.Json;
using Api.Data;
using Microsoft.EntityFrameworkCore;
using Api.Shared;
using Swashbuckle.AspNetCore.SwaggerGen;
using Api.Middlewares;
using Api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Api.Services.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .SetBasePath(Path.Combine(AppContext.BaseDirectory))
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddIniFile("dev.ini", optional: true)
    .AddEnvironmentVariables()
    .AddDockerSecrets();

builder.Host.UseSerilog();
Log.Logger = SerilogConfig.GetLogger(builder.Configuration);
Serilog.Debugging.SelfLog.Enable(msg => Debug.WriteLine(msg));

builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
        options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.SnakeCaseLower;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

builder.Services
    .AddTransient<CategoryRepository>()
    .AddTransient<ProductRepository>()
    .AddTransient<ColorRepository>()
    .AddTransient<BrandRepository>()
    .AddTransient<StateRepository>()
    .AddTransient<CityRepository>()
    .AddTransient<NotificationRepository>()
    .AddTransient<UserRepository>()
    .AddTransient<UserAddressRepository>()
    .AddSingleton<ImageRepository>()
    .AddSingleton<JwtTokenService>();

builder.Services.AddDbContextPool<ApiDbContext>((sp, options) => {
    options.UseNpgsql(builder.Configuration.GetConfig("DB_CONNSTRING"));
    options.UseSnakeCaseNamingConvention();
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.OpenApiInfo
    {
        Version = "v1.0.0",
        Title = "backend",
        Description = "backend API"
    });
    options.DocInclusionPredicate((docName, apiDesc) =>
    {
        if (apiDesc.CustomAttributes().Any(attr => attr is SwaggerExcludeAttribute))
        {
            return false;
        }
        return true;
    });
    var xmlFilename = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

builder.Services.AddCors(options => { 
    options.AddPolicy("AllowAll", policy => 
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader()
    );
});

// JWT Authentication
var jwtSecretKey = builder.Configuration.GetConfig("JWT_SECRET_KEY");
var jwtIssuer = builder.Configuration["JWT_ISSUER"] ?? "online-shop-api";
var jwtAudience = builder.Configuration["JWT_AUDIENCE"] ?? "online-shop-client";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecretKey))
    };
});

builder.Services.AddAuthorization();

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<MainMiddleware>();
app.MapControllers();
app.Run();
