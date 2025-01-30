using AnotherTodoApi;
using AnotherTodoApi.Validators;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Serilog;
using ILogger = Serilog.ILogger;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

var logger = Log.Logger;
logger.Information("Starting AnotherTodoApi");

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<TodoDbContext>(opt => opt.UseInMemoryDatabase("TodoList"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddSingleton<ILogger>(logger);

builder.Services.AddScoped<IValidator<TodoItemDto>, TodoItemDtoValidator>();

var app = builder.Build();

RouteGroupBuilder todoItems = app.MapGroup("/todoitems");

todoItems.MapGet("/", GetAllTodos);
todoItems.MapGet("/complete", GetCompleteTodos);
todoItems.MapGet("/{id}", GetTodo);
todoItems.MapPost("/", CreateTodo);
todoItems.MapPut("/{id}", UpdateTodo);
todoItems.MapDelete("/{id}", DeleteTodo);

app.Run();

static async Task<IResult> GetAllTodos(TodoDbContext db, ILogger logger)
{
    logger.Information("GetAllTodos endpoint called");

    try
    {
        var todos = await db.Todos.Select(x => new TodoItemDto(x)).ToArrayAsync();
        return TypedResults.Ok(todos);
    }
    catch (Exception e)
    {
        logger.Error(e, "GetAllTodos api endpoint errored");
        return TypedResults.Problem("An error occurred while retrieving all todos.");
    }
}

static async Task<IResult> GetCompleteTodos(TodoDbContext db, ILogger logger)
{
    logger.Information("GetAllTodos endpoint called");

    try
    {
        var todos = await db.Todos.Where(t => t.IsComplete).Select(x => new TodoItemDto(x)).ToListAsync();
        return TypedResults.Ok(todos);
    }
    catch (Exception e)
    {
        logger.Error(e, "GetCompleteTodos api endpoint errored");
        return TypedResults.Problem("An error occurred while retrieving all complete todos.");
    }
}

static async Task<IResult> GetTodo(int id, TodoDbContext db, ILogger logger)
{
    var apiLogger = logger.ForContext("ID", id);
    apiLogger.Information("GetTodo api endpoint called");

    try
    {
        var todo = await db.Todos.FindAsync(id);
        if (todo is null)
        {
            apiLogger.Error("Todo not found in database");
            return TypedResults.NotFound();
        }

        apiLogger = logger.ForContext("TodoItem", todo);
        apiLogger.Information("Todo found in database");

        var todoItem = new TodoItemDto(todo);
        return TypedResults.Ok(todoItem);
    }
    catch (Exception e)
    {
        apiLogger.Error(e, "GetTodo api endpoint errored");
        return TypedResults.Problem($"An error occurred whilst requesting the Todo item for ID {id}.");
    }
}

static async Task<IResult> CreateTodo(IValidator<TodoItemDto> validator, TodoItemDto todoItemDto, TodoDbContext db,
    ILogger logger)
{
    var apiLogger = logger.ForContext("Payload", todoItemDto);
    apiLogger.Information($"CreateTodo endpoint called with payload: {todoItemDto.Name}");

    var results = await validator.ValidateAsync(todoItemDto);

    if (results is null)
    {
        apiLogger.Error("Unexpected null validation result for {Payload}", todoItemDto);
        return TypedResults.Problem("Validation failed due to an unexpected error.");
    }

    if (!results.IsValid)
    {
        apiLogger.Warning("CreateTodo validation failed: {@ValidationErrors}", results.Errors);
        return TypedResults.ValidationProblem(results.ToDictionary());
    }

    try
    {
        var todoItem = new Todo
        {
            Name = todoItemDto.Name,
            IsComplete = todoItemDto.IsComplete
        };

        db.Todos.Add(todoItem);
        await db.SaveChangesAsync();

        apiLogger.Information($"Todo created with ID: {todoItem.Id}");

        todoItemDto = new TodoItemDto(todoItem);
        return TypedResults.Created($"/todoitems/{todoItem.Id}", todoItemDto);
    }
    catch (Exception e)
    {
        apiLogger.Error(e, "CreateTodo api endpoint errored");
        return TypedResults.Problem("An error occurred whilst creating a todo");
    }
}

static async Task<IResult> UpdateTodo(int id, TodoItemDto todoItemDto, TodoDbContext db, ILogger logger)
{
    var apiLogger = logger.ForContext("ID", id);
    apiLogger.Information($"UpdateTodo endpoint called with ID: {id}");

    try
    {
        var todo = await db.Todos.FindAsync(id);
        if (todo is null)
        {
            apiLogger.Information($"Todo with ID {id} not found ");
            return TypedResults.NotFound();
        }

        todo.Name = todoItemDto.Name;
        todo.IsComplete = todoItemDto.IsComplete;
        await db.SaveChangesAsync();

        apiLogger.Warning($"Todo with ID {id} updated");
        return TypedResults.NoContent();
    }
    catch (Exception e)
    {
        apiLogger.Error(e, "UpdateTodo api endpoint errored");
        return TypedResults.Problem("An error occurred whilst updating a todo");
    }
}

static async Task<IResult> DeleteTodo(int id, TodoDbContext db, ILogger logger)
{
    var apiLogger = logger.ForContext("ID", id);
    apiLogger.Information($"DeleteTodo endpoint called with ID: {id}");

    try
    {
        if (await db.Todos.FindAsync(id) is Todo todo)
        {
            db.Todos.Remove(todo);
            await db.SaveChangesAsync();

            apiLogger.Information($"Todo with ID {id} deleted");
            return TypedResults.NoContent();
        }

        apiLogger.Warning($"Todo with ID {id} not found.", id);
        return TypedResults.NotFound();
    }
    catch (Exception e)
    {
        apiLogger.Error(e, "DeleteTodo api endpoint errored");
        return TypedResults.Problem("An error occurred whilst deleting a todo");
    }
}