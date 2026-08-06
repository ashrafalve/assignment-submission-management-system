using System.Net;
using System.Text.Json;
using AssignmentManagement.Api.Shared;

namespace AssignmentManagement.Api.Middleware;

/// <summary>
/// Global exception handling middleware that catches unhandled exceptions
/// and returns a standardized error response.
/// </summary>
public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
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
            _logger.LogError(ex, "Unhandled exception occurred. TraceId: {TraceId}", context.TraceIdentifier);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var (statusCode, message) = exception switch
        {
            ArgumentNullException     => (HttpStatusCode.BadRequest,          "A required argument was null."),
            ArgumentException         => (HttpStatusCode.BadRequest,          exception.Message),
            KeyNotFoundException      => (HttpStatusCode.NotFound,            "The requested resource was not found."),
            UnauthorizedAccessException => (HttpStatusCode.Unauthorized,      "You are not authorized to perform this action."),
            InvalidOperationException => (HttpStatusCode.UnprocessableContent,"Invalid operation: " + exception.Message),
            NotImplementedException   => (HttpStatusCode.NotImplemented,      "This feature is not yet implemented."),
            _                         => (HttpStatusCode.InternalServerError, "An unexpected error occurred. Please try again later.")
        };

        context.Response.StatusCode = (int)statusCode;

        var response = ApiResponse<object>.Fail(
            message,
            statusCode: (int)statusCode,
            traceId: context.TraceIdentifier
        );

        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        await context.Response.WriteAsync(JsonSerializer.Serialize(response, options));
    }
}
