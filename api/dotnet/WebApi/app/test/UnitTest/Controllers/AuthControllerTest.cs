using System.Text;
using WebApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Configuration;
using WebApi.Shared;


namespace UnitTest.Controllers
{
    public class AuthControllerTest
    {
        private readonly AuthUserModel _validUser;
        private readonly AuthUserModel _invalidUser;
        private readonly string _baseUrl;

        public AuthControllerTest()
        {
            IConfiguration config = new ConfigurationBuilder().AddConfigs("API_").Build();
            _validUser = new(
                Username: config["TESTER_LDAP_USERNAME"] ?? throw new ConfigNotFoundException("TESTER_LDAP_USERNAME"), 
                Password: config["TESTER_LDAP_PASSWORD"] ?? throw new ConfigNotFoundException("TESTER_LDAP_PASSWORD")) ;
            _invalidUser = _validUser with { Password = "1234"};
            _baseUrl = config["BASE_URL"] ?? "http://localhost:80";
        }

        private async Task<HttpResponseMessage> SendTokenRequest(AuthUserModel user)
        {
            using (HttpClient client = new HttpClient())
            {
                string jsonPayload = JsonConvert.SerializeObject(user);
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
                string url = $"{_baseUrl}/token";
                return await client.PostAsync(url, content);
            }
        }

        [Fact]
        public async Task tokenValidUser()
        {
            try 
            { 
                HttpResponseMessage response = await SendTokenRequest(_validUser);
                Assert.Equal(200, (int)response.StatusCode);
                string responseContent = await response.Content.ReadAsStringAsync();
                JObject responseJson = JObject.Parse(responseContent);
                var token = responseJson["token"];
                Assert.NotNull(token);
                Assert.IsType<string>(token.ToString());
                Assert.NotEmpty(token.ToString());
                Assert.True(token.ToString().Length > 100);
            }
            catch (Exception ex)
            {
                Assert.Fail($"Test failed with exception: {ex.Message}");
            }
        }

        [Fact]
        public async Task tokenInvalidUser()
        {
            try 
            { 
                HttpResponseMessage response = await SendTokenRequest(_invalidUser);
                Assert.Equal(401, (int)response.StatusCode);
            }
            catch (Exception ex)
            {
                Assert.Fail($"Test failed with exception: {ex.Message}");
            }
        }
    }
}