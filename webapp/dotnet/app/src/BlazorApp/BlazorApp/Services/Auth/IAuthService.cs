using System.Security.Claims;
using BlazorApp.Models;


namespace BlazorApp.Services.Auth;

public interface IAuthService
{
    //JwtToken IssueJwtToken(string username, IEnumerable<string> roles);
    ClaimsPrincipal GenerateClaimsPrincipal(string authenticationScheme, string userId, IEnumerable<string> roles);
    int ValidMinutes { get; }
}
