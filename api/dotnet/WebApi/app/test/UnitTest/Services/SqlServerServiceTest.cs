using WebApi.Services.Sql;
using WebApi.Models;
using Microsoft.Extensions.Configuration;
using WebApi.Shared;


namespace WebApi.Tests.Services
{
    public class SqlServerServiceTest
    {
        SqlServerService _sqlServerService;
        SqlOptionModel _options;

        public SqlServerServiceTest()
        {
            IConfiguration config = new ConfigurationBuilder().AddConfigs("API_").Build();
            _sqlServerService = new();
            _options = new()
            {
                Database = new SqlDatabaseModel 
                { 
                    Server = config["TESTER_SQL_SERVER"] ?? throw new ConfigNotFoundException("TESTER_SQL_SERVER"), 
                    Database= config["TESTER_SQL_DATABASE"] ?? throw new ConfigNotFoundException("TESTER_SQL_DATABASE")
                },
                Credentials = new SqlCredentialModel 
                { 
                    Username = config["TESTER_SQL_USERNAME"] ?? throw new ConfigNotFoundException("TESTER_SQL_USERNAME"), 
                    Password = config["TESTER_SQL_PASSWORD"] ?? throw new ConfigNotFoundException("TESTER_SQL_PASSWORD"),
                },
                PoolSize = 100
            };
        }

        [Fact]
        public async Task Connect()
        {
            try
            {
                var result = await _sqlServerService.QueryAsync<TestResultModel>("SELECT 1 AS id", options: _options);
                Assert.Single(result);
            }
            catch (Exception ex)
            {
                Assert.Fail($"Test failed with exception: {ex.Message}");
            }
        }
    }

    public class TestResultModel
    {
        public string id { get; set; }
    }
}
