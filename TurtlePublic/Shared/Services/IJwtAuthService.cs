using Generated;
using System.Security.Claims;

namespace TurtlePublic.Services;

public interface IJwtAuthService
{
    (string, DateTime) GenerateToken(string signingKey, int seconds, List<Claim> claims);

    void GenerateToken(Account.WithRoles account, out Account.WithSession session);
}