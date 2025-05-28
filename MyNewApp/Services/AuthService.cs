using Microsoft.EntityFrameworkCore;
using MyNewApp.Data;
using MyNewApp.Models;
using MyNewApp.Services.Interfaces;

namespace MyNewApp.Services
{
    public class AuthService : IAuthService
    {
        private readonly TodoContext _context;

        public AuthService(TodoContext context)
        {
            _context = context;
        }

        public async Task<User?> ValidateUserAsync(string username, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (user is null) return null;

            bool isValid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
            return isValid ? user : null;
        }
    }
}
