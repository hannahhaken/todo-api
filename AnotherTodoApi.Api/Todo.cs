namespace AnotherTodoApi.Api;

public class Todo
{
    public int Id { get; init; }
    public required string Name { get; set; }
    public bool IsComplete { get; set; }
    public DateTime? DueDate { get; set; }

    public bool SetDueDate(DateTime dueDate)
    {
        if (dueDate < DateTime.UtcNow) return false;
        DueDate = dueDate;
        return true;
    }
}