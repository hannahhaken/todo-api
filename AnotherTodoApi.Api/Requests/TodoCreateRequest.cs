namespace AnotherTodoApi.Api.Requests;

public class TodoCreateRequest
{
    public string Name { get; set; }
    public bool IsComplete { get; set; }

    public TodoCreateRequest(Todo todoItem) => (Name, IsComplete) = (todoItem.Name, todoItem.IsComplete);
}