using Elastic.Ingest.Elasticsearch.DataStreams;
using Elastic.Serilog.Sinks;
using Elastic.Transport;
using Serilog;
using Serilog.Core;

namespace Api.Shared;

public static class SerilogConfig
{
    public static Logger GetLogger(IConfiguration config) =>
        new LoggerConfiguration()
        .Enrich.FromLogContext()
        .Enrich.WithMachineName()
        .Enrich.WithHeaders(() => ["X-Forwarded-Host"])
        .Enrich.WithProperty("Environment", Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"))
        .ReadFrom.Configuration(config)
        .WriteTo.Debug()
        .WriteTo.Console()
        //.WriteTo.Elasticsearch(new[] { new Uri(config.GetConfig("ELASTICSEARCH_URL")) }, options =>
        //{
        //    options.DataStream = new DataStreamName("logs", config.GetConfig("ELASTICSEARCH_DATASET_NAME"), Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "development");
        //    options.BootstrapMethod = Elastic.Ingest.Elasticsearch.BootstrapMethod.Failure;
        //}, transport =>
        //{
        //    transport.Authentication(
        //        new BasicAuthentication(
        //            config.GetConfig("ELASTICSEARCH_USERNAME"),
        //            config.GetConfig("ELASTICSEARCH_PASSWORD")
        //        )
        //    );
        //})    
        .CreateLogger();
}
