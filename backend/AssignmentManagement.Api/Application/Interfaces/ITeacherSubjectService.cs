using AssignmentManagement.Api.Application.DTOs.Admin;

namespace AssignmentManagement.Api.Application.Interfaces;

/// <summary>Service for managing teacher-subject-class assignments.</summary>
public interface ITeacherSubjectService
{
    Task<IEnumerable<TeacherSubjectDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<TeacherSubjectDto>> GetByTeacherAsync(Guid teacherId,
        CancellationToken cancellationToken = default);
    Task<IEnumerable<TeacherSubjectDto>> GetByClassAsync(Guid classId,
        CancellationToken cancellationToken = default);
    Task<TeacherSubjectDto> AssignTeacherAsync(AssignTeacherDto dto,
        CancellationToken cancellationToken = default);
    Task RemoveAssignmentAsync(Guid id, CancellationToken cancellationToken = default);
}
