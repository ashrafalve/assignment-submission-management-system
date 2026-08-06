using Microsoft.EntityFrameworkCore;
using AssignmentManagement.Api.Domain.Entities;
using AssignmentManagement.Api.Domain.Interfaces;
using AssignmentManagement.Api.Infrastructure.Persistence;

namespace AssignmentManagement.Api.Infrastructure.Repositories;

public class SubjectRepository : GenericRepository<Subject>, ISubjectRepository
{
    public SubjectRepository(ApplicationDbContext context) : base(context) { }

    public async Task<Subject?> GetByCodeAsync(string code,
        CancellationToken cancellationToken = default)
        => await _dbSet.FirstOrDefaultAsync(
            s => s.Code == code.ToUpperInvariant(), cancellationToken);

    public async Task<bool> CodeExistsAsync(string code, Guid? excludeId = null,
        CancellationToken cancellationToken = default)
        => await _dbSet.AnyAsync(s =>
            s.Code == code.ToUpperInvariant() &&
            (excludeId == null || s.Id != excludeId), cancellationToken);

    public async Task<IEnumerable<Subject>> GetActiveAsync(CancellationToken cancellationToken = default)
        => await _dbSet.Where(s => s.IsActive).OrderBy(s => s.Name).ToListAsync(cancellationToken);
}
