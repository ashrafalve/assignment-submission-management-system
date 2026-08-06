namespace AssignmentManagement.Api.Shared;

/// <summary>
/// Standardized API response wrapper for all endpoints.
/// </summary>
/// <typeparam name="T">The type of the response data payload.</typeparam>
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
    public int StatusCode { get; set; }
    public string? TraceId { get; set; }
    public IEnumerable<string>? Errors { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    public static ApiResponse<T> Ok(T data, string message = "Success", int statusCode = 200)
        => new() { Success = true, Data = data, Message = message, StatusCode = statusCode };

    public static ApiResponse<T> Fail(string message, IEnumerable<string>? errors = null,
        int statusCode = 500, string? traceId = null)
        => new() { Success = false, Message = message, Errors = errors, StatusCode = statusCode, TraceId = traceId };
}
