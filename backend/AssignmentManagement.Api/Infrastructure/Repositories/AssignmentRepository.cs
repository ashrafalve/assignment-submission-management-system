using Microsoft.EntityFrameworkCore;
using AssignmentManagement.Api.Domain.Entities;
using AssignmentManagement.Api.Domain.Enums;
using AssignmentManagement.Api.Domain.Interfaces;
using AssignmentManagement.Api.Infrastructure.Persistence;
using AssignmentManagement.Api.Shared;

namespace AssignmentManagement.Api.Infrastructure.Repositories;

public class AssignmentRepository : GenericRepository<Assignment>, IAssignmentRepository
{
    public AssignmentRepository(ApplicationDbContext context) : base(context) { }

    public async Task<Assignment?> GetByIdDetailedAsync(Guid id, CancellationToken cancellationToken = default)
        => await _dbSet
            .Include(a => a.Teacher)
            .Include(a => a.Subject)
            .Include(a => a.Class)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

    public async Task<PagedResponse<Assignment>> GetByTeacherAsync(
        Guid teacherId,
        PaginationParams pagination,
        Guid? classId = null,
        Guid? subjectId = null,
        AssignmentStatus? status = null,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet.AsNoTracking()
            .Include(a => a.Teacher)
            .Include(a => a.Subject)
            .Include(a => a.Class)
            .Where(a => a.TeacherId == teacherId);

        if (classId.HasValue)
            query = query.Where(a => a.ClassId == classId.Value);

        if (subjectId.HasValue)
            query = query.Where(a => a.SubjectId == subjectId.Value);

        if (status.HasValue)
            query = query.Where(a => a.Status == status.Value);

        if (!string.IsNullOrWhiteSpace(pagination.SearchTerm))
        {
            var term = pagination.SearchTerm.ToLower();
            query = query.Where(a => a.Title.ToLower().Contains(term) || a.Description.ToLower().Contains(term));
        }

        query = pagination.SortBy?.ToLower() switch
        {
            "duedate" => pagination.SortDescending ? query.OrderByDescending(a => a.DueDate) : query.OrderBy(a => a.DueDate),
            "title"   => pagination.SortDescending ? query.OrderByDescending(a => a.Title)   : query.OrderBy(a => a.Title),
            "status"  => pagination.SortDescending ? query.OrderByDescending(a => a.Status)  : query.OrderBy(a => a.Status),
            _         => pagination.SortDescending ? query.OrderByDescending(a => a.CreatedAt) : query.OrderBy(a => a.CreatedAt)
        };

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((pagination.PageNumber - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .ToListAsync(cancellationToken);

        return PagedResponse<Assignment>.Create(items, total, pagination.PageNumber, pagination.PageSize);
    }

    public async Task<PagedResponse<Assignment>> GetPublishedForStudentAsync(
        Guid classId,
        PaginationParams pagination,
        Guid? subjectId = null,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet.AsNoTracking()
            .Include(a => a.Teacher)
            .Include(a => a.Subject)
            .Include(a => a.Class)
            .Where(a => a.ClassId == classId && a.Status == AssignmentStatus.Published);

        if (subjectId.HasValue)
            query = query.Where(a => a.SubjectId == subjectId.Value);

        if (!string.IsNullOrWhiteSpace(pagination.SearchTerm))
        {
            var term = pagination.SearchTerm.ToLower();
            query = query.Where(a => a.Title.ToLower().Contains(term) || a.Description.ToLower().Contains(term));
        }

        query = pagination.SortBy?.ToLower() switch
        {
            "duedate" => pagination.SortDescending ? query.OrderByDescending(a => a.DueDate) : query.OrderBy(a => a.DueDate),
            "title"   => pagination.SortDescending ? query.OrderByDescending(a => a.Title)   : query.OrderBy(a => a.Title),
            _         => query.OrderBy(a => a.DueDate)
        };

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((pagination.PageNumber - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .ToListAsync(cancellationToken);

        return PagedResponse<Assignment>.Create(items, total, pagination.PageNumber, pagination.PageSize);
    }
}
