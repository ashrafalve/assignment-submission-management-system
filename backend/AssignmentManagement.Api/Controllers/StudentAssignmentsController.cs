using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AssignmentManagement.Api.Application.DTOs.Student;
using AssignmentManagement.Api.Application.DTOs.Teacher;
using AssignmentManagement.Api.Application.Interfaces;
using AssignmentManagement.Api.Shared;

namespace AssignmentManagement.Api.Controllers;

/// <summary>Student: View published assignments and submit work.</summary>
[ApiController]
[Route("api/student")]
[Authorize(Roles = "Student")]
[Tags("Student - Assignments & Submissions")]
[Produces("application/json")]
public class StudentAssignmentsController : ControllerBase
{
    private readonly IStudentAssignmentService _service;

    public StudentAssignmentsController(IStudentAssignmentService service)
    {
        _service = service;
    }

    /// <summary>Lists published assignments for the student's assigned class.</summary>
    [HttpGet("assignments")]
    [ProducesResponseType(typeof(ApiResponse<PagedResponse<AssignmentDto>>), 200)]
    public async Task<IActionResult> GetPublishedAssignments(
        [FromQuery] PaginationParams pagination,
        [FromQuery] Guid? subjectId,
        CancellationToken ct)
    {
        var result = await _service.GetPublishedAssignmentsAsync(User.GetUserId(), pagination, subjectId, ct);
        return Ok(ApiResponse<PagedResponse<AssignmentDto>>.Ok(result));
    }

    /// <summary>Gets details for a specific published assignment including the student's submission if any.</summary>
    [HttpGet("assignments/{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<StudentAssignmentDetailDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    public async Task<IActionResult> GetAssignmentDetails(Guid id, CancellationToken ct)
    {
        var result = await _service.GetAssignmentDetailsAsync(User.GetUserId(), id, ct);
        return Ok(ApiResponse<StudentAssignmentDetailDto>.Ok(result));
    }

    /// <summary>Submits work for an assignment.</summary>
    [HttpPost("submissions")]
    [ProducesResponseType(typeof(ApiResponse<SubmissionDto>), 201)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    public async Task<IActionResult> SubmitAssignment([FromBody] SubmitAssignmentDto dto, CancellationToken ct)
    {
        var result = await _service.SubmitAssignmentAsync(User.GetUserId(), dto, ct);
        return StatusCode(201, ApiResponse<SubmissionDto>.Ok(result, "Assignment submitted successfully.", 201));
    }

    /// <summary>Updates an existing submission before the deadline.</summary>
    [HttpPut("submissions/{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<SubmissionDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    public async Task<IActionResult> UpdateSubmission(Guid id, [FromBody] UpdateSubmissionDto dto, CancellationToken ct)
    {
        var result = await _service.UpdateSubmissionAsync(User.GetUserId(), id, dto, ct);
        return Ok(ApiResponse<SubmissionDto>.Ok(result, "Submission updated successfully."));
    }

    /// <summary>Lists all submissions made by the student.</summary>
    [HttpGet("submissions")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<SubmissionDto>>), 200)]
    public async Task<IActionResult> GetMySubmissions(CancellationToken ct)
    {
        var result = await _service.GetMySubmissionsAsync(User.GetUserId(), ct);
        return Ok(ApiResponse<IEnumerable<SubmissionDto>>.Ok(result));
    }
}
