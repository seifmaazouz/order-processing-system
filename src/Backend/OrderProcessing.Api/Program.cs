// Infrastructure is referenced here only at the composition root
// to wire concrete implementations to domain interfaces at startup. (No violations of layered architecture)
using OrderProcessing.Api.Middleware;
using OrderProcessing.Application.Interfaces;
using OrderProcessing.Application.Security;
using OrderProcessing.Application.Services;
using OrderProcessing.Infrastructure;
using Scalar.AspNetCore;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;


var builder = WebApplication.CreateBuilder(args);

// PostgreSQL connection string from appsettings.json
var connectionString = builder.Configuration.GetConnectionString("PostgresConnection");

// Register infrastructure services
builder.Services.AddInfrastructure(connectionString!);
builder.Services.AddControllers();

// Add Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//JWT tokens
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = Encoding.UTF8.GetBytes(jwtSettings["Secret"]);
var secret = jwtSettings["Secret"] ?? throw new InvalidOperationException("JWT Secret is not configured.");


builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(secretKey)
    };
});

// Register application services
builder.Services.AddScoped<IUserService, UserServices>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<IJwtService, JwtService>();


// Configure JSON options globally
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
    options.SerializerOptions.NumberHandling = JsonNumberHandling.Strict;
});

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(
            new JsonStringEnumConverter());
        options.JsonSerializerOptions.NumberHandling = JsonNumberHandling.Strict;
    });

builder.Services.AddRouting(options => options.LowercaseUrls = true); // make URLs lowercase

// 1. Generate the OpenAPI document (The "Data")
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        document.Info = new()
        {
            Title = "Order Processing API",
            Version = "v1",
            Description = "Modern API for Book Management"
        };
        return Task.CompletedTask;
    });
    
    options.AddSchemaTransformer((schema, context, cancellationToken) =>
    {
        // Remove 'null' from enum lists for nullable enums to prevent Swagger from sending "null" string
        if (schema.Enum?.Count > 0 && schema.Enum.Any(e => e == null))
        {
            schema.Enum = schema.Enum.Where(e => e != null).ToList();
        }
        return Task.CompletedTask;
    });
});


builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendPolicy", policy =>
    {
        policy
            .WithOrigins("http://localhost:5174") // your frontend URL
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

// Register global exception handling middleware
app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    // .NET 10 Way: Map the OpenAPI JSON endpoint (defaults to /openapi/v1.json)
    app.MapOpenApi();

    // Map the Scalar UI (/scalar/v1)
    app.MapScalarApiReference(options => 
    {
        options.WithTitle("Order System API")
               .WithTheme(ScalarTheme.Moon)
               .WithDefaultHttpClient(ScalarTarget.JavaScript, ScalarClient.HttpClient);
    });

    // Swagger UI (optional) - Remove if team is comfortable with Scalar UI
    app.UseSwaggerUI(options => 
    {
        options.SwaggerEndpoint("/openapi/v1.json", "Order System API");
    });
}

app.MapGet("/", () => "This is the Order Processing API Root!");

app.MapControllers();
app.UseCors("FrontendPolicy");
app.UseAuthentication();
app.UseAuthorization();

app.Run();