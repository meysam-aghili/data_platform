using System.Text;
using WebApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Configuration;
using WebApi.Shared;
using System.Collections;


namespace UnitTest.Controllers
{
    public class ApiControllerTest
    {
        private readonly AuthUserModel _user;
        private readonly string _baseUrl;

        public ApiControllerTest()
        {
            IConfiguration config = new ConfigurationBuilder().AddConfigs("API_").Build();
            _user = new(
                Username: config["TESTER_LDAP_USERNAME"] ?? throw new ConfigNotFoundException("TESTER_LDAP_USERNAME"), 
                Password: config["TESTER_LDAP_PASSWORD"] ?? throw new ConfigNotFoundException("TESTER_LDAP_PASSWORD")) ;
            _baseUrl = config["BASE_URL"] ?? "http://localhost:80";
        }

        private async Task<string> GetToken()
        {
            using (HttpClient client = new HttpClient())
            {
                string jsonPayload = JsonConvert.SerializeObject(_user);
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
                string url = $"{_baseUrl}/token";
                HttpResponseMessage response = await client.PostAsync(url, content);
                string responseContent = await response.Content.ReadAsStringAsync();
                JObject responseJson = JObject.Parse(responseContent);
                var token = responseJson["token"];
                return token.ToString();
            }
        }

        [Fact]
        public async Task api()
        {
            try
            {
                string token = await GetToken();
                Assert.True(token.Length > 100);
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                    string url = $"{_baseUrl}/api/sales-target-values";
                    HttpResponseMessage response = await client.GetAsync(url);
                    Assert.Equal(200, (int)response.StatusCode);
                    string responseContent = await response.Content.ReadAsStringAsync();
                    JObject responseJson = JObject.Parse(responseContent);
                    var took = responseJson["took"];
                    Assert.NotNull(took);
                    var data = responseJson["data"];
                    Assert.NotNull(data);
                    Assert.True(data.Count() > 0);
                }
            }
            catch (Exception ex)
            {
                Assert.Fail($"Test failed with exception: {ex.Message}");
            }
        }

        [Fact]
        public async Task apiParams()
        {
            try 
            { 
                using (HttpClient client = new HttpClient())
                {
                    string url = $"{_baseUrl}/api/sales-target-values/params";
                    HttpResponseMessage response = await client.GetAsync(url);
                    Assert.Equal(200, (int)response.StatusCode);
                    string responseContent = await response.Content.ReadAsStringAsync();
                    Assert.NotNull(responseContent);
                    Assert.Contains("[", responseContent);
                }
            }
            catch (Exception ex)
            {
                Assert.Fail($"Test failed with exception: {ex.Message}");
            }
        }

        [Fact]
        public async Task job()
        {
            try 
            { 
                string token = await GetToken();
                Assert.True(token.Length > 100);
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                    string url = $"{_baseUrl}/api/job/test-job";
                    HttpResponseMessage response = await client.PostAsync(url, null);
                    Assert.Equal(200, (int)response.StatusCode);
                    string responseContent = await response.Content.ReadAsStringAsync();
                    JObject responseJson = JObject.Parse(responseContent);
                    var metadata = responseJson["metadata"];
                    var containerId = metadata["containerId"];
                    Assert.NotNull(containerId);
                    Assert.True(containerId.ToString().Length > 1);
                }
            }
            catch (Exception ex)
            {
                Assert.Fail($"Test failed with exception: {ex.Message}");
            }
        }

        [Fact]
        public async Task apiUpdateCache()
        {
            try 
            { 
                using (HttpClient client = new HttpClient())
                {
                    string url = $"{_baseUrl}/api/sales-target-values/update-cache";
                    HttpResponseMessage response = await client.PutAsync(url, null);
                    Assert.Equal(204, (int)response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                Assert.Fail($"Test failed with exception: {ex.Message}");
            }
        }

    }
}