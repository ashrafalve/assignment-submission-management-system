using Microsoft.EntityFrameworkCore;
using AssignmentManagement.Api.Domain.Entities;
using AssignmentManagement.Api.Domain.Interfaces;
using AssignmentManagement.Api.Infrastructure.Persistence;

namespace AssignmentManagement.Api.Infrastructure.Repositories;

public class TeacherSubjectRepository : GenericRepository<TeacherSubject>, ITeacherSubjectRepository
{
    public TeacherSubjectRepository(ApplicationDbContext context) : base(context) { }

    public async Task<bool> AssignmentExistsAsync(Guid teacherId, Guid subjectId, Guid classId,
        CancellationToken cancellationToken = default)
        => await _dbSet.AnyAsync(ts =>
            ts.TeacherId == teacherId &&
            ts.SubjectId == subjectId &&
            ts.ClassId   == classId, cancellationToken);

    public async Task<IEnumerable<TeacherSubject>> GetByTeacherAsync(Guid teacherId,
        CancellationToken cancellationToken = default)
        => await _dbSet
            .Include(ts => ts.Teacher)
            .Include(ts => ts.Subject)
            .Include(ts => ts.Class)
            .Where(ts => ts.TeacherId == teacherId)
            .OrderBy(ts => ts.Subject.Name)
            .ToListAsync(cancellationToken);

    public async Task<IEnumerable<TeacherSubject>> GetByClassAsync(Guid classId,
        CancellationToken cancellationToken = default)
        => await _dbSet
            .Include(ts => ts.Teacher)
            .Include(ts => ts.Subject)
            .Include(ts => ts.Class)
            .Where(ts => ts.ClassId == classId)
            .OrderBy(ts => ts.Subject.Name)
            .ToListAsync(cancellationToken);

    public async Task<IEnumerable<TeacherSubject>> GetBySubjectAsync(Guid subjectId,
        CancellationToken cancellationToken = default)
        => await _dbSet
            .Include(ts => ts.Teacher)
            .Include(ts => ts.Subject)
            .Include(ts => ts.Class)
            .Where(ts => ts.SubjectId == subjectId)
            .OrderBy(ts => ts.Class.Name)
            .ToListAsync(cancellationToken);

    public async Task<TeacherSubject?> GetDetailedAsync(Guid id,
        CancellationToken cancellationToken = default)
        => await _dbSet
            .Include(ts => ts.Teacher)
            .Include(ts => ts.Subject)
            .Include(ts => ts.Class)
            .FirstOrDefaultAsync(ts => ts.Id == id, cancellationToken);

    public override async Task<IEnumerable<TeacherSubject>> GetAllAsync(
        CancellationToken cancellationToken = default)
        => await _dbSet
            .Include(ts => ts.Teacher)
            .Include(ts => ts.Subject)
            .Include(ts => ts.Class)
            .OrderBy(ts => ts.Class.Name)
            .ThenBy(ts => ts.Subject.Name)
            .ToListAsync(cancellationToken);
}
