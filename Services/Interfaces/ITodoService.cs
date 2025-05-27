using MyNewApp.Models;

namespace MyNewApp.Services.Interfaces;
public interface ITodoService
{
    Task<Todo?> GetTodoByIdAsync(int id);

    Task<List<Todo>> GetTodosAsync();

    Task<bool> DeleteTodoByIdAsync(int id);

    Task<Todo> AddTodoAsync(Todo Task);
}
