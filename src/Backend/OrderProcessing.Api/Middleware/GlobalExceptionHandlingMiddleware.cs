using System.Net;
using System.Text.Json;
using OrderProcessing.Api.Models;
using OrderProcessing.Application.Exceptions;
using OrderProcessing.Domain.Exceptions;

namespace OrderProcessing.Api.Middleware;

public class GlobalExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;

    public GlobalExceptionHandlingMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, message) = exception switch
        {
            UnauthorizedAccessException ex => (HttpStatusCode.Unauthorized, ex.Message),
            NotFoundException ex => (HttpStatusCode.NotFound, ex.Message),
            InsufficientStockException ex => (HttpStatusCode.BadRequest, ex.Message),
            DuplicateResourceException ex => (HttpStatusCode.Conflict, ex.Message),
            BusinessRuleViolationException ex => (HttpStatusCode.BadRequest, ex.Message),
            ArgumentException ex => (HttpStatusCode.BadRequest, ex.Message),
            InvalidOperationException ex when ex.Message.Contains("libgssapi") || ex.Message.Contains("PostgreSQL native libraries") =>
                (HttpStatusCode.InternalServerError, ex.Message),
            InvalidOperationException ex => (HttpStatusCode.BadRequest, ex.Message),
            DataConstraintException ex => (HttpStatusCode.Conflict, ex.Message),
            DllNotFoundException ex when ex.Message.Contains("libgssapi") =>
                (HttpStatusCode.InternalServerError, "PostgreSQL native libraries are missing. Please install PostgreSQL client libraries."),
            _ => (HttpStatusCode.InternalServerError, "An unexpected error occurred.")
        };

        LogException(exception, statusCode, message);

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        // Special-case insufficient stock to return structured details (single or multiple items)
        if (exception is InsufficientStockException ise)
        {
            // Build a typed response DTO for clarity and testability
            var dtoItems = (ise.Items ?? new[] { new InsufficientStockException.InsufficientItem(ise.ISBN ?? string.Empty, ise.Available ?? 0, ise.Title) })
                .Select(i => new InsufficientStockItemDto(i.ISBN, i.Title, i.Available));

            var body = new InsufficientStockResponse("Insufficient stock", dtoItems);
            return context.Response.WriteAsync(JsonSerializer.Serialize(body));
        }

        var response = new ErrorResponse(message, (int)statusCode);
        return context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }

    private void LogException(Exception exception, HttpStatusCode statusCode, string message)
    {
        if (statusCode == HttpStatusCode.InternalServerError)
        {
            // For internal server errors include exception and stack trace
            _logger.LogError(exception, "Unhandled exception: {Message}", message);
        }
        else
        {
            // For expected/non-critical errors (e.g. domain validations) log only type and message
            _logger.LogWarning("{Type}: {Message}", exception.GetType().Name, message);
        }
    }
}
