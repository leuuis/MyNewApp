using Microsoft.EntityFrameworkCore;
using MyNewApp.Models;

namespace MyNewApp.Data
{
    public class TodoContext : DbContext
    {
        public TodoContext(DbContextOptions<TodoContext> options)
            : base(options) { }
        public DbSet<Todo> Todos {get; set; }
    }
}