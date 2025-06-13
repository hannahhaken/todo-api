using AnotherTodoApi.Api.Api.Requests;
using AnotherTodoApi.Api.Repository;
using AnotherTodoApi.Api.Responses;
using Microsoft.EntityFrameworkCore;

namespace AnotherTodoApi.Api.Services;

public class TodoService
{
    private readonly TodoDbContext _dbContext;

    public TodoService(TodoDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<TodoItemResponse>> GetAllTodosAsync()
    {
        return await _dbContext.Todos
            .Select(todo => new TodoItemResponse(todo))
            .ToListAsync();
    }

    public async Task<List<TodoItemResponse>> GetCompleteTodosAsync()
    {
        return await _dbContext.Todos
            .Where(t => t.IsComplete)
            .Select(todo => new TodoItemResponse(todo))
            .ToListAsync();
    }

    public async Task<TodoItemResponse?> GetTodoByIdAsync(int id)
    {
        var todo = await _dbContext.Todos.FindAsync(id);
        return todo is null ? null : new TodoItemResponse(todo);
    }

    public async Task<TodoItemResponse> CreateTodoAsync(Todo todo)
    {
        _dbContext.Todos.Add(todo);
        await _dbContext.SaveChangesAsync();
        return new TodoItemResponse(todo);
    }

    public async Task<TodoItemResponse?> UpdateTodoAsync(int id, TodoUpdateRequest todoUpdateRequest)
    {
        var todo = await _dbContext.Todos.FindAsync(id);
        if (todo is null)
        {
            return null;
        }

        todo.Name = todoUpdateRequest.Name;
        todo.IsComplete = todoUpdateRequest.IsComplete;
        await _dbContext.SaveChangesAsync();
        return new TodoItemResponse(todo);
    }

    public async Task<bool> DeleteTodoAsync(int id)
    {
        if (await _dbContext.Todos.FindAsync(id) is not { } todo) return false;
        _dbContext.Todos.Remove(todo);
        await _dbContext.SaveChangesAsync();
        return true;
    }
}