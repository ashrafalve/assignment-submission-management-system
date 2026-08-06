using AssignmentManagement.Api.Application.DTOs.Student;
using AssignmentManagement.Api.Application.DTOs.Teacher;
using AssignmentManagement.Api.Shared;

namespace AssignmentManagement.Api.Application.Interfaces;

public interface IStudentAssignmentService
{
    Task<PagedResponse<AssignmentDto>> GetPublishedAssignmentsAsync(Guid studentId, PaginationParams pagination, Guid? subjectId = null, CancellationToken cancellationToken = default);
    Task<StudentAssignmentDetailDto> GetAssignmentDetailsAsync(Guid studentId, Guid assignmentId, CancellationToken cancellationToken = default);
    Task<SubmissionDto> SubmitAssignmentAsync(Guid studentId, SubmitAssignmentDto dto, CancellationToken cancellationToken = default);
    Task<SubmissionDto> UpdateSubmissionAsync(Guid studentId, Guid submissionId, UpdateSubmissionDto dto, CancellationToken cancellationToken = default);
    Task<IEnumerable<SubmissionDto>> GetMySubmissionsAsync(Guid studentId, CancellationToken cancellationToken = default);
}
