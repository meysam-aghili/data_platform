using Microsoft.Data.SqlClient;
using BlazorApp.Models;
using BlazorApp.Shared;


namespace BlazorApp.Services;

public class SqlServerProviderService(IConfiguration config) : ISqlServerProviderService
{
    public SqlCredentialModel GetCredentials(SqlUser user) => user switch
    {
        SqlUser.SqlApiUser => new()
        {
            Username = config["SQL_API_USER"] ?? string.Empty, // ?? throw  new BiConfigNotFoundException("TSQL_BIAPPS_USER"),
            Password = config["SQL_API_PASS"] ?? string.Empty // ?? throw  new BiConfigNotFoundException("TSQL_BIAPPS_PASS")
        },
        SqlUser.SqlApiUnmaskedUser => new()
        {
            Username = config["SQL_API_UNMASKED_USER"] ?? string.Empty, // ?? throw  new BiConfigNotFoundException("TSQL_BIAPI_USER"),
            Password = config["SQL_API_UNMASKED_PASS"] ?? string.Empty // ?? throw  new BiConfigNotFoundException("TSQL_BIAPI_PASS")
        },
        _ => throw new ConfigNotFoundException(nameof(user))
    };

    //public string GetRoutableAddress(ApiSqlServer server) => server switch
    //{
    //    ApiSqlServer.BiWarehousing => "biwarehousing.digikala.com",
    //    ApiSqlServer.BiMarketing => "bimarketing.digikala.com",
    //    ApiSqlServer.BiLog => "bilog.digikala.com",
    //    ApiSqlServer.BiTestDW => "bitestdw.digikala.com",
    //    ApiSqlServer.BiHA => "biha.digikala.com",
    //    _ => throw new ConfigNotFoundException(nameof(server))
    //};
}
