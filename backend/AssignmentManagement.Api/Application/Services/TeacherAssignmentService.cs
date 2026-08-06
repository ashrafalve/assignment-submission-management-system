using AutoMapper;
using AssignmentManagement.Api.Application.DTOs.Teacher;
using AssignmentManagement.Api.Application.Interfaces;
using AssignmentManagement.Api.Domain.Entities;
using AssignmentManagement.Api.Domain.Enums;
using AssignmentManagement.Api.Domain.Interfaces;
using AssignmentManagement.Api.Shared;

namespace AssignmentManagement.Api.Application.Services;

public class TeacherAssignmentService : ITeacherAssignmentService
{
    private readonly IAssignmentRepository _assignmentRepo;
    private readonly ITeacherSubjectRepository _tsRepo;
    private readonly ISubjectRepository _subjectRepo;
    private readonly IClassRepository _classRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public TeacherAssignmentService(
        IAssignmentRepository assignmentRepo,
        ITeacherSubjectRepository tsRepo,
        ISubjectRepository subjectRepo,
        IClassRepository classRepo,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _assignmentRepo = assignmentRepo;
        _tsRepo         = tsRepo;
        _subjectRepo    = subjectRepo;
        _classRepo      = classRepo;
        _unitOfWork     = unitOfWork;
        _mapper         = mapper;
    }

    public async Task<AssignmentDto> CreateAssignmentAsync(Guid teacherId, CreateAssignmentDto dto, CancellationToken cancellationToken = default)
    {
        // 1. Verify subject exists
        _ = await _subjectRepo.GetByIdAsync(dto.SubjectId, cancellationToken)
            ?? throw new KeyNotFoundException($"Subject with id '{dto.SubjectId}' was not found.");

        // 2. Verify class exists
        _ = await _classRepo.GetByIdAsync(dto.ClassId, cancellationToken)
            ?? throw new KeyNotFoundException($"Class with id '{dto.ClassId}' was not found.");

        // 3. Verify teacher is assigned to teach this subject in this class
        var isAssigned = await _tsRepo.AssignmentExistsAsync(teacherId, dto.SubjectId, dto.ClassId, cancellationToken);
        if (!isAssigned)
        {
            throw new UnauthorizedAccessException("You are not assigned to teach this subject for the selected class.");
        }

        var assignment = new Assignment
        {
            Title       = dto.Title.Trim(),
            Description = dto.Description.Trim(),
            DueDate     = dto.DueDate,
            TotalMarks  = dto.TotalMarks,
            ClassId     = dto.ClassId,
            SubjectId   = dto.SubjectId,
            TeacherId   = teacherId,
            Status      = dto.PublishNow ? AssignmentStatus.Published : AssignmentStatus.Draft
        };

        await _assignmentRepo.AddAsync(assignment, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var detailed = await _assignmentRepo.GetByIdDetailedAsync(assignment.Id, cancellationToken);
        return _mapper.Map<AssignmentDto>(detailed!);
    }

    public async Task<AssignmentDto> UpdateAssignmentAsync(Guid teacherId, Guid assignmentId, UpdateAssignmentDto dto, CancellationToken cancellationToken = default)
    {
        var assignment = await _assignmentRepo.GetByIdAsync(assignmentId, cancellationToken)
            ?? throw new KeyNotFoundException($"Assignment with id '{assignmentId}' was not found.");

        if (assignment.TeacherId != teacherId)
        {
            throw new UnauthorizedAccessException("You can only modify assignments created by yourself.");
        }

        var targetClassId   = dto.ClassId   ?? assignment.ClassId;
        var targetSubjectId = dto.SubjectId ?? assignment.SubjectId;

        if (dto.ClassId.HasValue || dto.SubjectId.HasValue)
        {
            var isAssigned = await _tsRepo.AssignmentExistsAsync(teacherId, targetSubjectId, targetClassId, cancellationToken);
            if (!isAssigned)
            {
                throw new UnauthorizedAccessException("You are not assigned to teach this subject for the selected class.");
            }
        }

        if (dto.Title       is not null) assignment.Title       = dto.Title.Trim();
        if (dto.Description is not null) assignment.Description = dto.Description.Trim();
        if (dto.DueDate.HasValue)        assignment.DueDate     = dto.DueDate.Value;
        if (dto.TotalMarks.HasValue)     assignment.TotalMarks  = dto.TotalMarks.Value;
        if (dto.ClassId.HasValue)        assignment.ClassId     = dto.ClassId.Value;
        if (dto.SubjectId.HasValue)      assignment.SubjectId   = dto.SubjectId.Value;

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var detailed = await _assignmentRepo.GetByIdDetailedAsync(assignment.Id, cancellationToken);
        return _mapper.Map<AssignmentDto>(detailed!);
    }

    public async Task<AssignmentDto> PublishAssignmentAsync(Guid teacherId, Guid assignmentId, CancellationToken cancellationToken = default)
    {
        var assignment = await _assignmentRepo.GetByIdAsync(assignmentId, cancellationToken)
            ?? throw new KeyNotFoundException($"Assignment with id '{assignmentId}' was not found.");

        if (assignment.TeacherId != teacherId)
        {
            throw new UnauthorizedAccessException("You can only publish assignments created by yourself.");
        }

        assignment.Status = AssignmentStatus.Published;
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var detailed = await _assignmentRepo.GetByIdDetailedAsync(assignment.Id, cancellationToken);
        return _mapper.Map<AssignmentDto>(detailed!);
    }

    public async Task<AssignmentDto> SaveDraftAssignmentAsync(Guid teacherId, Guid assignmentId, CancellationToken cancellationToken = default)
    {
        var assignment = await _assignmentRepo.GetByIdAsync(assignmentId, cancellationToken)
            ?? throw new KeyNotFoundException($"Assignment with id '{assignmentId}' was not found.");

        if (assignment.TeacherId != teacherId)
        {
            throw new UnauthorizedAccessException("You can only edit assignments created by yourself.");
        }

        assignment.Status = AssignmentStatus.Draft;
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var detailed = await _assignmentRepo.GetByIdDetailedAsync(assignment.Id, cancellationToken);
        return _mapper.Map<AssignmentDto>(detailed!);
    }

    public async Task DeleteAssignmentAsync(Guid teacherId, Guid assignmentId, CancellationToken cancellationToken = default)
    {
        var assignment = await _assignmentRepo.GetByIdAsync(assignmentId, cancellationToken)
            ?? throw new KeyNotFoundException($"Assignment with id '{assignmentId}' was not found.");

        if (assignment.TeacherId != teacherId)
        {
            throw new UnauthorizedAccessException("You can only delete assignments created by yourself.");
        }

        await _assignmentRepo.DeleteAsync(assignmentId, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<PagedResponse<AssignmentDto>> GetTeacherAssignmentsAsync(Guid teacherId, PaginationParams pagination, Guid? classId = null, Guid? subjectId = null, AssignmentStatus? status = null, CancellationToken cancellationToken = default)
    {
        var paged = await _assignmentRepo.GetByTeacherAsync(teacherId, pagination, classId, subjectId, status, cancellationToken);
        return PagedResponse<AssignmentDto>.Create(
            _mapper.Map<IEnumerable<AssignmentDto>>(paged.Items),
            paged.TotalCount,
            paged.PageNumber,
            paged.PageSize);
    }

    public async Task<AssignmentDto> GetAssignmentByIdAsync(Guid teacherId, Guid assignmentId, CancellationToken cancellationToken = default)
    {
        var assignment = await _assignmentRepo.GetByIdDetailedAsync(assignmentId, cancellationToken)
            ?? throw new KeyNotFoundException($"Assignment with id '{assignmentId}' was not found.");

        if (assignment.TeacherId != teacherId)
        {
            throw new UnauthorizedAccessException("You can only view assignments created by yourself.");
        }

        return _mapper.Map<AssignmentDto>(assignment);
    }
}
