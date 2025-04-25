using AnotherTodoApi.Api.Validators;
using AnotherTodoApi.Api.Endpoints;
using AnotherTodoApi.Api.Repository;
using Microsoft.EntityFrameworkCore;
using FluentValidation;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Services.AddDbContext<TodoDbContext>(opt =>
    opt.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddValidatorsFromAssembly(typeof(TodoCreateRequestValidator).Assembly);
builder.Services.AddSingleton(Log.Logger);

var app = builder.Build();

app.RegisterTodoItemsEndpoints();

// Auto-apply migrations on startup
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<TodoDbContext>();
    dbContext.Database.Migrate();
}

app.Run();