using MyNewApp.Models;

namespace MyNewApp.Services.Interfaces
{
    public interface IJwtTokenService
    {
        string GenerateToken(User user);
    }
}
