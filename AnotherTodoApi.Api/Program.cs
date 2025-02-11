using AnotherTodoApi.Api;
using AnotherTodoApi.Api.Validators;
using AnotherTodoApi.Api.Endpoints;
using Microsoft.EntityFrameworkCore;
using FluentValidation;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Services.AddDbContext<TodoDbContext>(opt => opt.UseInMemoryDatabase("TodoList"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddValidatorsFromAssembly(typeof(TodoItemDtoValidator).Assembly);
builder.Services.AddSingleton(Log.Logger);

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.RegisterTodoItemsEndpoints();

app.Run();