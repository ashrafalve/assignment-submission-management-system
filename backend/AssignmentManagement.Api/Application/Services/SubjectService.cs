using AutoMapper;
using Microsoft.EntityFrameworkCore;
using AssignmentManagement.Api.Application.DTOs.Admin;
using AssignmentManagement.Api.Application.Interfaces;
using AssignmentManagement.Api.Domain.Entities;
using AssignmentManagement.Api.Domain.Interfaces;
using AssignmentManagement.Api.Infrastructure.Persistence;
using AssignmentManagement.Api.Shared;

namespace AssignmentManagement.Api.Application.Services;

public class SubjectService : ISubjectService
{
    private readonly ISubjectRepository _subjectRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ApplicationDbContext _context;

    public SubjectService(ISubjectRepository subjectRepo, IUnitOfWork unitOfWork,
        IMapper mapper, ApplicationDbContext context)
    {
        _subjectRepo = subjectRepo;
        _unitOfWork  = unitOfWork;
        _mapper      = mapper;
        _context     = context;
    }

    public async Task<PagedResponse<SubjectDto>> GetSubjectsAsync(PaginationParams pagination,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Subjects.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(pagination.SearchTerm))
        {
            var term = pagination.SearchTerm.ToLower();
            query = query.Where(s => s.Name.ToLower().Contains(term) ||
                                     s.Code.ToLower().Contains(term));
        }

        query = pagination.SortBy?.ToLower() switch
        {
            "code" => pagination.SortDescending
                ? query.OrderByDescending(s => s.Code)
                : query.OrderBy(s => s.Code),
            _ => pagination.SortDescending
                ? query.OrderByDescending(s => s.Name)
                : query.OrderBy(s => s.Name)
        };

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((pagination.PageNumber - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .ToListAsync(cancellationToken);

        return PagedResponse<SubjectDto>.Create(
            _mapper.Map<IEnumerable<SubjectDto>>(items), total,
            pagination.PageNumber, pagination.PageSize);
    }

    public async Task<SubjectDto> GetSubjectByIdAsync(Guid id,
        CancellationToken cancellationToken = default)
    {
        var subject = await _subjectRepo.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException($"Subject with id '{id}' was not found.");
        return _mapper.Map<SubjectDto>(subject);
    }

    public async Task<SubjectDto> CreateSubjectAsync(CreateSubjectDto dto,
        CancellationToken cancellationToken = default)
    {
        var code = dto.Code.ToUpperInvariant();
        if (await _subjectRepo.CodeExistsAsync(code, null, cancellationToken))
            throw new InvalidOperationException($"Subject code '{code}' is already in use.");

        var subject = _mapper.Map<Subject>(dto);
        subject.Code = code;

        await _subjectRepo.AddAsync(subject, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return _mapper.Map<SubjectDto>(subject);
    }

    public async Task<SubjectDto> UpdateSubjectAsync(Guid id, UpdateSubjectDto dto,
        CancellationToken cancellationToken = default)
    {
        var subject = await _subjectRepo.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException($"Subject with id '{id}' was not found.");

        if (dto.Code is not null)
        {
            var code = dto.Code.ToUpperInvariant();
            if (await _subjectRepo.CodeExistsAsync(code, id, cancellationToken))
                throw new InvalidOperationException($"Subject code '{code}' is already in use.");
            subject.Code = code;
        }

        if (dto.Name        is not null) subject.Name        = dto.Name;
        if (dto.Description is not null) subject.Description = dto.Description;
        if (dto.IsActive    is not null) subject.IsActive    = dto.IsActive.Value;

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return _mapper.Map<SubjectDto>(subject);
    }

    public async Task DeleteSubjectAsync(Guid id,
        CancellationToken cancellationToken = default)
    {
        _ = await _subjectRepo.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException($"Subject with id '{id}' was not found.");
        await _subjectRepo.DeleteAsync(id, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
