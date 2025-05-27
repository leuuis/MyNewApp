using Microsoft.AspNetCore.Rewrite;
using Microsoft.EntityFrameworkCore;
using MyNewApp.Data;
using MyNewApp.Models;
using MyNewApp.Services;
using MyNewApp.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);


// Independencies Injection
builder.Services.AddScoped<ITodoService, TodoService>();
builder.Services.AddDbContext<TodoContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
    c.RoutePrefix = "swagger"; // esto hace que cargue en /swagger
});

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

app.MapGet("/todos", (ITodoService taskService) => taskService.GetTodosAsync());

app.MapGet("/todos/{id}", async (int id, ITodoService taskService) =>
{
    var targetTodo = await taskService.GetTodoByIdAsync(id);
    return targetTodo is not null 
        ? Results.Ok(targetTodo) 
        : Results.NotFound();
});

app.MapPost("/todos", async (Todo task, ITodoService taskService) =>
{
    var added = await taskService.AddTodoAsync(task);
    return Results.Created($"/todos/{added.Id}", added);
})
.AddEndpointFilter(async (context, next) =>
{
    var taskArgument = context.GetArgument<Todo>(0);
    var errors = new Dictionary<string, string[]>();

    if (taskArgument.DueDate < DateTime.UtcNow)
    {
        errors.Add(nameof(Todo.DueDate), ["Cannot have due date in the past time."]);
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

app.MapDelete("/todos/{id}", async (ITodoService taskService, int id) =>
{
    var deleted = await taskService.DeleteTodoByIdAsync(id);
    return deleted ? Results.NoContent(): Results.NotFound();
});

app.MapPut("/todos/{id}", async (int id, Todo updateTask, ITodoService taskService) =>
{
    var updated = await taskService.UpdateTodoByIdAsync(id, updateTask);
    return updated is not null ? Results.Ok(updated) : Results.NotFound();
});

app.Run();
