using System.Net.Http.Json;
using AnotherTodoApi.Api;
using Microsoft.AspNetCore.Mvc.Testing;

namespace AnotherTodoApi.Tests.Integration;

public class TodoItemEndpointsTests
{
    [Fact]
    public async Task GetListOfTodos()
    {
        await using var application = new WebApplicationFactory<Todo>();
        using var client = application.CreateClient();

        await AddTodoItems(client);
        await AddTodoItems(client);

        var response = await client.GetStringAsync("/todoitems");

        // Assert.Equal("Hello World!", response);
    }

    [Fact]
    public async Task GetListOfTodos2()
    {
        await using var application = new WebApplicationFactory<Todo>();
        using var client = application.CreateClient();

        await AddTodoItems(client);
        await AddTodoItems(client);

        var response = await client.GetStringAsync("/todoitems");

        // Assert.Equal("Hello World!", response);
    }

    private async Task AddTodoItems(HttpClient client)
    {
        await client.PostAsJsonAsync("/todoitems", new
        {
            name = "walk dog",
            isComplete = true
        });
    }
}