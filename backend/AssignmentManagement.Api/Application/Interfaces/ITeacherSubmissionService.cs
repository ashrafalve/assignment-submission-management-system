using AssignmentManagement.Api.Application.DTOs.Student;
using AssignmentManagement.Api.Application.DTOs.Teacher;
using AssignmentManagement.Api.Shared;

namespace AssignmentManagement.Api.Application.Interfaces;

public interface ITeacherSubmissionService
{
    Task<PagedResponse<SubmissionDto>> GetSubmissionsForAssignmentAsync(Guid teacherId, Guid assignmentId, PaginationParams pagination, CancellationToken cancellationToken = default);
    Task<SubmissionDto> GetSubmissionByIdAsync(Guid teacherId, Guid submissionId, CancellationToken cancellationToken = default);
    Task<SubmissionDto> GradeSubmissionAsync(Guid teacherId, Guid submissionId, GradeSubmissionDto dto, CancellationToken cancellationToken = default);
    Task<SubmissionDto> ChangeSubmissionStatusAsync(Guid teacherId, Guid submissionId, ChangeSubmissionStatusDto dto, CancellationToken cancellationToken = default);
}
