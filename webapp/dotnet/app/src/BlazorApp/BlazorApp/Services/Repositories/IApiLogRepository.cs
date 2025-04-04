namespace BlazorApp.Services.Repositories;

public interface IApiLogRepository
{
    Task<List<Dictionary<string, object>>> GetPrevious30DaysUsageAsync(string slug);
}
