using System.Net;
using System.Net.Http.Json;
using AnotherTodoApi.Api;
using AnotherTodoApi.Api.Responses;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit.Abstractions;

namespace AnotherTodoApi.Tests.Integration;

public class TodoItemEndpointsTests : IClassFixture<WebApplicationFactory<Todo>>
{
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly HttpClient _client;

    public TodoItemEndpointsTests(WebApplicationFactory<Todo> application, ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
        _client = application.CreateClient();
    }

    [Fact]
    public async Task ShouldReturnAllTodos()
    {
        await AddTodoItem();
        await AddTodoItem();

        var response = await _client.GetAsync("/todoitems");
        response.EnsureSuccessStatusCode();

        var todos = await response.Content.ReadFromJsonAsync<List<TodoItemResponse>>();

        Assert.NotNull(todos);
        Assert.Equal(2, todos.Count);
    }

    [Fact]
    public async Task ShouldReturnEmptyList_WhenNoTodosExist()
    {
        var response = await _client.GetAsync("/todoitems");
        response.EnsureSuccessStatusCode();

        var todos = await response.Content.ReadFromJsonAsync<List<TodoItemResponse>>();

        Assert.NotNull(todos);
        Assert.Empty(todos);
    }

    [Fact]
    public async Task ShouldReturnCorrectTodo_WhenValidIdProvided()
    {
        await AddTodoItem();
        await AddTodoItem();

        var response = await _client.GetAsync("/todoitems{2}");
        response.EnsureSuccessStatusCode();

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task ShouldReturnNotFound_WhenIdDoesNotExist()
    {
        var response = await _client.GetAsync("/todoitems{10}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private async Task AddTodoItem()
    {
        var response = await _client.PostAsJsonAsync("/todoitems", new
        {
            name = "walk dog",
            isComplete = true
        });

        response.EnsureSuccessStatusCode();
    }
}