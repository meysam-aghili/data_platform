using Serilog;
using WebApi.Shared;
using WebApi.Middlewares;
using System.Diagnostics;


var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddConfigs("API_");

builder.Host.UseSerilog();
Log.Logger = SerilogExtention.GetLogger(builder.Configuration);
Serilog.Debugging.SelfLog.Enable(msg => Debug.WriteLine(msg));

builder.Services.AddServices(builder.Configuration);

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
await app.UseCustomClientRateLimiting();
app.UseSwagger();
app.UseSwaggerUI();
//app.UseHttpsRedirection();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<ApiMiddleware>();
app.MapControllers();
app.Run();
