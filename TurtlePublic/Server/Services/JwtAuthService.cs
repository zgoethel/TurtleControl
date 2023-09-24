using Generated;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TurtlePublic.Extensions;

namespace TurtlePublic.Services;

public class JwtAuthService : IJwtAuthService
{
    private readonly IConfiguration config;
    public JwtAuthService(IConfiguration config)
    {
        this.config = config;
    }

    public (string, DateTime) GenerateToken(string signingKey, int seconds, List<Claim> claims)
    {
        var expires = DateTime.UtcNow.AddSeconds(seconds);
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var handler = new JwtSecurityTokenHandler();
        var token = handler.WriteToken(new JwtSecurityToken(claims: claims, expires: expires, signingCredentials: creds));
        return (token, expires);
    }

    public void GenerateToken(Account.WithRoles account, out Account.WithSession session)
    {
        session = account.ConvertTo<Account.WithSession>();
        var claims = new List<Claim>()
        {
            new(ClaimTypes.Name, account.Email)
        };

        var resetKey = config.GetValue<string>("Jwt:RefreshKey");
        var resetSeconds = config.GetValue<int>("Jwt:RefreshSeconds");
        (session.ResetToken, session.ResetExpiration) = GenerateToken(resetKey, resetSeconds, claims);

        foreach (var role in account.roles)
        {
            claims.Add(new(ClaimTypes.Role, role.Name));
        }

        var key = config.GetValue<string>("Jwt:Key");
        var lifespanSeconds = config.GetValue<int>("Jwt:LifespanSeconds");
        (session.SessionToken, session.SessionExpiration) = GenerateToken(key, lifespanSeconds, claims);
    }
}