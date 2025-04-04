using BlazorApp.Services.Elastic;


namespace BlazorApp.Services.Repositories;

public class ApiLogRepository(IElasticService elastic) : IApiLogRepository
{
    private readonly IElasticService _elastic = elastic;

    public async Task<List<Dictionary<string, object>>> GetPrevious30DaysUsageAsync(string slug) =>
        await _elastic.QuerySqlAsync(@$"
            SELECT
                DATETIME_FORMAT(CAST(""@timestamp"" AS DATE), 'YYYY-MM-dd') date,
                fields.statusCode statusCode,
                COUNT(*) count
            FROM
                ""biapi-production-*""
            WHERE
                ""@timestamp"" >= TODAY() - INTERVAL 30 DAYS
                AND fields.apiSlug.keyword = '{slug}'
            GROUP BY
                DATETIME_FORMAT(CAST(""@timestamp"" AS DATE), 'YYYY-MM-dd'),
                fields.statusCode
            ");
}
