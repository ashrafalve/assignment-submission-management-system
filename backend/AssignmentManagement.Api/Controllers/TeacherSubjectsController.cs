using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AssignmentManagement.Api.Application.DTOs.Admin;
using AssignmentManagement.Api.Application.Interfaces;
using AssignmentManagement.Api.Shared;

namespace AssignmentManagement.Api.Controllers;

/// <summary>Admin: Assign teachers to subjects and classes.</summary>
[ApiController]
[Route("api/admin/teacher-subjects")]
[Authorize(Roles = "Admin")]
[Tags("Admin - Teacher Assignments")]
[Produces("application/json")]
public class TeacherSubjectsController : ControllerBase
{
    private readonly ITeacherSubjectService _service;

    public TeacherSubjectsController(ITeacherSubjectService service) => _service = service;

    /// <summary>Lists all teacher-subject-class assignments.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<TeacherSubjectDto>>), 200)]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var assignments = await _service.GetAllAsync(ct);
        return Ok(ApiResponse<IEnumerable<TeacherSubjectDto>>.Ok(assignments));
    }

    /// <summary>Gets assignments for a specific teacher.</summary>
    [HttpGet("teacher/{teacherId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<TeacherSubjectDto>>), 200)]
    public async Task<IActionResult> GetByTeacher(Guid teacherId, CancellationToken ct)
    {
        var assignments = await _service.GetByTeacherAsync(teacherId, ct);
        return Ok(ApiResponse<IEnumerable<TeacherSubjectDto>>.Ok(assignments));
    }

    /// <summary>Gets assignments for a specific class.</summary>
    [HttpGet("class/{classId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<TeacherSubjectDto>>), 200)]
    public async Task<IActionResult> GetByClass(Guid classId, CancellationToken ct)
    {
        var assignments = await _service.GetByClassAsync(classId, ct);
        return Ok(ApiResponse<IEnumerable<TeacherSubjectDto>>.Ok(assignments));
    }

    /// <summary>Assigns a teacher to a subject and class.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<TeacherSubjectDto>), 201)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    public async Task<IActionResult> AssignTeacher([FromBody] AssignTeacherDto dto, CancellationToken ct)
    {
        var assignment = await _service.AssignTeacherAsync(dto, ct);
        return StatusCode(201, ApiResponse<TeacherSubjectDto>.Ok(assignment, "Teacher assigned successfully.", 201));
    }

    /// <summary>Removes a teacher-subject assignment.</summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<object>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    public async Task<IActionResult> RemoveAssignment(Guid id, CancellationToken ct)
    {
        await _service.RemoveAssignmentAsync(id, ct);
        return Ok(ApiResponse<object?>.Ok(null, "Teacher assignment removed successfully."));
    }
}
