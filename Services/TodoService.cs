using Microsoft.EntityFrameworkCore;
using MyNewApp.Data;
using MyNewApp.Models;
using MyNewApp.Services.Interfaces;

class TodoService(TodoContext context) : ITodoService
{
    private readonly TodoContext _context = context;

    public async Task<Todo> AddTodoAsync(Todo task)
    {
        _context.Todos.Add(task);
        await _context.SaveChangesAsync();
        return task;
    }

    public async Task<bool> DeleteTodoByIdAsync(int id)
    {
        var todo = await _context.Todos.FindAsync(id);
        if (todo == null) return false;

        _context.Todos.Remove(todo);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<Todo?> GetTodoByIdAsync(int id)
    {
        return await _context.Todos.FindAsync(id);
    }

    public async Task<List<Todo>> GetTodosAsync()
    {
        return await _context.Todos.ToListAsync();
    }
}