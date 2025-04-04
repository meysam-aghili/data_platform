using System.Security.Claims;
using WebApi.Models;


namespace WebApi.Services.Auth;

public interface IAuthService
{
    JwtToken IssueJwtToken(string username, IEnumerable<string> roles);
    //ClaimsPrincipal GenerateClaimsPrincipal(string authenticationScheme, string userId, IEnumerable<string> roles);
    int ValidMinutes { get; }
}
