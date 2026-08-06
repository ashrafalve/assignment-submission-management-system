using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AssignmentManagement.Api.Application.DTOs.Admin;
using AssignmentManagement.Api.Application.Interfaces;
using AssignmentManagement.Api.Shared;

namespace AssignmentManagement.Api.Controllers;

/// <summary>Admin: Full user management (CRUD).</summary>
[ApiController]
[Route("api/admin/users")]
[Authorize(Roles = "Admin")]
[Tags("Admin - Users")]
[Produces("application/json")]
public class AdminUsersController : ControllerBase
{
    private readonly IAdminUserService _service;

    public AdminUsersController(IAdminUserService service) => _service = service;

    /// <summary>Lists all users with pagination, search, and optional role filter.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResponse<UserListItemDto>>), 200)]
    public async Task<IActionResult> GetUsers(
        [FromQuery] PaginationParams pagination,
        [FromQuery] string? role,
        CancellationToken ct)
    {
        var result = await _service.GetUsersAsync(pagination, role, ct);
        return Ok(ApiResponse<PagedResponse<UserListItemDto>>.Ok(result));
    }

    /// <summary>Gets a user by ID.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<object>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    public async Task<IActionResult> GetUser(Guid id, CancellationToken ct)
    {
        var user = await _service.GetUserByIdAsync(id, ct);
        return Ok(ApiResponse<object>.Ok(user));
    }

    /// <summary>Creates a new user with a specified role.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<object>), 201)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserDto dto, CancellationToken ct)
    {
        var user = await _service.CreateUserAsync(dto, ct);
        return StatusCode(201, ApiResponse<object>.Ok(user, "User created successfully.", 201));
    }

    /// <summary>Updates a user's details or role.</summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<object>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UpdateUserDto dto, CancellationToken ct)
    {
        var user = await _service.UpdateUserAsync(id, dto, ct);
        return Ok(ApiResponse<object>.Ok(user, "User updated successfully."));
    }

    /// <summary>Changes a user's password (admin override).</summary>
    [HttpPatch("{id:guid}/password")]
    [ProducesResponseType(typeof(ApiResponse<object>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    public async Task<IActionResult> ChangePassword(Guid id, [FromBody] ChangePasswordDto dto,
        CancellationToken ct)
    {
        await _service.ChangePasswordAsync(id, dto, ct);
        return Ok(ApiResponse<object?>.Ok(null, "Password changed successfully."));
    }

    /// <summary>Soft-deletes a user.</summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<object>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    public async Task<IActionResult> DeleteUser(Guid id, CancellationToken ct)
    {
        await _service.DeleteUserAsync(id, ct);
        return Ok(ApiResponse<object?>.Ok(null, "User deleted successfully."));
    }
}
