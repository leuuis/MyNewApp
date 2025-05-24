using System.Reflection.Metadata;
using Microsoft.AspNetCore.Rewrite;

var builder = WebApplication.CreateBuilder(args);

// Independencies Injections
builder.Services.AddSingleton<ITaskService>(new InMemoryTaskService());

var app = builder.Build();

// Middleware to rewrite the URL
app.UseRewriter(new RewriteOptions()
    .AddRedirect("tasks/(.*)", "todos/$1"));

// My own middleware to log the request
app.Use(async (context, next) =>
{
    Console.WriteLine($"Request: {context.Request.Method} {context.Request.Path} {DateTime.UtcNow} Started.");
    await next.Invoke();
    Console.WriteLine($"Request: {context.Request.Method} {context.Request.Path} {DateTime.UtcNow} Finished.");
});

app.MapGet("/", () => "I'm still alive YEAHHH!");

var todos = new List<Todo>();

app.MapGet("/todos", (ITaskService taskService) => taskService.GetTodos());

app.MapGet("/todos/{id}", (int id, ITaskService taskService) =>
{
    var targetTodo = taskService.GetTodoBodyId(id);
    return targetTodo is not null 
        ? Results.Ok(targetTodo) 
        : Results.NotFound();
});

app.MapPost("/todos", (Todo task, ITaskService taskService) =>
{
    taskService.AddTodo(task);
    return Results.Created($"/todos/{task.Id}", task);
})
.AddEndpointFilter(async (context, next) =>
{
    var taskArgument = context.GetArgument<Todo>(0);
    var errors = new Dictionary<string, string[]>();

    if (taskArgument.DueDate < DateTime.UtcNow)
    {
        errors.Add(nameof(Todo.DueDate), ["Cannot hav due date in the past time."]);
    }
    if (taskArgument.IsCompleted)
    {
        errors.Add(nameof(Todo.IsCompleted), ["Cannot add completed todo."]);
    }
    if (errors.Count > 0)
    {
        return Results.ValidationProblem(errors);
    }

    return await next(context);
});

app.MapDelete("/todos/{id}", (ITaskService taskService, int id) =>
{
    taskService.DeleteTodoById(id);
    return Results.NoContent();
});

app.Run();

public record Todo(int Id, string Name, DateTime DueDate, bool IsCompleted);

// Logic with interface and implementation
interface ITaskService
{
    Todo? GetTodoBodyId(int id);

    List<Todo> GetTodos();

    void DeleteTodoById(int id);

    Todo AddTodo(Todo Task);
}

class InMemoryTaskService : ITaskService
{
    private readonly List<Todo> _todos = [];

    public Todo AddTodo(Todo task)
    {
        _todos.Add(task);
        return task;
    }

    public void DeleteTodoById(int id)
    {
        _todos.RemoveAll(task => task.Id == id);   
    }

    public Todo? GetTodoBodyId(int id)
    {
        return _todos.SingleOrDefault(t => id == t.Id);
    }

    public List<Todo> GetTodos()
    {
        return _todos;
    }
}
