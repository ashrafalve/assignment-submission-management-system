using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Moq;
using Xunit;
using AssignmentManagement.Api.Application.DTOs.Student;
using AssignmentManagement.Api.Application.DTOs.Teacher;
using AssignmentManagement.Api.Application.Services;
using AssignmentManagement.Api.Domain.Entities;
using AssignmentManagement.Api.Domain.Enums;
using AssignmentManagement.Api.Domain.Exceptions;
using AssignmentManagement.Api.Domain.Interfaces;

namespace AssignmentManagement.Tests;

public class TeacherSubmissionServiceTests
{
    private readonly Mock<IAssignmentRepository> _assignmentRepoMock = new();
    private readonly Mock<ISubmissionRepository> _submissionRepoMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<IMapper> _mapperMock = new();

    private readonly TeacherSubmissionService _sut;

    public TeacherSubmissionServiceTests()
    {
        _sut = new TeacherSubmissionService(
            _assignmentRepoMock.Object,
            _submissionRepoMock.Object,
            _unitOfWorkMock.Object,
            _mapperMock.Object);
    }

    [Fact]
    public async Task GradeSubmissionAsync_ValidMarks_GradesSuccessfully()
    {
        // Arrange
        var teacherId = Guid.NewGuid();
        var submissionId = Guid.NewGuid();
        var maxMarks = 100m;

        var assignment = new Assignment
        {
            Id = Guid.NewGuid(),
            TeacherId = teacherId,
            TotalMarks = maxMarks
        };

        var submission = new Submission
        {
            Id = submissionId,
            Assignment = assignment,
            Status = SubmissionStatus.Submitted
        };

        _submissionRepoMock.Setup(r => r.GetByIdDetailedAsync(submissionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(submission);

        var gradeDto = new GradeSubmissionDto
        {
            MarksObtained = 85m,
            Feedback = "Well done!",
            Status = SubmissionStatus.Graded
        };

        _mapperMock.Setup(m => m.Map<SubmissionDto>(It.IsAny<Submission>()))
            .Returns(new SubmissionDto { Id = submissionId, MarksObtained = 85m, Feedback = "Well done!", Status = SubmissionStatus.Graded });

        // Act
        var result = await _sut.GradeSubmissionAsync(teacherId, submissionId, gradeDto);

        // Assert
        result.Should().NotBeNull();
        result.MarksObtained.Should().Be(85m);
        submission.Status.Should().Be(SubmissionStatus.Graded);
        submission.GradedAt.Should().NotBeNull();
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GradeSubmissionAsync_MarksExceedMaxMarks_ThrowsBusinessRuleException()
    {
        // Arrange
        var teacherId = Guid.NewGuid();
        var submissionId = Guid.NewGuid();
        var maxMarks = 50m; // Max marks is 50

        var assignment = new Assignment
        {
            Id = Guid.NewGuid(),
            TeacherId = teacherId,
            TotalMarks = maxMarks
        };

        var submission = new Submission
        {
            Id = submissionId,
            Assignment = assignment
        };

        _submissionRepoMock.Setup(r => r.GetByIdDetailedAsync(submissionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(submission);

        var gradeDto = new GradeSubmissionDto
        {
            MarksObtained = 60m, // 60 > 50 -> Exceeds max marks!
            Feedback = "Extra credit attempt"
        };

        // Act
        Func<Task> act = async () => await _sut.GradeSubmissionAsync(teacherId, submissionId, gradeDto);

        // Assert
        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage("*cannot exceed maximum marks*");
    }

    [Fact]
    public async Task GradeSubmissionAsync_OtherTeacher_ThrowsForbiddenException()
    {
        // Arrange
        var ownerTeacherId = Guid.NewGuid();
        var otherTeacherId = Guid.NewGuid();
        var submissionId = Guid.NewGuid();

        var assignment = new Assignment
        {
            Id = Guid.NewGuid(),
            TeacherId = ownerTeacherId,
            TotalMarks = 100m
        };

        var submission = new Submission
        {
            Id = submissionId,
            Assignment = assignment
        };

        _submissionRepoMock.Setup(r => r.GetByIdDetailedAsync(submissionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(submission);

        var gradeDto = new GradeSubmissionDto { MarksObtained = 80m };

        // Act
        Func<Task> act = async () => await _sut.GradeSubmissionAsync(otherTeacherId, submissionId, gradeDto);

        // Assert
        await act.Should().ThrowAsync<ForbiddenException>()
            .WithMessage("*created by yourself*");
    }
}
