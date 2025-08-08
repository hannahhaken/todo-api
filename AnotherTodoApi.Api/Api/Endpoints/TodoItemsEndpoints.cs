using AnotherTodoApi.Api.Api.Requests;
using AnotherTodoApi.Api.Repository;
using AnotherTodoApi.Api.Services;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using ILogger = Serilog.ILogger;

namespace AnotherTodoApi.Api.Api.Endpoints;

public static class TodoItemsEndpoints
{
    public static void RegisterTodoItemsEndpoints(this WebApplication app)
    {
        RouteGroupBuilder todoItems = app.MapGroup("/todoitems");

        todoItems.MapGet("/", GetAllTodos);
        todoItems.MapGet("/complete", GetCompleteTodos);
        todoItems.MapGet("/{id:int}", GetTodo);
        todoItems.MapPost("/", CreateTodo);
        todoItems.MapPut("/{id:int}", UpdateTodo);
        todoItems.MapDelete("/{id:int}", DeleteTodo);
        return;

        static async Task<IResult> GetAllTodos([FromServices] TodoService todoService, [FromServices] ILogger logger)
        {
            try
            {
                var todos = await todoService.GetAllTodosAsync();
                return TypedResults.Ok(todos);
            }
            catch (Exception e)
            {
                logger.Error(e, "GetAllTodos api endpoint errored");
                return TypedResults.Problem("An error occurred while retrieving all todos.");
            }
        }

        static async Task<IResult> GetCompleteTodos([FromServices] TodoService todoService,
            [FromServices] ILogger logger)
        {
            try
            {
                var todos = await todoService.GetCompleteTodosAsync();

                return TypedResults.Ok(todos);
            }
            catch (Exception e)
            {
                logger.Error(e, "GetCompleteTodos api endpoint errored");
                return TypedResults.Problem("An error occurred while retrieving all complete todos.");
            }
        }

        static async Task<IResult> GetTodo(int id, [FromServices] TodoService todoService,
            [FromServices] ILogger logger)
        {
            var apiLogger = logger.ForContext("ID", id);

            try
            {
                var todo = await todoService.GetTodoByIdAsync(id);
                if (todo is null)
                {
                    apiLogger.Error("Todo not found in database");
                    return TypedResults.NotFound();
                }

                apiLogger = apiLogger.ForContext("TodoItem", todo);
                apiLogger.Information("Todo found in database");

                return TypedResults.Ok(todo);
            }
            catch (Exception e)
            {
                apiLogger.Error(e, "GetTodo api endpoint errored");
                return TypedResults.Problem($"An error occurred whilst requesting the Todo item for ID {id}.");
            }
        }

        static async Task<IResult> CreateTodo(IValidator<TodoCreateRequest> validator,
            TodoCreateRequest todoCreateRequest,
            [FromServices] TodoService todoService,
            ILogger logger)
        {
            var apiLogger = logger.ForContext("Payload", todoCreateRequest);
            apiLogger.Information("CreateTodo endpoint called with payload: {Name}", todoCreateRequest.Name);

            var results = await validator.ValidateAsync(todoCreateRequest);

            if (results is null)
            {
                apiLogger.Error("Unexpected null validation result for {Payload}", todoCreateRequest);
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
                    Name = todoCreateRequest.Name,
                    IsComplete = todoCreateRequest.IsComplete
                };

                var createdTodo = await todoService.CreateTodoAsync(todoItem);
                apiLogger.Information("Todo created with ID: {TodoItemId}", createdTodo.Id);

                return TypedResults.Created($"/todoitems/{createdTodo.Id}", createdTodo);
            }
            catch (Exception e)
            {
                apiLogger.Error(e, "CreateTodo api endpoint errored");
                return TypedResults.Problem("An error occurred whilst creating a todo");
            }
        }

        static async Task<IResult> UpdateTodo(int id, TodoUpdateRequest todoUpdateRequest,
            [FromServices] TodoService todoService,
            ILogger logger)
        {
            var apiLogger = logger.ForContext("ID", id);

            try
            {
                var existingTodo = await todoService.UpdateTodoAsync(id, todoUpdateRequest);
                if (existingTodo is null)
                {
                    apiLogger.Information("Todo with ID {Id} not found ", id);
                    return TypedResults.NotFound();
                }

                apiLogger.Warning("Todo with ID {Id} updated", id);
                return TypedResults.NoContent();
            }
            catch (Exception e)
            {
                apiLogger.Error(e, "UpdateTodo api endpoint errored");
                return TypedResults.Problem("An error occurred whilst updating a todo");
            }
        }

        static async Task<IResult> DeleteTodo(int id, [FromServices] TodoService todoService,
            ILogger logger)
        {
            var apiLogger = logger.ForContext("ID", id);

            try
            {
                var deletedTodo = await todoService.DeleteTodoAsync(id);
                if (!deletedTodo)
                {
                    apiLogger.Warning("Todo with ID {Id} not found.", id, id);
                    return TypedResults.NotFound();
                }
            }
            catch (Exception e)
            {
                apiLogger.Error(e, "DeleteTodo api endpoint errored");
                return TypedResults.Problem("An error occurred whilst deleting a todo");
            }

            apiLogger.Information("Todo with ID {Id} deleted", id);
            return TypedResults.NoContent();
        }
    }
}