using AutoMapper;
using Microsoft.EntityFrameworkCore;
using AssignmentManagement.Api.Application.DTOs.Admin;
using AssignmentManagement.Api.Application.Interfaces;
using AssignmentManagement.Api.Domain.Entities;
using AssignmentManagement.Api.Domain.Interfaces;
using AssignmentManagement.Api.Infrastructure.Persistence;
using AssignmentManagement.Api.Shared;

namespace AssignmentManagement.Api.Application.Services;

public class ClassService : IClassService
{
    private readonly IClassRepository _classRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ApplicationDbContext _context;

    public ClassService(IClassRepository classRepo, IUnitOfWork unitOfWork,
        IMapper mapper, ApplicationDbContext context)
    {
        _classRepo   = classRepo;
        _unitOfWork  = unitOfWork;
        _mapper      = mapper;
        _context     = context;
    }

    public async Task<PagedResponse<ClassDto>> GetClassesAsync(PaginationParams pagination,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Classes.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(pagination.SearchTerm))
        {
            var term = pagination.SearchTerm.ToLower();
            query = query.Where(c => c.Name.ToLower().Contains(term) ||
                                     c.AcademicYear.Contains(term));
        }

        query = pagination.SortBy?.ToLower() switch
        {
            "academicyear" => pagination.SortDescending
                ? query.OrderByDescending(c => c.AcademicYear)
                : query.OrderBy(c => c.AcademicYear),
            _ => pagination.SortDescending
                ? query.OrderByDescending(c => c.Name)
                : query.OrderBy(c => c.Name)
        };

        var total  = await query.CountAsync(cancellationToken);
        var items  = await query
            .Skip((pagination.PageNumber - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .ToListAsync(cancellationToken);

        return PagedResponse<ClassDto>.Create(
            _mapper.Map<IEnumerable<ClassDto>>(items), total,
            pagination.PageNumber, pagination.PageSize);
    }

    public async Task<ClassDto> GetClassByIdAsync(Guid id,
        CancellationToken cancellationToken = default)
    {
        var cls = await _classRepo.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException($"Class with id '{id}' was not found.");
        return _mapper.Map<ClassDto>(cls);
    }

    public async Task<ClassDto> CreateClassAsync(CreateClassDto dto,
        CancellationToken cancellationToken = default)
    {
        if (await _classRepo.NameExistsAsync(dto.Name, dto.AcademicYear, null, cancellationToken))
            throw new InvalidOperationException(
                $"A class named '{dto.Name}' already exists for academic year '{dto.AcademicYear}'.");

        var cls = _mapper.Map<SchoolClass>(dto);
        await _classRepo.AddAsync(cls, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return _mapper.Map<ClassDto>(cls);
    }

    public async Task<ClassDto> UpdateClassAsync(Guid id, UpdateClassDto dto,
        CancellationToken cancellationToken = default)
    {
        var cls = await _classRepo.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException($"Class with id '{id}' was not found.");

        var newName = dto.Name ?? cls.Name;
        var newYear = dto.AcademicYear ?? cls.AcademicYear;

        if ((dto.Name is not null || dto.AcademicYear is not null) &&
            await _classRepo.NameExistsAsync(newName, newYear, id, cancellationToken))
        {
            throw new InvalidOperationException(
                $"A class named '{newName}' already exists for academic year '{newYear}'.");
        }

        if (dto.Name         is not null) cls.Name         = dto.Name;
        if (dto.Description  is not null) cls.Description  = dto.Description;
        if (dto.AcademicYear is not null) cls.AcademicYear = dto.AcademicYear;
        if (dto.IsActive     is not null) cls.IsActive     = dto.IsActive.Value;

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return _mapper.Map<ClassDto>(cls);
    }

    public async Task DeleteClassAsync(Guid id,
        CancellationToken cancellationToken = default)
    {
        _ = await _classRepo.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException($"Class with id '{id}' was not found.");
        await _classRepo.DeleteAsync(id, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
