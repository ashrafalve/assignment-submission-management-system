using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AssignmentManagement.Api.Application.DTOs.Admin;
using AssignmentManagement.Api.Application.Interfaces;
using AssignmentManagement.Api.Shared;

namespace AssignmentManagement.Api.Controllers;

/// <summary>Admin: Manage school classes.</summary>
[ApiController]
[Route("api/admin/classes")]
[Authorize(Roles = "Admin")]
[Tags("Admin - Classes")]
[Produces("application/json")]
public class ClassesController : ControllerBase
{
    private readonly IClassService _service;
    public ClassesController(IClassService service) => _service = service;

    /// <summary>Lists all classes with pagination and search.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResponse<ClassDto>>), 200)]
    public async Task<IActionResult> GetClasses([FromQuery] PaginationParams pagination, CancellationToken ct)
    {
        var result = await _service.GetClassesAsync(pagination, ct);
        return Ok(ApiResponse<PagedResponse<ClassDto>>.Ok(result));
    }

    /// <summary>Gets a class by ID.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<ClassDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    public async Task<IActionResult> GetClass(Guid id, CancellationToken ct)
    {
        var cls = await _service.GetClassByIdAsync(id, ct);
        return Ok(ApiResponse<ClassDto>.Ok(cls));
    }

    /// <summary>Creates a new class.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<ClassDto>), 201)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    public async Task<IActionResult> CreateClass([FromBody] CreateClassDto dto, CancellationToken ct)
    {
        var cls = await _service.CreateClassAsync(dto, ct);
        return StatusCode(201, ApiResponse<ClassDto>.Ok(cls, "Class created successfully.", 201));
    }

    /// <summary>Updates an existing class.</summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<ClassDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    public async Task<IActionResult> UpdateClass(Guid id, [FromBody] UpdateClassDto dto, CancellationToken ct)
    {
        var cls = await _service.UpdateClassAsync(id, dto, ct);
        return Ok(ApiResponse<ClassDto>.Ok(cls, "Class updated successfully."));
    }

    /// <summary>Deletes a class (soft delete).</summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<object>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    public async Task<IActionResult> DeleteClass(Guid id, CancellationToken ct)
    {
        await _service.DeleteClassAsync(id, ct);
        return Ok(ApiResponse<object?>.Ok(null, "Class deleted successfully."));
    }
}
