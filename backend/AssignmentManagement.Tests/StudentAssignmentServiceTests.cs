using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Moq;
using Xunit;
using AssignmentManagement.Api.Application.DTOs.Student;
using AssignmentManagement.Api.Application.Services;
using AssignmentManagement.Api.Domain.Entities;
using AssignmentManagement.Api.Domain.Enums;
using AssignmentManagement.Api.Domain.Exceptions;
using AssignmentManagement.Api.Domain.Interfaces;

namespace AssignmentManagement.Tests;

public class StudentAssignmentServiceTests
{
    private readonly Mock<IUserRepository> _userRepoMock = new();
    private readonly Mock<IAssignmentRepository> _assignmentRepoMock = new();
    private readonly Mock<ISubmissionRepository> _submissionRepoMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<IMapper> _mapperMock = new();

    private readonly StudentAssignmentService _sut;

    public StudentAssignmentServiceTests()
    {
        _sut = new StudentAssignmentService(
            _userRepoMock.Object,
            _assignmentRepoMock.Object,
            _submissionRepoMock.Object,
            _unitOfWorkMock.Object,
            _mapperMock.Object);
    }

    [Fact]
    public async Task SubmitAssignmentAsync_BeforeDeadline_SubmitsSuccessfully()
    {
        // Arrange
        var studentId = Guid.NewGuid();
        var classId = Guid.NewGuid();
        var assignmentId = Guid.NewGuid();

        var student = new User { Id = studentId, ClassId = classId, Role = UserRole.Student };
        var assignment = new Assignment
        {
            Id = assignmentId,
            ClassId = classId,
            Status = AssignmentStatus.Published,
            DueDate = DateTime.UtcNow.AddHours(2) // Future deadline
        };

        _userRepoMock.Setup(r => r.GetByIdAsync(studentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(student);

        _assignmentRepoMock.Setup(r => r.GetByIdDetailedAsync(assignmentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(assignment);

        _submissionRepoMock.Setup(r => r.GetByAssignmentAndStudentAsync(assignmentId, studentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Submission?)null);

        var dto = new SubmitAssignmentDto { AssignmentId = assignmentId, Content = "My solution text" };

        var newSubmission = new Submission
        {
            Id = Guid.NewGuid(),
            AssignmentId = assignmentId,
            StudentId = studentId,
            Content = dto.Content,
            Status = SubmissionStatus.Submitted
        };

        _submissionRepoMock.Setup(r => r.AddAsync(It.IsAny<Submission>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(newSubmission);
        _submissionRepoMock.Setup(r => r.GetByIdDetailedAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(newSubmission);

        _mapperMock.Setup(m => m.Map<SubmissionDto>(It.IsAny<Submission>()))
            .Returns(new SubmissionDto { Id = newSubmission.Id, Content = dto.Content, Status = SubmissionStatus.Submitted });

        // Act
        var result = await _sut.SubmitAssignmentAsync(studentId, dto);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be(SubmissionStatus.Submitted);
        _submissionRepoMock.Verify(r => r.AddAsync(It.IsAny<Submission>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task SubmitAssignmentAsync_AfterDeadline_ThrowsBusinessRuleException()
    {
        // Arrange
        var studentId = Guid.NewGuid();
        var classId = Guid.NewGuid();
        var assignmentId = Guid.NewGuid();

        var student = new User { Id = studentId, ClassId = classId, Role = UserRole.Student };
        var assignment = new Assignment
        {
            Id = assignmentId,
            ClassId = classId,
            Status = AssignmentStatus.Published,
            DueDate = DateTime.UtcNow.AddMinutes(-10) // Expired deadline
        };

        _userRepoMock.Setup(r => r.GetByIdAsync(studentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(student);

        _assignmentRepoMock.Setup(r => r.GetByIdDetailedAsync(assignmentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(assignment);

        var dto = new SubmitAssignmentDto { AssignmentId = assignmentId, Content = "Late solution" };

        // Act
        Func<Task> act = async () => await _sut.SubmitAssignmentAsync(studentId, dto);

        // Assert
        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage("*deadline*passed*");
    }

    [Fact]
    public async Task UpdateSubmissionAsync_BeforeDeadline_UpdatesSuccessfully()
    {
        // Arrange
        var studentId = Guid.NewGuid();
        var classId = Guid.NewGuid();
        var submissionId = Guid.NewGuid();

        var student = new User { Id = studentId, ClassId = classId };
        var assignment = new Assignment { DueDate = DateTime.UtcNow.AddHours(5) }; // Future deadline

        var existingSubmission = new Submission
        {
            Id = submissionId,
            StudentId = studentId,
            Assignment = assignment,
            Content = "Old Content"
        };

        _userRepoMock.Setup(r => r.GetByIdAsync(studentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(student);
        _submissionRepoMock.Setup(r => r.GetByIdDetailedAsync(submissionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingSubmission);

        var updateDto = new UpdateSubmissionDto { Content = "Updated Content" };

        _mapperMock.Setup(m => m.Map<SubmissionDto>(It.IsAny<Submission>()))
            .Returns(new SubmissionDto { Id = submissionId, Content = updateDto.Content });

        // Act
        var result = await _sut.UpdateSubmissionAsync(studentId, submissionId, updateDto);

        // Assert
        result.Should().NotBeNull();
        existingSubmission.Content.Should().Be("Updated Content");
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateSubmissionAsync_AfterDeadline_ThrowsBusinessRuleException()
    {
        // Arrange
        var studentId = Guid.NewGuid();
        var classId = Guid.NewGuid();
        var submissionId = Guid.NewGuid();

        var student = new User { Id = studentId, ClassId = classId };
        var assignment = new Assignment { DueDate = DateTime.UtcNow.AddMinutes(-5) }; // Expired deadline

        var existingSubmission = new Submission
        {
            Id = submissionId,
            StudentId = studentId,
            Assignment = assignment
        };

        _userRepoMock.Setup(r => r.GetByIdAsync(studentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(student);
        _submissionRepoMock.Setup(r => r.GetByIdDetailedAsync(submissionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingSubmission);

        var updateDto = new UpdateSubmissionDto { Content = "Updated Content" };

        // Act
        Func<Task> act = async () => await _sut.UpdateSubmissionAsync(studentId, submissionId, updateDto);

        // Assert
        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage("*deadline has passed*");
    }
}
