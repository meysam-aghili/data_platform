using Microsoft.Data.SqlClient;
using BlazorApp.Models;


namespace BlazorApp.Services;

public interface ISqlServerProviderService
{
    SqlCredentialModel GetCredentials(SqlUser username);
    //string GetRoutableAddress(ApiSqlServer server);
}
