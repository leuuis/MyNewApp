using MyNewApp.Models;

namespace MyNewApp.Services.Interfaces
{
    public interface IAuthService
    {
        Task<User?> ValidateUserAsync(string username, string password);
    }
}
