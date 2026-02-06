using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Shared.Models;
using Api.Shared;

namespace Api.Services;

public class JwtTokenService(IConfiguration config)
{
    private readonly string _secretKey = config.GetConfig("JWT_SECRET_KEY");
    private readonly string _issuer = config.GetConfig("JWT_ISSUER");
    private readonly string _audience = config.GetConfig("JWT_AUDIENCE");
    private readonly int _expirationMinutes = int.Parse(config.GetConfig("JWT_EXPIRATION_MINUTES"));

    public string GenerateToken(User user)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Slug),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_expirationMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

