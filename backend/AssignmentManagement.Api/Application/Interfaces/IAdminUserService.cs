using AssignmentManagement.Api.Application.DTOs.Admin;
using AssignmentManagement.Api.Application.DTOs.Auth;
using AssignmentManagement.Api.Shared;

namespace AssignmentManagement.Api.Application.Interfaces;

/// <summary>Admin service for full user management.</summary>
public interface IAdminUserService
{
    Task<PagedResponse<UserListItemDto>> GetUsersAsync(PaginationParams pagination,
        string? role = null, CancellationToken cancellationToken = default);
    Task<UserDto> GetUserByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<UserDto> CreateUserAsync(CreateUserDto dto, CancellationToken cancellationToken = default);
    Task<UserDto> UpdateUserAsync(Guid id, UpdateUserDto dto, CancellationToken cancellationToken = default);
    Task ChangePasswordAsync(Guid id, ChangePasswordDto dto, CancellationToken cancellationToken = default);
    Task DeleteUserAsync(Guid id, CancellationToken cancellationToken = default);
}
