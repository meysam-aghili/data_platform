using WebApi.Models;
using WebApi.Services.Auth;
using WebApi.Shared;
using Microsoft.AspNetCore.Mvc;
using WebApi.Services;


namespace WebApi.Controllers;

[ApiController]
public class AuthController : BaseController
{
    private readonly ILogger<AuthController> _logger;
    private readonly ILdapService _ldap;
    private readonly IAuthService _auth;
    private readonly IApiService _api;

    public AuthController(
        ILogger<AuthController> logger,
        IConfiguration config,
        IApiService api,
        ILdapService ldap,
        IAuthService auth
        ) : base(logger, config)
    {
        _logger = logger;
        _auth = auth;
        _api = api;
        _ldap = ldap;
        _ldap.SearchBases = [LdapBaseModel.ITUsers];
    }

    [HttpPost("token")]
    public async Task<IActionResult> IssueToken([FromBody] AuthUserModel user)
    {
        if (string.IsNullOrWhiteSpace(user.Username) || string.IsNullOrWhiteSpace(user.Password))
        {
            return await HandleUnauthorized();
        }
        if (!await _ldap.AuthenticateAsync(user))
        {
            return await HandleUnauthorized();
        }
        var roles = await _api.GetUserRoleClaimsAsync(user.Username.ToLower());
        var token = _auth.IssueJwtToken(user.Username.ToLower(), roles);
        LogHelper.LogRequest(_logger, HttpContext, 0);
        return Ok(token);
    }
}
