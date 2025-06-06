namespace AnotherTodoApi.Api.Responses;

public class TodoItemResponse
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public bool IsComplete { get; set; }

    public TodoItemResponse()
    {
    }

    public TodoItemResponse(Todo todoItem) =>
        (Id, Name, IsComplete) = (todoItem.Id, todoItem.Name, todoItem.IsComplete);
}