using AssignmentManagement.Api.Application.DTOs.Teacher;
using AssignmentManagement.Api.Domain.Enums;
using AssignmentManagement.Api.Shared;

namespace AssignmentManagement.Api.Application.Interfaces;

public interface ITeacherAssignmentService
{
    Task<AssignmentDto> CreateAssignmentAsync(Guid teacherId, CreateAssignmentDto dto, CancellationToken cancellationToken = default);
    Task<AssignmentDto> UpdateAssignmentAsync(Guid teacherId, Guid assignmentId, UpdateAssignmentDto dto, CancellationToken cancellationToken = default);
    Task<AssignmentDto> PublishAssignmentAsync(Guid teacherId, Guid assignmentId, CancellationToken cancellationToken = default);
    Task<AssignmentDto> SaveDraftAssignmentAsync(Guid teacherId, Guid assignmentId, CancellationToken cancellationToken = default);
    Task DeleteAssignmentAsync(Guid teacherId, Guid assignmentId, CancellationToken cancellationToken = default);
    Task<PagedResponse<AssignmentDto>> GetTeacherAssignmentsAsync(Guid teacherId, PaginationParams pagination, Guid? classId = null, Guid? subjectId = null, AssignmentStatus? status = null, CancellationToken cancellationToken = default);
    Task<AssignmentDto> GetAssignmentByIdAsync(Guid teacherId, Guid assignmentId, CancellationToken cancellationToken = default);
}
