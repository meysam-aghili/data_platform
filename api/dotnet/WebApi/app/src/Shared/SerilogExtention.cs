using Microsoft.Data.SqlClient;
using Serilog;
using Serilog.Core;
using Serilog.Sinks.Elasticsearch;
using Serilog.Sinks.MSSqlServer;
using System.Data;
using System.Reflection;
using WebApi.Shared;


namespace WebApi.Shared;

public static class SerilogExtention
{
    public static ElasticsearchSinkOptions ConfigureElasticsearchSink(IConfiguration configuration) =>
        new ElasticsearchSinkOptions(new Uri(
            configuration["ELASTICSEARCH_ADDRESS"]
            ?? throw new ConfigNotFoundException("ELASTICSEARCH_ADDRESS")))
        {
            AutoRegisterTemplate = true,
            DetectElasticsearchVersion = false,
            AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv7,
            IndexFormat =
                Assembly.GetExecutingAssembly().GetName().Name?.ToLower().Replace(".", "-")
                + '-'
                + Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")?.ToLower().Replace(".", "-")
                + '-'
                + $"{DateTime.UtcNow:yyyy-MM}",
            ModifyConnectionSettings = x => x.BasicAuthentication(configuration["ELASTICSEARCH_USERNAME"], configuration["ELASTICSEARCH_PASSWORD"]),
            FailureCallback = (evt, exc) => Console.WriteLine("unable to submit event: " + evt.MessageTemplate + exc.Message),
            EmitEventFailure =
                EmitEventFailureHandling.WriteToSelfLog
                | EmitEventFailureHandling.WriteToFailureSink
                | EmitEventFailureHandling.RaiseCallback
        };

    public static string GetMssqlSinkConstring(IConfiguration config)
    {
        SqlConnectionStringBuilder _logConstringBuilder = [];
        _logConstringBuilder.UserID = config["SQL_LOG_USERNAME"];
        _logConstringBuilder.Password = config["SQL_LOG_PASSWORD"];
        _logConstringBuilder.DataSource = config["SQL_LOG_SERVER"];
        _logConstringBuilder.InitialCatalog = config["SQL_LOG_DATABASE"];
        _logConstringBuilder.TrustServerCertificate = true;
        return _logConstringBuilder.ConnectionString;
    }

    public static MSSqlServerSinkOptions GetMssqlSinkOptions(IConfiguration config) =>
        new()
        {
            SchemaName = config["SQL_LOG_SCHEMA_NAME"],
            TableName = config["SQL_LOG_TABLE_NAME"],
            AutoCreateSqlTable = false
        };

    private static ColumnOptions GetColumnOptions()
    {
        var options = new ColumnOptions
        {
            AdditionalColumns =
            [
                new()
                {
                    ColumnName = "Status",
                    PropertyName = "status",
                    DataType = SqlDbType.SmallInt
                },
                new()
                {
                    ColumnName = "Endpoint",
                    PropertyName = "endpoint",
                    DataType = SqlDbType.NVarChar,
                    DataLength = 100
                },
                new()
                {
                    ColumnName = "User",
                    PropertyName = "user",
                    DataType = SqlDbType.NVarChar,
                    DataLength = 50
                },
                new()
                {
                    ColumnName = "TraceId",
                    PropertyName = "traceId",
                    DataType = SqlDbType.NVarChar,
                    DataLength = 30
                },
                new()
                {
                    ColumnName = "ResponseTime",
                    PropertyName = "elapsed",
                    DataType = SqlDbType.Int
                },
                new()
                {
                    ColumnName = "MachineName",
                    PropertyName = "MachineName",
                    DataType = SqlDbType.NVarChar,
                    DataLength = 50
                },
                new()
                {
                    ColumnName = "Environment",
                    PropertyName = "Environment",
                    DataType = SqlDbType.NVarChar,
                    DataLength = 30
                },
                new()
                {
                    ColumnName = "IP",
                    PropertyName = "X-Forwarded-Host",
                    DataType = SqlDbType.NVarChar,
                    DataLength = 20
                },
                new()
                {
                    ColumnName = "Query",
                    PropertyName = "query",
                    DataType = SqlDbType.NVarChar,
                    DataLength = -1
                },
                new()
                {
                    ColumnName = "Method",
                    PropertyName = "method",
                    DataType = SqlDbType.NVarChar,
                    DataLength = 10
                }
            ]
        };
        options.Store.Remove(StandardColumn.MessageTemplate);
        options.Store.Remove(StandardColumn.Message);
        options.Store.Remove(StandardColumn.Properties);
        return options;
    }

    public static Logger GetLogger(IConfiguration config) => new LoggerConfiguration()
        .Enrich.FromLogContext()
        .Enrich.WithMachineName()
        .Enrich.WithHeaders(() =>
        {
            return ["X-Forwarded-Host"];
        })
        .Enrich.With()
        .Enrich.WithProperty("Environment", Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"))
        .ReadFrom.Configuration(config)
        .WriteTo.Debug()
        .WriteTo.Console()
        .WriteTo.Elasticsearch(ConfigureElasticsearchSink(config))
        .WriteTo.Conditional(
            evt => 
            //evt.Properties.GetValueOrDefault("api")?.ToString()
            true.ToString() == true.ToString(),
            sink => sink
                .MSSqlServer(
                    GetMssqlSinkConstring(config),
                    sinkOptions: GetMssqlSinkOptions(config),
                    columnOptions: GetColumnOptions()
                    )
                )
        .CreateLogger();
}
