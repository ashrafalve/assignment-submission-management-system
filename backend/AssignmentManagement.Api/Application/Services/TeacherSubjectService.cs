using AutoMapper;
using AssignmentManagement.Api.Application.DTOs.Admin;
using AssignmentManagement.Api.Application.Interfaces;
using AssignmentManagement.Api.Domain.Entities;
using AssignmentManagement.Api.Domain.Enums;
using AssignmentManagement.Api.Domain.Interfaces;

namespace AssignmentManagement.Api.Application.Services;

public class TeacherSubjectService : ITeacherSubjectService
{
    private readonly ITeacherSubjectRepository _tsRepo;
    private readonly IUserRepository _userRepo;
    private readonly ISubjectRepository _subjectRepo;
    private readonly IClassRepository _classRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public TeacherSubjectService(
        ITeacherSubjectRepository tsRepo,
        IUserRepository userRepo,
        ISubjectRepository subjectRepo,
        IClassRepository classRepo,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _tsRepo      = tsRepo;
        _userRepo    = userRepo;
        _subjectRepo = subjectRepo;
        _classRepo   = classRepo;
        _unitOfWork  = unitOfWork;
        _mapper      = mapper;
    }

    public async Task<IEnumerable<TeacherSubjectDto>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var assignments = await _tsRepo.GetAllAsync(cancellationToken);
        return _mapper.Map<IEnumerable<TeacherSubjectDto>>(assignments);
    }

    public async Task<IEnumerable<TeacherSubjectDto>> GetByTeacherAsync(Guid teacherId,
        CancellationToken cancellationToken = default)
    {
        var assignments = await _tsRepo.GetByTeacherAsync(teacherId, cancellationToken);
        return _mapper.Map<IEnumerable<TeacherSubjectDto>>(assignments);
    }

    public async Task<IEnumerable<TeacherSubjectDto>> GetByClassAsync(Guid classId,
        CancellationToken cancellationToken = default)
    {
        var assignments = await _tsRepo.GetByClassAsync(classId, cancellationToken);
        return _mapper.Map<IEnumerable<TeacherSubjectDto>>(assignments);
    }

    public async Task<TeacherSubjectDto> AssignTeacherAsync(AssignTeacherDto dto,
        CancellationToken cancellationToken = default)
    {
        // Validate teacher exists and has Teacher role
        var teacher = await _userRepo.GetByIdAsync(dto.TeacherId, cancellationToken)
            ?? throw new KeyNotFoundException($"User with id '{dto.TeacherId}' was not found.");
        if (teacher.Role != UserRole.Teacher)
            throw new InvalidOperationException("The specified user is not a Teacher.");

        // Validate subject exists
        _ = await _subjectRepo.GetByIdAsync(dto.SubjectId, cancellationToken)
            ?? throw new KeyNotFoundException($"Subject with id '{dto.SubjectId}' was not found.");

        // Validate class exists
        _ = await _classRepo.GetByIdAsync(dto.ClassId, cancellationToken)
            ?? throw new KeyNotFoundException($"Class with id '{dto.ClassId}' was not found.");

        // Check for duplicate assignment
        if (await _tsRepo.AssignmentExistsAsync(dto.TeacherId, dto.SubjectId, dto.ClassId, cancellationToken))
            throw new InvalidOperationException(
                "This teacher is already assigned to that subject and class.");

        var assignment = new TeacherSubject
        {
            TeacherId  = dto.TeacherId,
            SubjectId  = dto.SubjectId,
            ClassId    = dto.ClassId,
            AssignedAt = DateTime.UtcNow,
            IsActive   = true
        };

        await _tsRepo.AddAsync(assignment, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var detailed = await _tsRepo.GetDetailedAsync(assignment.Id, cancellationToken);
        return _mapper.Map<TeacherSubjectDto>(detailed!);
    }

    public async Task RemoveAssignmentAsync(Guid id,
        CancellationToken cancellationToken = default)
    {
        _ = await _tsRepo.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException($"Assignment with id '{id}' was not found.");
        await _tsRepo.DeleteAsync(id, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
