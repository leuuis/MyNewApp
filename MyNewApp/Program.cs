using FluentValidation;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.EntityFrameworkCore;
using MyNewApp.Data;
using MyNewApp.Models;
using MyNewApp.Services;
using MyNewApp.Services.Interfaces;
using MyNewApp.Validators;

var builder = WebApplication.CreateBuilder(args);


// Independencies Injection
builder.Services.AddControllers();
builder.Services.AddScoped<ITodoService, TodoService>();
builder.Services.AddScoped<IValidator<Todo>, TodoValidator>();
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

app.UseAuthorization();
app.MapControllers();

app.Run();
