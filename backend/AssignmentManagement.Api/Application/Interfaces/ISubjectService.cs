using AssignmentManagement.Api.Application.DTOs.Admin;
using AssignmentManagement.Api.Shared;

namespace AssignmentManagement.Api.Application.Interfaces;

/// <summary>Service for managing academic subjects.</summary>
public interface ISubjectService
{
    Task<PagedResponse<SubjectDto>> GetSubjectsAsync(PaginationParams pagination,
        CancellationToken cancellationToken = default);
    Task<SubjectDto> GetSubjectByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<SubjectDto> CreateSubjectAsync(CreateSubjectDto dto, CancellationToken cancellationToken = default);
    Task<SubjectDto> UpdateSubjectAsync(Guid id, UpdateSubjectDto dto, CancellationToken cancellationToken = default);
    Task DeleteSubjectAsync(Guid id, CancellationToken cancellationToken = default);
}
