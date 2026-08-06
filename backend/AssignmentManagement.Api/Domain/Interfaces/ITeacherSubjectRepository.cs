using AssignmentManagement.Api.Domain.Entities;

namespace AssignmentManagement.Api.Domain.Interfaces;

/// <summary>TeacherSubject-specific repository operations.</summary>
public interface ITeacherSubjectRepository : IRepository<TeacherSubject>
{
    Task<bool> AssignmentExistsAsync(Guid teacherId, Guid subjectId, Guid classId,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<TeacherSubject>> GetByTeacherAsync(Guid teacherId,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<TeacherSubject>> GetByClassAsync(Guid classId,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<TeacherSubject>> GetBySubjectAsync(Guid subjectId,
        CancellationToken cancellationToken = default);

    Task<TeacherSubject?> GetDetailedAsync(Guid id,
        CancellationToken cancellationToken = default);
}
