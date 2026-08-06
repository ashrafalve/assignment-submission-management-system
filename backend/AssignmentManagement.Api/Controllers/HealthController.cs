using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using HealthChecks.UI.Client;
using AssignmentManagement.Api.Shared;

namespace AssignmentManagement.Api.Controllers;

/// <summary>
/// Health check endpoint for monitoring application and dependency status.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Tags("Health")]
public class HealthController : ControllerBase
{
    private readonly HealthCheckService _healthCheckService;

    public HealthController(HealthCheckService healthCheckService)
    {
        _healthCheckService = healthCheckService;
    }

    /// <summary>
    /// Returns the overall health status of the application and its dependencies.
    /// </summary>
    /// <returns>Health report with individual component statuses.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> GetHealth(CancellationToken cancellationToken)
    {
        var report = await _healthCheckService.CheckHealthAsync(cancellationToken);

        var result = new
        {
            Status = report.Status.ToString(),
            Duration = report.TotalDuration.TotalMilliseconds + "ms",
            Checks = report.Entries.Select(e => new
            {
                Name = e.Key,
                Status = e.Value.Status.ToString(),
                Duration = e.Value.Duration.TotalMilliseconds + "ms",
                Description = e.Value.Description
            })
        };

        return report.Status == HealthStatus.Healthy
            ? Ok(ApiResponse<object>.Ok(result, "All systems operational"))
            : StatusCode(StatusCodes.Status503ServiceUnavailable,
                ApiResponse<object>.Fail("One or more health checks failed", statusCode: 503));
    }
}
