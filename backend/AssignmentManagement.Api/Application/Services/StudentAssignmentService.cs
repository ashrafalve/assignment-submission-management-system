using AutoMapper;
using AssignmentManagement.Api.Application.DTOs.Student;
using AssignmentManagement.Api.Application.DTOs.Teacher;
using AssignmentManagement.Api.Application.Interfaces;
using AssignmentManagement.Api.Domain.Entities;
using AssignmentManagement.Api.Domain.Enums;
using AssignmentManagement.Api.Domain.Exceptions;
using AssignmentManagement.Api.Domain.Interfaces;
using AssignmentManagement.Api.Shared;

namespace AssignmentManagement.Api.Application.Services;

public class StudentAssignmentService : IStudentAssignmentService
{
    private readonly IUserRepository _userRepo;
    private readonly IAssignmentRepository _assignmentRepo;
    private readonly ISubmissionRepository _submissionRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public StudentAssignmentService(
        IUserRepository userRepo,
        IAssignmentRepository assignmentRepo,
        ISubmissionRepository submissionRepo,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _userRepo       = userRepo;
        _assignmentRepo = assignmentRepo;
        _submissionRepo = submissionRepo;
        _unitOfWork     = unitOfWork;
        _mapper         = mapper;
    }

    private async Task<User> GetStudentWithClassAsync(Guid studentId, CancellationToken cancellationToken)
    {
        var student = await _userRepo.GetByIdAsync(studentId, cancellationToken)
            ?? throw new NotFoundException("User", studentId);

        if (!student.ClassId.HasValue)
        {
            throw new BusinessRuleException("You are not assigned to any class. Please contact your administrator.");
        }

        return student;
    }

    public async Task<PagedResponse<AssignmentDto>> GetPublishedAssignmentsAsync(
        Guid studentId, PaginationParams pagination, Guid? subjectId = null, CancellationToken cancellationToken = default)
    {
        var student = await GetStudentWithClassAsync(studentId, cancellationToken);
        var paged = await _assignmentRepo.GetPublishedForStudentAsync(student.ClassId!.Value, pagination, subjectId, cancellationToken);

        return PagedResponse<AssignmentDto>.Create(
            _mapper.Map<IEnumerable<AssignmentDto>>(paged.Items),
            paged.TotalCount,
            paged.PageNumber,
            paged.PageSize);
    }

    public async Task<StudentAssignmentDetailDto> GetAssignmentDetailsAsync(
        Guid studentId, Guid assignmentId, CancellationToken cancellationToken = default)
    {
        var student = await GetStudentWithClassAsync(studentId, cancellationToken);
        var assignment = await _assignmentRepo.GetByIdDetailedAsync(assignmentId, cancellationToken)
            ?? throw new NotFoundException("Assignment", assignmentId);

        if (assignment.ClassId != student.ClassId!.Value)
        {
            throw new ForbiddenException("This assignment is not for your assigned class.");
        }

        if (assignment.Status != AssignmentStatus.Published)
        {
            throw new ForbiddenException("This assignment is not published yet.");
        }

        var submission = await _submissionRepo.GetByAssignmentAndStudentAsync(assignmentId, studentId, cancellationToken);

        return new StudentAssignmentDetailDto
        {
            Assignment = _mapper.Map<AssignmentDto>(assignment),
            Submission = submission != null ? _mapper.Map<SubmissionDto>(submission) : null
        };
    }

    public async Task<SubmissionDto> SubmitAssignmentAsync(
        Guid studentId, SubmitAssignmentDto dto, CancellationToken cancellationToken = default)
    {
        var student = await GetStudentWithClassAsync(studentId, cancellationToken);

        var assignment = await _assignmentRepo.GetByIdDetailedAsync(dto.AssignmentId, cancellationToken)
            ?? throw new NotFoundException("Assignment", dto.AssignmentId);

        if (assignment.ClassId != student.ClassId!.Value)
        {
            throw new ForbiddenException("This assignment is not for your assigned class.");
        }

        if (assignment.Status != AssignmentStatus.Published)
        {
            throw new BusinessRuleException("You cannot submit work to an assignment that is not published.");
        }

        // Deadline check for submission
        if (DateTime.UtcNow > assignment.DueDate)
        {
            throw new BusinessRuleException("The deadline for this assignment has passed. Submissions are no longer accepted.");
        }

        // Check if existing submission exists
        var existing = await _submissionRepo.GetByAssignmentAndStudentAsync(dto.AssignmentId, studentId, cancellationToken);
        if (existing != null)
        {
            throw new ConflictException("You have already submitted for this assignment. Please use the update endpoint to revise your submission.");
        }

        var submission = new Submission
        {
            AssignmentId = dto.AssignmentId,
            StudentId    = studentId,
            Content      = dto.Content?.Trim(),
            FilePath     = dto.FilePath?.Trim(),
            Status       = SubmissionStatus.Submitted,
            SubmittedAt  = DateTime.UtcNow
        };

        await _submissionRepo.AddAsync(submission, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var detailed = await _submissionRepo.GetByIdDetailedAsync(submission.Id, cancellationToken);
        return _mapper.Map<SubmissionDto>(detailed!);
    }

    public async Task<SubmissionDto> UpdateSubmissionAsync(
        Guid studentId, Guid submissionId, UpdateSubmissionDto dto, CancellationToken cancellationToken = default)
    {
        var student = await GetStudentWithClassAsync(studentId, cancellationToken);

        var submission = await _submissionRepo.GetByIdDetailedAsync(submissionId, cancellationToken)
            ?? throw new NotFoundException("Submission", submissionId);

        if (submission.StudentId != studentId)
        {
            throw new ForbiddenException("You can only update your own submission.");
        }

        // Check deadline constraint: "Student can update before deadline"
        if (DateTime.UtcNow > submission.Assignment.DueDate)
        {
            throw new BusinessRuleException("The assignment deadline has passed. Submissions can no longer be updated after the deadline.");
        }

        if (submission.Status == SubmissionStatus.Graded)
        {
            throw new BusinessRuleException("This submission has already been graded and cannot be modified.");
        }

        if (dto.Content is not null)  submission.Content  = dto.Content.Trim();
        if (dto.FilePath is not null) submission.FilePath = dto.FilePath.Trim();
        submission.SubmittedAt = DateTime.UtcNow;

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var detailed = await _submissionRepo.GetByIdDetailedAsync(submission.Id, cancellationToken);
        return _mapper.Map<SubmissionDto>(detailed!);
    }

    public async Task<IEnumerable<SubmissionDto>> GetMySubmissionsAsync(
        Guid studentId, CancellationToken cancellationToken = default)
    {
        _ = await GetStudentWithClassAsync(studentId, cancellationToken);
        var submissions = await _submissionRepo.GetStudentSubmissionsAsync(studentId, cancellationToken);
        return _mapper.Map<IEnumerable<SubmissionDto>>(submissions);
    }
}
