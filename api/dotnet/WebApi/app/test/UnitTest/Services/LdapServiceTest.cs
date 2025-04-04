using WebApi.Shared;
using Microsoft.Extensions.Configuration;
using Novell.Directory.Ldap;
using WebApi.Services.Auth;
using WebApi.Models;
using WebApi.Services.Sql;


namespace WebApi.Tests.Services
{
    public class LdapServiceTest
    {

        private readonly LdapService _ldap;
        private readonly AuthUserModel _user;
        private readonly string _userLdapUsername;
        private readonly string _userLdapPassword;

        public LdapServiceTest()
        {
            IConfiguration config = new ConfigurationBuilder().AddConfigs("API_").Build();
            _ldap = new(config);
            _ldap.SearchBases = [LdapBaseModel.ITUsers];
            _user = new(
                Username: config["TESTER_LDAP_USERNAME"] ?? throw new ConfigNotFoundException("TESTER_LDAP_USERNAME"),
                Password: config["TESTER_LDAP_PASSWORD"] ?? throw new ConfigNotFoundException("TESTER_LDAP_PASSWORD"));
            _userLdapUsername = config["LDAP_SEARCH_DN"] ?? throw new ConfigNotFoundException("LDAP_SEARCH_DN");
            _userLdapPassword = config["LDAP_SEARCH_DN_PASS"] ?? throw new ConfigNotFoundException("LDAP_SEARCH_DN_PASS");
        }

        [Fact]
        public async Task Connect()
        {
            try
            {
                var result = await _ldap.ConnectAsync(_userLdapUsername, _userLdapPassword);
                _ldap.Dispose();
                Assert.True(result);
            }
            catch (Exception ex)
            {
                Assert.Fail($"Test failed with exception: {ex.Message}");
            }
        }

        [Fact]
        public async Task AuthenticateUser()
        {
            try
            {
                var result = await _ldap.AuthenticateAsync(_user);
                _ldap.Dispose();
                Assert.True(result);
            }
            catch (Exception ex)
            {
                Assert.Fail($"Test failed with exception: {ex.Message}");
            }
}
    }
}
