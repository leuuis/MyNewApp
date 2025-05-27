using Microsoft.EntityFrameworkCore;
using MyNewApp.Data;
using MyNewApp.Models;
using MyNewApp.Services;

namespace MyNewApp.Tests.Services
{
    public class TodoServiceTests
    {
        private TodoContext GetInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<TodoContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new TodoContext(options);
            context.Database.EnsureDeleted();  // This cleans db
            context.Database.EnsureCreated();  // This creates db again
            return context;
        }

        [Fact]
        public async Task AddTodoAsync_ShouldAddTodoSuccessfully()
        {
            // Arrange
            using var context = GetInMemoryContext();
            var service = new TodoService(context);
            var newTodo = new Todo
            {
                Name = "Todo xUnit",
                DueDate = DateTime.UtcNow.AddDays(1),
                IsCompleted = false
            };

            // Act
            var result = await service.AddTodoAsync(newTodo);

            // Assert
            var insertedTodo = await context.Todos.FirstOrDefaultAsync();
            Assert.NotNull(insertedTodo);
            Assert.Equal("Todo xUnit", insertedTodo!.Name);
            Assert.False(insertedTodo.IsCompleted);
            Assert.True(insertedTodo.DueDate > DateTime.UtcNow);
        }
    }
}
