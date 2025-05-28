using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyNewApp.Models;
using MyNewApp.Services.Interfaces;

namespace MyNewApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class TodoController(ITodoService todoService) : ControllerBase
    {
        private readonly ITodoService _todoService = todoService;

        [Authorize(Roles = "admin, user")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var targetTodo = await _todoService.GetTodoByIdAsync(id);
            return targetTodo is not null
                ? Ok(targetTodo)
                : NotFound();
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<IActionResult> AddTodoAsync([FromBody] Todo todo, [FromServices] IValidator<Todo> validator)
        {
            var validationResult = await validator.ValidateAsync(todo, options => options.IncludeRuleSets("Create"));
    
            if (!validationResult.IsValid)
            {
                return BadRequest(new ValidationProblemDetails(validationResult.ToDictionary()));
            }

            // TODO: Improve error management with a Middleware
            try
            {
                var added = await _todoService.AddTodoAsync(todo);
                return Created($"/todos/{added.Id}", added);
            }
            catch (Exception ex)
            {
                return Conflict(new { error = ex.Message });
            }
            
        }

        [HttpGet("health")]
        public IActionResult Health()
        {
            return Ok(new { status = "Healthy", message = "I'm still alive YEAHHH!" });
        }

        [Authorize(Roles = "admin, user")]
        [HttpGet()]
        public async Task<IActionResult> GetTodoAsync()
        {
            var todos = await _todoService.GetTodosAsync();
            return Ok(todos);
        }

        [Authorize(Roles = "admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodoByIdAsync(int id)
        {
            var deleted = await _todoService.DeleteTodoByIdAsync(id);
            return deleted ? NoContent() : NotFound();
        }

        [Authorize(Roles = "admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTodoByIdAsync(int id, Todo todo)
        {
            var updated = await _todoService.UpdateTodoByIdAsync(id, todo);
            return updated is not null ? Ok(updated) : NotFound();
        }
    }
}