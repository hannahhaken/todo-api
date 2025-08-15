using AnotherTodoApi.Api;

namespace AnotherTodoApi.Tests.Unit;

public class TodoTests
{
    [Fact]
    public void WhenValidDate_ShouldSetDueDate()
    {
        var todo = new Todo { Id = 1, Name = "Test todo", IsComplete = false };
        var futureDate = DateTime.UtcNow.AddDays(3);

        todo.SetDueDate(futureDate);

        Assert.Equal(futureDate, todo.DueDate);
    }

    [Fact]
    public void WhenDateIsInPast_ShouldReturnFalseAndNotSetDueDate()
    {
        var todo = new Todo { Id = 1, Name = "Test todo", IsComplete = false };
        var pastDate = DateTime.UtcNow.AddDays(-3);
        
        var result = todo.SetDueDate(pastDate);
        
        Assert.False(result);
        Assert.Null(todo.DueDate);
    }
}