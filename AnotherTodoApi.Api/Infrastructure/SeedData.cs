using AnotherTodoApi.Api.Repository;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace AnotherTodoApi.Api.Infrastructure;

public static class SeedData
{
    public static void Initialise(TodoDbContext context)
    {
        if (context.Database.GetPendingMigrations().Any())
        {
            Console.WriteLine("Pending migrations detected. Seeding aborted to avoid data inconsistency.");
            return;
        }

        using var transaction = context.Database.BeginTransaction();

        try
        {
            if (!context.Todos.Any())
            {
                context.Todos.AddRange(
                    new Todo { Name = "Learn about EF Core", IsComplete = false },
                    new Todo { Name = "Seed database with data", IsComplete = true },
                    new Todo { Name = "Order coffee beans", IsComplete = false },
                    new Todo { Name = "Clean windows", IsComplete = false },
                    new Todo { Name = "Pack for holiday", IsComplete = false }
                );

                context.SaveChanges();
            }

            transaction.Commit();
        }
        catch (Exception e)
        {
            Log.Error(e, "An error occurred while seeding the database.");
            transaction.Rollback();
            throw;
        }
    }
}