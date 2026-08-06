using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AssignmentManagement.Api.Application.DTOs.Admin;
using AssignmentManagement.Api.Application.Interfaces;
using AssignmentManagement.Api.Shared;

namespace AssignmentManagement.Api.Controllers;

/// <summary>Admin: Manage academic subjects.</summary>
[ApiController]
[Route("api/admin/subjects")]
[Authorize(Roles = "Admin")]
[Tags("Admin - Subjects")]
[Produces("application/json")]
public class SubjectsController : ControllerBase
{
    private readonly ISubjectService _service;

    public SubjectsController(ISubjectService service) => _service = service;

    /// <summary>Lists all subjects with pagination and search.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResponse<SubjectDto>>), 200)]
    public async Task<IActionResult> GetSubjects([FromQuery] PaginationParams pagination, CancellationToken ct)
    {
        var result = await _service.GetSubjectsAsync(pagination, ct);
        return Ok(ApiResponse<PagedResponse<SubjectDto>>.Ok(result));
    }

    /// <summary>Gets a subject by ID.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<SubjectDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    public async Task<IActionResult> GetSubject(Guid id, CancellationToken ct)
    {
        var subject = await _service.GetSubjectByIdAsync(id, ct);
        return Ok(ApiResponse<SubjectDto>.Ok(subject));
    }

    /// <summary>Creates a new subject.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<SubjectDto>), 201)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    public async Task<IActionResult> CreateSubject([FromBody] CreateSubjectDto dto, CancellationToken ct)
    {
        var subject = await _service.CreateSubjectAsync(dto, ct);
        return StatusCode(201, ApiResponse<SubjectDto>.Ok(subject, "Subject created successfully.", 201));
    }

    /// <summary>Updates an existing subject.</summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<SubjectDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    public async Task<IActionResult> UpdateSubject(Guid id, [FromBody] UpdateSubjectDto dto, CancellationToken ct)
    {
        var subject = await _service.UpdateSubjectAsync(id, dto, ct);
        return Ok(ApiResponse<SubjectDto>.Ok(subject, "Subject updated successfully."));
    }

    /// <summary>Deletes a subject (soft delete).</summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<object>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    public async Task<IActionResult> DeleteSubject(Guid id, CancellationToken ct)
    {
        await _service.DeleteSubjectAsync(id, ct);
        return Ok(ApiResponse<object?>.Ok(null, "Subject deleted successfully."));
    }
}
