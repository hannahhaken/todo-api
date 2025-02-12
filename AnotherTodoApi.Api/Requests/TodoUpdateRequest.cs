namespace AnotherTodoApi.Api.Requests;

public class TodoUpdateRequest
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public bool IsComplete { get; set; }
}