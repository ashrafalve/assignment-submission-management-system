using AssignmentManagement.Api.Domain.Entities;

namespace AssignmentManagement.Api.Domain.Interfaces;

/// <summary>Subject-specific repository operations.</summary>
public interface ISubjectRepository : IRepository<Subject>
{
    Task<Subject?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<bool> CodeExistsAsync(string code, Guid? excludeId = null,
        CancellationToken cancellationToken = default);
    Task<IEnumerable<Subject>> GetActiveAsync(CancellationToken cancellationToken = default);
}
