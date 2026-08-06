using AssignmentManagement.Api.Domain.Entities;

namespace AssignmentManagement.Api.Domain.Interfaces;

/// <summary>SchoolClass-specific repository operations.</summary>
public interface IClassRepository : IRepository<SchoolClass>
{
    Task<bool> NameExistsAsync(string name, string academicYear, Guid? excludeId = null,
        CancellationToken cancellationToken = default);
    Task<IEnumerable<SchoolClass>> GetActiveAsync(CancellationToken cancellationToken = default);
}
