using Microsoft.Data.SqlClient;
using WebApi.Models;


namespace WebApi.Services;

public interface ISqlServerProviderService
{
    SqlCredentialModel GetCredentials(SqlUser username);
    //string GetRoutableAddress(ApiSqlServer server);
}
