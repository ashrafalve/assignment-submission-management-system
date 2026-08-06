using Microsoft.EntityFrameworkCore;
using AssignmentManagement.Api.Domain.Entities;
using AssignmentManagement.Api.Domain.Interfaces;
using AssignmentManagement.Api.Infrastructure.Persistence;

namespace AssignmentManagement.Api.Infrastructure.Repositories;

public class ClassRepository : GenericRepository<SchoolClass>, IClassRepository
{
    public ClassRepository(ApplicationDbContext context) : base(context) { }

    public async Task<bool> NameExistsAsync(string name, string academicYear, Guid? excludeId = null,
        CancellationToken cancellationToken = default)
        => await _dbSet.AnyAsync(c =>
            c.Name == name && c.AcademicYear == academicYear &&
            (excludeId == null || c.Id != excludeId), cancellationToken);

    public async Task<IEnumerable<SchoolClass>> GetActiveAsync(CancellationToken cancellationToken = default)
        => await _dbSet.Where(c => c.IsActive).OrderBy(c => c.Name).ToListAsync(cancellationToken);
}
