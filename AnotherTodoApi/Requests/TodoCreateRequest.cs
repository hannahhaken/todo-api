namespace AnotherTodoApi.Requests;

public class TodoCreateRequest
{
    public string Name { get; set; }
    public bool IsComplete { get; set; }
}