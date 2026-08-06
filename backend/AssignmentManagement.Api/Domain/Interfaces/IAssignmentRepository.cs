using AssignmentManagement.Api.Domain.Entities;
using AssignmentManagement.Api.Domain.Enums;
using AssignmentManagement.Api.Shared;

namespace AssignmentManagement.Api.Domain.Interfaces;

public interface IAssignmentRepository : IRepository<Assignment>
{
    Task<Assignment?> GetByIdDetailedAsync(Guid id, CancellationToken cancellationToken = default);

    Task<PagedResponse<Assignment>> GetByTeacherAsync(
        Guid teacherId,
        PaginationParams pagination,
        Guid? classId = null,
        Guid? subjectId = null,
        AssignmentStatus? status = null,
        CancellationToken cancellationToken = default);

    Task<PagedResponse<Assignment>> GetPublishedForStudentAsync(
        Guid classId,
        PaginationParams pagination,
        Guid? subjectId = null,
        CancellationToken cancellationToken = default);
}
