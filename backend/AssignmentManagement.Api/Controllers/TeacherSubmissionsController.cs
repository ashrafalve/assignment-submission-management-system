using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AssignmentManagement.Api.Application.DTOs.Student;
using AssignmentManagement.Api.Application.DTOs.Teacher;
using AssignmentManagement.Api.Application.Interfaces;
using AssignmentManagement.Api.Shared;

namespace AssignmentManagement.Api.Controllers;

/// <summary>Teacher: Review, grade, and provide feedback on student submissions.</summary>
[ApiController]
[Route("api/teacher")]
[Authorize(Roles = "Teacher")]
[Tags("Teacher - Submissions Review")]
[Produces("application/json")]
public class TeacherSubmissionsController : ControllerBase
{
    private readonly ITeacherSubmissionService _service;

    public TeacherSubmissionsController(ITeacherSubmissionService service)
    {
        _service = service;
    }

    /// <summary>Lists all submissions for a specific assignment created by the teacher.</summary>
    [HttpGet("assignments/{assignmentId:guid}/submissions")]
    [ProducesResponseType(typeof(ApiResponse<PagedResponse<SubmissionDto>>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    public async Task<IActionResult> GetSubmissionsForAssignment(
        Guid assignmentId,
        [FromQuery] PaginationParams pagination,
        CancellationToken ct)
    {
        var result = await _service.GetSubmissionsForAssignmentAsync(User.GetUserId(), assignmentId, pagination, ct);
        return Ok(ApiResponse<PagedResponse<SubmissionDto>>.Ok(result));
    }

    /// <summary>Gets details of a specific submission.</summary>
    [HttpGet("submissions/{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<SubmissionDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    public async Task<IActionResult> GetSubmission(Guid id, CancellationToken ct)
    {
        var result = await _service.GetSubmissionByIdAsync(User.GetUserId(), id, ct);
        return Ok(ApiResponse<SubmissionDto>.Ok(result));
    }

    /// <summary>Grades a student submission with marks and feedback.</summary>
    [HttpPost("submissions/{id:guid}/grade")]
    [ProducesResponseType(typeof(ApiResponse<SubmissionDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    public async Task<IActionResult> GradeSubmission(
        Guid id,
        [FromBody] GradeSubmissionDto dto,
        CancellationToken ct)
    {
        var result = await _service.GradeSubmissionAsync(User.GetUserId(), id, dto, ct);
        return Ok(ApiResponse<SubmissionDto>.Ok(result, "Submission graded successfully."));
    }

    /// <summary>Changes submission status and optionally updates feedback.</summary>
    [HttpPatch("submissions/{id:guid}/status")]
    [ProducesResponseType(typeof(ApiResponse<SubmissionDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    public async Task<IActionResult> ChangeSubmissionStatus(
        Guid id,
        [FromBody] ChangeSubmissionStatusDto dto,
        CancellationToken ct)
    {
        var result = await _service.ChangeSubmissionStatusAsync(User.GetUserId(), id, dto, ct);
        return Ok(ApiResponse<SubmissionDto>.Ok(result, "Submission status updated successfully."));
    }
}
