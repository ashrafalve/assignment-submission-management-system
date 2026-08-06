using AssignmentManagement.Api.Application.DTOs.Admin;
using AssignmentManagement.Api.Shared;

namespace AssignmentManagement.Api.Application.Interfaces;

/// <summary>Service for managing school classes.</summary>
public interface IClassService
{
    Task<PagedResponse<ClassDto>> GetClassesAsync(PaginationParams pagination,
        CancellationToken cancellationToken = default);
    Task<ClassDto> GetClassByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ClassDto> CreateClassAsync(CreateClassDto dto, CancellationToken cancellationToken = default);
    Task<ClassDto> UpdateClassAsync(Guid id, UpdateClassDto dto, CancellationToken cancellationToken = default);
    Task DeleteClassAsync(Guid id, CancellationToken cancellationToken = default);
}
