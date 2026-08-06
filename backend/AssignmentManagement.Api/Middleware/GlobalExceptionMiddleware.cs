using System.Net;
using System.Text.Json;
using AssignmentManagement.Api.Domain.Exceptions;
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
            NotFoundException ex          => (HttpStatusCode.NotFound,            ex.Message),
            KeyNotFoundException ex       => (HttpStatusCode.NotFound,            ex.Message),
            ForbiddenException ex         => (HttpStatusCode.Forbidden,           ex.Message),
            UnauthorizedAccessException ex => (HttpStatusCode.Unauthorized,       ex.Message),
            BadRequestException ex        => (HttpStatusCode.BadRequest,          ex.Message),
            ArgumentNullException         => (HttpStatusCode.BadRequest,          "A required argument was null."),
            ArgumentException ex          => (HttpStatusCode.BadRequest,          ex.Message),
            ConflictException ex          => (HttpStatusCode.Conflict,            ex.Message),
            BusinessRuleException ex      => (HttpStatusCode.UnprocessableContent,ex.Message),
            InvalidOperationException ex  => (HttpStatusCode.BadRequest,          ex.Message),
            NotImplementedException       => (HttpStatusCode.NotImplemented,      "This feature is not yet implemented."),
            _                             => (HttpStatusCode.InternalServerError, "An unexpected error occurred. Please try again later.")
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
