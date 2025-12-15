// Infrastructure is referenced here only at the composition root
// to wire concrete implementations to domain interfaces at startup. (No violations of layered architecture)
using OrderProcessing.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// PostgreSQL connection string from appsettings.json
var connectionString = builder.Configuration.GetConnectionString("PostgresConnection");

// Register infrastructure services
builder.Services.AddInfrastructure(connectionString!);

var app = builder.Build();

app.MapGet("/", () => "This is the Order Processing API Root!");

app.Run();