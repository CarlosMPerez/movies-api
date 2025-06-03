using Movies.Application.Core;
using Movies.Application.Database;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;
// SERVICES REGISTRATION
// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Register application-layer services into the DI container.
// 
// This approach promotes a clean separation of concerns by isolating service registration logic 
// within the application layer itself, rather than cluttering the startup configuration with details.
// 
// The use of an extension method (AddApplicationLayer) encapsulates which services the application 
// layer needs, allowing the Program.cs file to remain focused on composition rather than implementation details.
//
// Dependency Injection (DI) is used to manage the lifetimes and dependencies of services. In this case,
// AddSingleton is used for IMovieRepository, meaning the same instance will be reused throughout the 
// application's lifetime—ideal for stateless services or those that manage internal state safely.
//
// This setup also improves testability, maintainability, and scalability by decoupling infrastructure concerns 
// from core business logic. Each layer is responsible for registering only its own dependencies.
builder.Services.AddApplicationLayer();
var connectionString = config["Database:ConnectionString"]
    ?? throw new InvalidOperationException("Connection string 'Database:ConnectionString' is missing.");
builder.Services.AddDatabase(connectionString);
builder.Services.AddControllers();

// From this point forward, builder.Services is read-only.
var app = builder.Build();

// MIDDLEWARE CONFIGURATION
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.MapControllers();

var dbInitializer = app.Services.GetRequiredService<DbInitializer>();
await dbInitializer.InitializeDatabaseAsync();

app.Run();
