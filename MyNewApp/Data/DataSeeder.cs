using MyNewApp.Models;

namespace MyNewApp.Data
{
    public static class DataSeeder
    {
        public static void SeedUsers(TodoContext context)
        {
            if (!context.Users.Any())
            {
                var admin = new User
                {
                    Username = "admin",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                    Role = "admin"
                };

                var standardUser = new User
                {
                    Username = "user",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("user123"),
                    Role = "user"
                };

                context.Users.AddRange(admin, standardUser);
                context.SaveChanges();
            }
        }
    }
}
