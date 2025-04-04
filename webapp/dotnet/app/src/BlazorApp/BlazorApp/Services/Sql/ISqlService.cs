using System.Collections.Specialized;
using BlazorApp.Models;


namespace BlazorApp.Services.Sql;

public interface ISqlService
{
    SqlOptionModel Options { get; set; }

    //Task ExecuteAsync(
    //    string query,
    //    object? parameters = null,
    //    SqlOptionModel? options = null);

    Task<IEnumerable<T>> QueryAsync<T>(
        string query,
        object? parameters = null,
        SqlOptionModel? options = null) where T : new();

    Task<SqlStoredProcedureResultModel> ExecuteStoredProcedureAsync(
        SqlStoredProcedureModel proc,
        NameValueCollection? inputs = null,
        SqlOptionModel? options = null) => throw new NotImplementedException();

    Task<IEnumerable<string>> GetDatabasesAsync(SqlOptionModel? options = null, bool includeSystemDbs = false);

    //Task<IEnumerable<string>> GetSchemasAsync(SqlOptionModel? options = null);

    //Task<IEnumerable<string>> GetTablesAsync(SqlOptionModel? options = null);

    Task<IEnumerable<string>> GetStoredProceduresAsync(string schema, SqlOptionModel? options = null);

    Task<List<SqlStoredProcedureParameterModel>> GetStoredProcedureParametersAsync(
        SqlStoredProcedureModel proc,
        SqlOptionModel? options = null);
}