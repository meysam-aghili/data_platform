using Dapper;
using Microsoft.Data.SqlClient;
using System.Collections.Specialized;
using System.Data;
using BlazorApp.Models;
using BlazorApp.Services.Sql;
using BlazorApp.Shared;


namespace BlazorApp.Services.Sql;

public class SqlServerService : ISqlService
{
    public SqlOptionModel Options { get; set; } = new();
    private readonly SqlConnectionStringBuilder _constringBuilder = [];
   
    private string BuildConnectionString(SqlOptionModel options)
    {
        options.Validate();
        _constringBuilder.Clear();
        _constringBuilder.UserID = options.Credentials.Username;
        _constringBuilder.Password = options.Credentials.Password;
        _constringBuilder.DataSource = options.Database.Server;
        _constringBuilder.InitialCatalog = options.Database.Database ?? string.Empty;
        _constringBuilder.TrustServerCertificate = true;
        _constringBuilder.CommandTimeout = options.CommandTimeoutSeconds;
        if (options.PoolSize is not null)
        {
            _constringBuilder.MaxPoolSize = (int)options.PoolSize;
        }
        return _constringBuilder.ConnectionString;
    }

    public async Task<IEnumerable<T>> QueryAsync<T>(
        string query,
        object? parameters = null,
        SqlOptionModel? options = null) where T : new()
    {
        var mergedOptions = options | Options;
        using var connection = new SqlConnection(BuildConnectionString(mergedOptions));
        var results = await connection.QueryAsync<T>(query, param: parameters, commandTimeout: mergedOptions.CommandTimeoutSeconds);
        return results;
    }

    private static DynamicParameters PreprareStoredProcedureParameters(
        IEnumerable<SqlStoredProcedureParameterModel> spParameters,
        NameValueCollection inputParameters)
    {
        const string _DefaultPageSize = "100000";

        DynamicParameters execParameters = new();
        List<string> inputs = [.. inputParameters.AllKeys];
        foreach (var spParam in spParameters)
        {
            var value = inputParameters.Get(spParam.Name);
            if (spParam.IsInput && !inputs.Contains(spParam.Name))
            {
                if (spParam.Name == "PageNumber")
                {
                    value = "1";
                }
                else if (spParam.Name == "PageSize")
                {
                    value = _DefaultPageSize;
                }
                else { continue; }
            }
            execParameters.Add(
                name: spParam.Name,
                value: spParam.IsInput ? value : null,
                dbType: Enum.Parse<SqlDbType>(spParam.Type, true).AsDbType(),
                direction: spParam.IsInput ? ParameterDirection.Input : ParameterDirection.Output,
                size: spParam.MaxLength
                );
        }
        return execParameters;
    }

    private static List<SqlStoredProcedureParameterModel> ExtractOutputParameters(
        IEnumerable<SqlStoredProcedureParameterModel> spParameters,
        DynamicParameters execParameters)
    {
        List<SqlStoredProcedureParameterModel> outputParameters = [];
        foreach (var param in spParameters.Where(p => p.IsOutput))
        {
            SqlStoredProcedureParameterModel outputParameter = (SqlStoredProcedureParameterModel)param.Clone();
            outputParameter.Value = execParameters.Get<object>(outputParameter.Name);
            outputParameters.Add(outputParameter);
        }

        return outputParameters;
    }

    public async Task<SqlStoredProcedureResultModel> ExecuteStoredProcedureAsync(
        SqlStoredProcedureModel proc,
        NameValueCollection? inputs = null,
        SqlOptionModel? options = null)
    {
        SqlOptionModel mergedOptions = options | Options;
        List<SqlStoredProcedureParameterModel> spParams =
            await GetStoredProcedureParametersAsync(proc, options: mergedOptions);
        DynamicParameters execParams = PreprareStoredProcedureParameters(spParams, inputs ?? []);
        using var connection = new SqlConnection(BuildConnectionString(mergedOptions));

        var spExecResults = await connection.QueryMultipleAsync(
            $"[{proc.Schema}].[{proc.StoredProcedure}]",
            execParams,
            commandType: CommandType.StoredProcedure,
            commandTimeout: mergedOptions.CommandTimeoutSeconds);

        SqlStoredProcedureResultModel results = new();

        while (!spExecResults.IsConsumed)
        {
            var resultSet = spExecResults.Read<dynamic>();
            results.ResultSets.Add([.. resultSet]);
        }
        results.OutputParameters = ExtractOutputParameters(spParams, execParams);

        if (execParams.Get<object>("PageNumber") != null)
        {
            results.OutputParameters.Add(new() { Name = "metadata_PageNumber", Value = execParams.Get<object>("PageNumber"), IsOutput = true, Type = "int", MaxLength = 4 });
        }

        if (execParams.Get<object>("PageSize") != null)
        {
            results.OutputParameters.Add(new() { Name = "metadata_PageSize", Value = execParams.Get<object>("PageSize"), IsOutput = true, Type = "int", MaxLength = 4 });
        }

        return results;
    }

    public async Task<List<SqlStoredProcedureParameterModel>> GetStoredProcedureParametersAsync(
    SqlStoredProcedureModel proc,
        SqlOptionModel? options = null) => [..
            await QueryAsync<SqlStoredProcedureParameterModel>(
                query:
                """
                SELECT
                    REPLACE(sp.name, '@', '') AS [Name],
                    TYPE_NAME(sp.user_type_id) AS [Type],
                    sp.max_length AS [MaxLength],
                    sp.is_output AS [IsOutput]
                FROM
                    sys.objects AS so
                    INNER JOIN sys.parameters AS sp ON so.object_id = sp.object_id
                WHERE
                    so.name = @StoredProcedure
                    AND SCHEMA_NAME(SCHEMA_ID) = @Schema
                """,
                parameters: new
                {
                    proc.StoredProcedure,
                    proc.Schema
                },
                options: options | Options
            )
        ];

    public async Task<IEnumerable<string>> GetDatabasesAsync(
        SqlOptionModel? options = null,
        bool includeSystemDbs = false)
    {
        var query =
            "SELECT [name] AS [Name] FROM sys.databases"
            + (includeSystemDbs ? string.Empty : " WHERE[name] NOT IN ('master', 'tempdb', 'model', 'msdb')")
            + ";";
        // mergedOptions.Database.Database = string.Empty; // Is this really necessary?
        var dbs = await QueryAsync<dynamic>(query: query, options: options | Options);
        return from db in dbs select (string)db.Name;
    }

    //public async Task<IEnumerable<string>> GetSchemasAsync(SqlOptionModel? options = null)
    //{
    //    var query = "SELECT [name] AS [Name] FROM sys.schemas;";
    //    var schemas = await QueryAsync<dynamic>(query: query, options: options | Options);
    //    return from schema in schemas select (string)schema.Name;
    //}

    //public async Task ExecuteAsync(
    //    string query,
    //    object? parameters = null,
    //    SqlOptionModel? options = null)
    //{
    //    var mergedOptions = options | Options;
    //    using var connection = new SqlConnection(BuildConnectionString(mergedOptions));
    //    await connection.ExecuteAsync(query, param: parameters, commandTimeout: mergedOptions.CommandTimeoutSeconds);
    //}

    public async Task<IEnumerable<string>> GetStoredProceduresAsync(string schema, SqlOptionModel? options = null)
    {
        var sps = await QueryAsync<dynamic>(
            query:
            """
            SELECT
                [name] AS [Name]
            FROM
                sys.procedures
            WHERE
                SCHEMA_NAME(schema_id) = @Schema;
            """,
            parameters: new
            {
                Schema = schema
            },
            options: options | Options
        );
        return from sp in sps select (string)sp.Name;
    }

    //public async Task<IEnumerable<string>> GetTablesAsync(SqlOptionModel? options = null)
    //{
    //    var mergedOptions = options | Options;
    //    var tables = await QueryAsync<dynamic>(
    //        query:
    //        """
    //        SELECT
    //            TABLE_NAME AS [Table]
    //        FROM
    //            INFORMATION_SCHEMA.TABLES
    //        WHERE
    //            TABLE_TYPE = 'BASE TABLE';
    //        """, options: mergedOptions
    //    );
    //    return from table in tables select (string)table.Table;
    //}
}
