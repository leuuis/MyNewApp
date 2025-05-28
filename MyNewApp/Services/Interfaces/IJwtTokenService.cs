using System.Security.Claims;

namespace MyNewApp.Services.Interfaces
{
    public interface IJwtTokenService
    {
        string GenerateToken(IEnumerable<Claim> claims);
    }
}
