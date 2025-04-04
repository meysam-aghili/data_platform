using WebApi.Models;
using WebApi.Services;
using Microsoft.Extensions.Configuration;
using WebApi.Shared;


namespace UnitTest.Services
{
    public class MongoServiceTest
    {
        public MongoService<ApiBsonDocument> _apiClient;
        public MongoService<SwarmJobBsonDocument> _jobClient;
        public MongoService<RoleModel> _roleClient;
        public MongoServiceTest()
        {
            IConfiguration config = new ConfigurationBuilder().AddConfigs("API_").Build();
            _apiClient = new(config, null);
            _jobClient = new(config, null);
            _roleClient = new(config, null);
        }

        [Fact]
        public void GetApiDocument()
        {
            try
            {
                IQueryable<ApiBsonDocument>? itemsQuery =
                    (from item in _apiClient.Query().Take(1)
                     select item);
                Assert.NotNull(itemsQuery);
                Assert.Equal(1, itemsQuery.Count());
            }
            catch (Exception ex)
            {
                Assert.Fail($"Test failed with exception: {ex.Message}");
            }
}

        [Fact]
        public void GetJobDocument()
        {
            try
            {
                IQueryable<SwarmJobBsonDocument>? itemsQuery =
                (from item in _jobClient.Query().Take(1)
                 select item);
                Assert.NotNull(itemsQuery);
                Assert.Equal(1, itemsQuery.Count());
            }
            catch (Exception ex)
            {
                Assert.Fail($"Test failed with exception: {ex.Message}");
            }
        }

        [Fact]
        public void GetRoleDocument()
        {
            try
            {
                IQueryable<RoleModel>? itemsQuery =
                    (from item in _roleClient.Query().Take(1)
                     select item);
                Assert.NotNull(itemsQuery);
                Assert.Equal(1, itemsQuery.Count());
            }
            catch (Exception ex)
            {
                Assert.Fail($"Test failed with exception: {ex.Message}");
            }
        }
    }
}