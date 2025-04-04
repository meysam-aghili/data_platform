using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using WebApi.Models;
using WebApi.Shared;


namespace WebApi.Services.Auth;

public class AuthService(IHttpContextAccessor _context, IConfiguration config) : IAuthService
{
    private readonly string _key = config["AUTH_JWT_KEY"] ?? throw new ConfigNotFoundException("AUTH_JWT_KEY");
    private readonly string _issuer = config["AUTH_JWT_ISSUER"] ?? "dataplatform.com";
    private readonly string _audience = config["AUTH_JWT_AUDIENCE"] ?? "dataplatform";
    private readonly int _lifetimeMinutes = int.Parse(config["AUTH_MINUTES_TO_EXPIRE"] ?? (60 * 24 * 2).ToString());

    public JwtToken IssueJwtToken(string username, IEnumerable<string> roles)
    {
        var idClaim = new Claim(ClaimTypes.Name, username);
        var roleClaims = from role in roles select new Claim(ClaimTypes.Role, role);
        List<Claim> claims = [idClaim, .. roleClaims];

        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenKey = Encoding.ASCII.GetBytes(_key);
        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(tokenKey),
            SecurityAlgorithms.HmacSha256Signature);
        var tokenDescriptor = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_lifetimeMinutes),
            signingCredentials: credentials);
        var token = tokenHandler.WriteToken(tokenDescriptor);
        return new JwtToken
        {
            Token = token,
            ExpiresAt = DateTimeOffset.Now.AddMinutes(ValidMinutes)
        };
    }

    //public ClaimsPrincipal GenerateClaimsPrincipal(string authenticationScheme, string userId, IEnumerable<string> roles)
    //{
    //    var idClaim = new Claim(ClaimTypes.Name, userId);
    //    // The expiration is not correct.
    //    // var exprClaim = new Claim(ClaimTypes.Expiration, _lifetimeDays.ToString());
    //    var roleClaims = from role in roles select new Claim(ClaimTypes.Role, role);
    //    // Microsoft.AspNetCore.Authentication.BearerToken.BearerTokenDefaults.AuthenticationScheme
    //    var identity = new ClaimsIdentity(authenticationType: authenticationScheme, claims: [idClaim, /*exprClaim,*/ .. roleClaims]);
    //    var principal = new ClaimsPrincipal(identity);
    //    return principal;
    //}

    //public async Task<LdapUserModel?> GetLoggedOnUserAsync()
    //{
    //    var principal = _context.HttpContext?.User.Identity?.Name;
    //    if (principal is null)
    //    {
    //        return null;
    //    }
    //    var user = await _ldapRepo.GetUserAsync(principal);
    //    return user;
    //}

    //public bool IsUserInRole(string role) => _context.HttpContext?.User.IsInRole(role) ?? false;

    public int ValidMinutes => _lifetimeMinutes;
}
