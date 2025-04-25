using Microsoft.EntityFrameworkCore;

namespace AnotherTodoApi.Api.Repository;

class TodoDbContext : DbContext
{
    public TodoDbContext(DbContextOptions<TodoDbContext> options)
        : base(options)
    {
    }

    public DbSet<Todo> Todos => Set<Todo>();
}