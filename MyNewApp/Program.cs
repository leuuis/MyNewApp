using FluentValidation;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.EntityFrameworkCore;
using MyNewApp.Data;
using MyNewApp.Models;
using MyNewApp.Services;
using MyNewApp.Services.Interfaces;
using MyNewApp.Validators;
using DotNetEnv;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Detect seeding argument
var isSeeding = args.Contains("--seed");

Env.Load();

// Add appsettings.josn and environment variables
builder.Configuration
    .AddJsonFile("appsettings.json", optional: false)
    .AddEnvironmentVariables();

// Services
// Independencies Injection
builder.Services.AddControllers();
builder.Services.AddScoped<ITodoService, TodoService>();
builder.Services.AddScoped<IValidator<Todo>, TodoValidator>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();

// Configuration of EF Core with SQLite
builder.Services.AddDbContext<TodoContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// JWT Configuration binding
var jwtSettingsSection = builder.Configuration.GetSection("JwtSettings");
builder.Services.Configure<JwtSettings>(options =>
{
    jwtSettingsSection.Bind(options);
    options.PrivateKeyPath = Env.GetString("JWT_PRIVATE_KEY_PATH");
    options.PublicKeyPath = Env.GetString("JWT_PUBLIC_KEY_PATH");
});

// Load Public RSA key from PEM file
var publicKeyPath = Env.GetString("JWT_PUBLIC_KEY_PATH");
var publicKeyText = File.ReadAllText(publicKeyPath);

using var rsa = RSA.Create();
rsa.ImportFromPem(publicKeyText.ToCharArray());
var rsaSecurityKey = new RsaSecurityKey(rsa);

// Configuration JWT for authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidIssuer = jwtSettingsSection["Issuer"],
        ValidAudience = jwtSettingsSection["Audience"],
        IssuerSigningKey = rsaSecurityKey
    };
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

    // Scheme JWT
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Introduce el token JWT en este formato: Bearer {tu token}"
    });

    // Require scheme
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// User seeder
if (isSeeding)
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<TodoContext>();
    context.Database.Migrate();

    Console.WriteLine("Aplicando seeding de usuarios...");
    DataSeeder.SeedUsers(context);
    Console.WriteLine("Seeding completado.");

    return;
}

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

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
