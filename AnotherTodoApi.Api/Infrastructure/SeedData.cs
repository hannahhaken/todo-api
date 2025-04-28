using AnotherTodoApi.Api.Repository;

namespace AnotherTodoApi.Api.Infrastructure;

public static class SeedData
{
    public static void Initialise(TodoDbContext context)
    {
        context.Database.EnsureCreated();
        if (!context.Todos.Any())
        {
            context.Todos.AddRange(
                new Todo { Name = "Learn about EF Core", IsComplete = false },
                new Todo { Name = "Seed database with data", IsComplete = true },
                new Todo { Name = "Order coffee beans", IsComplete = false },
                new Todo { Name = "Clean windows", IsComplete = false }
            );

            context.SaveChanges();
        }
    }
}