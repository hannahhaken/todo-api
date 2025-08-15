namespace AnotherTodoApi.Api;

public class Todo
{
    public int Id { get; init; }
    public required string Name { get; set; }
    public bool IsComplete { get; set; }
    public DateTime? DueDate { get; set; }

    public void SetDueDate(DateTime dueDate) => DueDate = dueDate;
}