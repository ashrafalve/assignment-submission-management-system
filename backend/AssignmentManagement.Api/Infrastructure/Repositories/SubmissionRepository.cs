using Microsoft.EntityFrameworkCore;
using AssignmentManagement.Api.Domain.Entities;
using AssignmentManagement.Api.Domain.Interfaces;
using AssignmentManagement.Api.Infrastructure.Persistence;
using AssignmentManagement.Api.Shared;

namespace AssignmentManagement.Api.Infrastructure.Repositories;

public class SubmissionRepository : GenericRepository<Submission>, ISubmissionRepository
{
    public SubmissionRepository(ApplicationDbContext context) : base(context) { }

    public async Task<Submission?> GetByAssignmentAndStudentAsync(Guid assignmentId, Guid studentId, CancellationToken cancellationToken = default)
        => await _dbSet
            .Include(s => s.Assignment)
            .Include(s => s.Student)
            .FirstOrDefaultAsync(s => s.AssignmentId == assignmentId && s.StudentId == studentId, cancellationToken);

    public async Task<Submission?> GetByIdDetailedAsync(Guid id, CancellationToken cancellationToken = default)
        => await _dbSet
            .Include(s => s.Assignment)
            .Include(s => s.Student)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

    public async Task<IEnumerable<Submission>> GetStudentSubmissionsAsync(Guid studentId, CancellationToken cancellationToken = default)
        => await _dbSet
            .AsNoTracking()
            .Include(s => s.Assignment)
            .Include(s => s.Student)
            .Where(s => s.StudentId == studentId)
            .OrderByDescending(s => s.SubmittedAt)
            .ToListAsync(cancellationToken);

    public async Task<PagedResponse<Submission>> GetSubmissionsForAssignmentAsync(Guid assignmentId, PaginationParams pagination, CancellationToken cancellationToken = default)
    {
        var query = _dbSet.AsNoTracking()
            .Include(s => s.Assignment)
            .Include(s => s.Student)
            .Where(s => s.AssignmentId == assignmentId);

        if (!string.IsNullOrWhiteSpace(pagination.SearchTerm))
        {
            var term = pagination.SearchTerm.ToLower();
            query = query.Where(s => s.Student.FirstName.ToLower().Contains(term) ||
                                     s.Student.LastName.ToLower().Contains(term) ||
                                     s.Student.Email.ToLower().Contains(term));
        }

        query = pagination.SortBy?.ToLower() switch
        {
            "submittedat" => pagination.SortDescending ? query.OrderByDescending(s => s.SubmittedAt) : query.OrderBy(s => s.SubmittedAt),
            "status"      => pagination.SortDescending ? query.OrderByDescending(s => s.Status)      : query.OrderBy(s => s.Status),
            "marksobtained" => pagination.SortDescending ? query.OrderByDescending(s => s.MarksObtained) : query.OrderBy(s => s.MarksObtained),
            _             => query.OrderByDescending(s => s.SubmittedAt)
        };

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((pagination.PageNumber - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .ToListAsync(cancellationToken);

        return PagedResponse<Submission>.Create(items, total, pagination.PageNumber, pagination.PageSize);
    }
}
