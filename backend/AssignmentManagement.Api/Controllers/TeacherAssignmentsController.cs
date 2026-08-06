using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AssignmentManagement.Api.Application.DTOs.Teacher;
using AssignmentManagement.Api.Application.Interfaces;
using AssignmentManagement.Api.Domain.Enums;
using AssignmentManagement.Api.Shared;

namespace AssignmentManagement.Api.Controllers;

/// <summary>Teacher: Manage assignments (Create, Edit, Delete, Publish, Save Draft).</summary>
[ApiController]
[Route("api/teacher/assignments")]
[Authorize(Roles = "Teacher")]
[Tags("Teacher - Assignments")]
[Produces("application/json")]
public class TeacherAssignmentsController : ControllerBase
{
    private readonly ITeacherAssignmentService _service;

    public TeacherAssignmentsController(ITeacherAssignmentService service)
    {
        _service = service;
    }

    /// <summary>Lists assignments created by the authenticated teacher.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResponse<AssignmentDto>>), 200)]
    public async Task<IActionResult> GetAssignments(
        [FromQuery] PaginationParams pagination,
        [FromQuery] Guid? classId,
        [FromQuery] Guid? subjectId,
        [FromQuery] AssignmentStatus? status,
        CancellationToken ct)
    {
        var result = await _service.GetTeacherAssignmentsAsync(User.GetUserId(), pagination, classId, subjectId, status, ct);
        return Ok(ApiResponse<PagedResponse<AssignmentDto>>.Ok(result));
    }

    /// <summary>Gets a specific assignment by ID.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<AssignmentDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    public async Task<IActionResult> GetAssignment(Guid id, CancellationToken ct)
    {
        var result = await _service.GetAssignmentByIdAsync(User.GetUserId(), id, ct);
        return Ok(ApiResponse<AssignmentDto>.Ok(result));
    }

    /// <summary>Creates a new assignment (as Draft or Published).</summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<AssignmentDto>), 201)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    public async Task<IActionResult> CreateAssignment([FromBody] CreateAssignmentDto dto, CancellationToken ct)
    {
        var result = await _service.CreateAssignmentAsync(User.GetUserId(), dto, ct);
        return StatusCode(201, ApiResponse<AssignmentDto>.Ok(result, "Assignment created successfully.", 201));
    }

    /// <summary>Edits an existing assignment.</summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<AssignmentDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    public async Task<IActionResult> UpdateAssignment(Guid id, [FromBody] UpdateAssignmentDto dto, CancellationToken ct)
    {
        var result = await _service.UpdateAssignmentAsync(User.GetUserId(), id, dto, ct);
        return Ok(ApiResponse<AssignmentDto>.Ok(result, "Assignment updated successfully."));
    }

    /// <summary>Publishes a draft assignment to students.</summary>
    [HttpPatch("{id:guid}/publish")]
    [ProducesResponseType(typeof(ApiResponse<AssignmentDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    public async Task<IActionResult> PublishAssignment(Guid id, CancellationToken ct)
    {
        var result = await _service.PublishAssignmentAsync(User.GetUserId(), id, ct);
        return Ok(ApiResponse<AssignmentDto>.Ok(result, "Assignment published successfully."));
    }

    /// <summary>Saves an assignment as a draft.</summary>
    [HttpPatch("{id:guid}/draft")]
    [ProducesResponseType(typeof(ApiResponse<AssignmentDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    public async Task<IActionResult> SaveDraft(Guid id, CancellationToken ct)
    {
        var result = await _service.SaveDraftAssignmentAsync(User.GetUserId(), id, ct);
        return Ok(ApiResponse<AssignmentDto>.Ok(result, "Assignment saved as draft successfully."));
    }

    /// <summary>Deletes an assignment.</summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<object>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    public async Task<IActionResult> DeleteAssignment(Guid id, CancellationToken ct)
    {
        await _service.DeleteAssignmentAsync(User.GetUserId(), id, ct);
        return Ok(ApiResponse<object?>.Ok(null, "Assignment deleted successfully."));
    }
}
