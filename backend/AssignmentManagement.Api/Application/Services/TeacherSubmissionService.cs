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

public class TeacherSubmissionService : ITeacherSubmissionService
{
    private readonly IAssignmentRepository _assignmentRepo;
    private readonly ISubmissionRepository _submissionRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public TeacherSubmissionService(
        IAssignmentRepository assignmentRepo,
        ISubmissionRepository submissionRepo,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _assignmentRepo = assignmentRepo;
        _submissionRepo = submissionRepo;
        _unitOfWork     = unitOfWork;
        _mapper         = mapper;
    }

    public async Task<PagedResponse<SubmissionDto>> GetSubmissionsForAssignmentAsync(
        Guid teacherId, Guid assignmentId, PaginationParams pagination, CancellationToken cancellationToken = default)
    {
        var assignment = await _assignmentRepo.GetByIdAsync(assignmentId, cancellationToken)
            ?? throw new NotFoundException("Assignment", assignmentId);

        if (assignment.TeacherId != teacherId)
        {
            throw new ForbiddenException("You can only view submissions for assignments created by yourself.");
        }

        var paged = await _submissionRepo.GetSubmissionsForAssignmentAsync(assignmentId, pagination, cancellationToken);

        return PagedResponse<SubmissionDto>.Create(
            _mapper.Map<IEnumerable<SubmissionDto>>(paged.Items),
            paged.TotalCount,
            paged.PageNumber,
            paged.PageSize);
    }

    public async Task<SubmissionDto> GetSubmissionByIdAsync(
        Guid teacherId, Guid submissionId, CancellationToken cancellationToken = default)
    {
        var submission = await _submissionRepo.GetByIdDetailedAsync(submissionId, cancellationToken)
            ?? throw new NotFoundException("Submission", submissionId);

        if (submission.Assignment.TeacherId != teacherId)
        {
            throw new ForbiddenException("You can only view submissions for assignments created by yourself.");
        }

        return _mapper.Map<SubmissionDto>(submission);
    }

    public async Task<SubmissionDto> GradeSubmissionAsync(
        Guid teacherId, Guid submissionId, GradeSubmissionDto dto, CancellationToken cancellationToken = default)
    {
        var submission = await _submissionRepo.GetByIdDetailedAsync(submissionId, cancellationToken)
            ?? throw new NotFoundException("Submission", submissionId);

        if (submission.Assignment.TeacherId != teacherId)
        {
            throw new ForbiddenException("You can only grade submissions for assignments created by yourself.");
        }

        // Marks validation rule: Marks cannot exceed MaxMarks (TotalMarks)
        if (dto.MarksObtained > submission.Assignment.TotalMarks)
        {
            throw new BusinessRuleException($"Marks obtained ({dto.MarksObtained}) cannot exceed maximum marks ({submission.Assignment.TotalMarks}) for this assignment.");
        }

        submission.MarksObtained = dto.MarksObtained;
        submission.Feedback      = dto.Feedback?.Trim();
        submission.Status        = dto.Status;
        submission.GradedAt      = DateTime.UtcNow;

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var updated = await _submissionRepo.GetByIdDetailedAsync(submissionId, cancellationToken);
        return _mapper.Map<SubmissionDto>(updated!);
    }

    public async Task<SubmissionDto> ChangeSubmissionStatusAsync(
        Guid teacherId, Guid submissionId, ChangeSubmissionStatusDto dto, CancellationToken cancellationToken = default)
    {
        var submission = await _submissionRepo.GetByIdDetailedAsync(submissionId, cancellationToken)
            ?? throw new NotFoundException("Submission", submissionId);

        if (submission.Assignment.TeacherId != teacherId)
        {
            throw new ForbiddenException("You can only review submissions for assignments created by yourself.");
        }

        submission.Status = dto.Status;
        if (dto.Feedback is not null)
        {
            submission.Feedback = dto.Feedback.Trim();
        }

        if (dto.Status == SubmissionStatus.Graded && !submission.GradedAt.HasValue)
        {
            submission.GradedAt = DateTime.UtcNow;
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var updated = await _submissionRepo.GetByIdDetailedAsync(submissionId, cancellationToken);
        return _mapper.Map<SubmissionDto>(updated!);
    }
}
