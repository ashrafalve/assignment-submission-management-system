using AssignmentManagement.Api.Domain.Entities;
using AssignmentManagement.Api.Shared;

namespace AssignmentManagement.Api.Domain.Interfaces;

public interface ISubmissionRepository : IRepository<Submission>
{
    Task<Submission?> GetByAssignmentAndStudentAsync(Guid assignmentId, Guid studentId, CancellationToken cancellationToken = default);
    Task<Submission?> GetByIdDetailedAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Submission>> GetStudentSubmissionsAsync(Guid studentId, CancellationToken cancellationToken = default);
    Task<PagedResponse<Submission>> GetSubmissionsForAssignmentAsync(Guid assignmentId, PaginationParams pagination, CancellationToken cancellationToken = default);
}
