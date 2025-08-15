using AnotherTodoApi.Api;
using AnotherTodoApi.Api.Repository;
using AnotherTodoApi.Api.Services;
using Microsoft.EntityFrameworkCore;

namespace AnotherTodoApi.Tests.Unit;

public class TodoServiceTests
{
    [Fact]
    public async Task ShouldReturnCorrectTodo_WhenValidIdProvided()
    {
        //arrange
        var options = new DbContextOptionsBuilder<TodoDbContext>()
            .UseInMemoryDatabase(databaseName: nameof(ShouldReturnCorrectTodo_WhenValidIdProvided))
            .Options;

        var dbContext = new TodoDbContext(options);

        var todo = new Todo { Id = 1, Name = "walk dog", IsComplete = true };
        dbContext.Add(todo);
        await dbContext.SaveChangesAsync();

        var todoService = new TodoService(dbContext);

        //act
        var result = await todoService.GetTodoByIdAsync(1);

        //assert
        Assert.NotNull(result);
        Assert.Equal("walk dog", result.Name);
    }

    [Fact]
    public async Task ShouldReturnNull_WhenTodoDoesNotExist()
    {
        //arrange
        var options = new DbContextOptionsBuilder<TodoDbContext>()
            .UseInMemoryDatabase(databaseName: nameof(ShouldReturnNull_WhenTodoDoesNotExist))
            .Options;

        var dbContext = new TodoDbContext(options);
        var todoService = new TodoService(dbContext);

        //act
        var result = await todoService.GetTodoByIdAsync(1);

        //assert
        Assert.Null(result);
    }
}