namespace AnotherTodoApi.Api.Api.Requests;

public class TodoCreateRequest
{
    public required string Name { get; set; }
    public bool IsComplete { get; set; }

    public TodoCreateRequest()
    {
    }

    public TodoCreateRequest(Todo todoItem)
    {
        Name = todoItem.Name;
        IsComplete = todoItem.IsComplete;
    }
}