using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Moq;
using Xunit;
using AssignmentManagement.Api.Application.DTOs.Teacher;
using AssignmentManagement.Api.Application.Services;
using AssignmentManagement.Api.Domain.Entities;
using AssignmentManagement.Api.Domain.Enums;
using AssignmentManagement.Api.Domain.Exceptions;
using AssignmentManagement.Api.Domain.Interfaces;

namespace AssignmentManagement.Tests;

public class TeacherAssignmentServiceTests
{
    private readonly Mock<IAssignmentRepository> _assignmentRepoMock = new();
    private readonly Mock<ITeacherSubjectRepository> _tsRepoMock = new();
    private readonly Mock<ISubjectRepository> _subjectRepoMock = new();
    private readonly Mock<IClassRepository> _classRepoMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<IMapper> _mapperMock = new();

    private readonly TeacherAssignmentService _sut;

    public TeacherAssignmentServiceTests()
    {
        _sut = new TeacherAssignmentService(
            _assignmentRepoMock.Object,
            _tsRepoMock.Object,
            _subjectRepoMock.Object,
            _classRepoMock.Object,
            _unitOfWorkMock.Object,
            _mapperMock.Object);
    }

    [Fact]
    public async Task CreateAssignmentAsync_AssignedTeacher_CreatesAssignmentSuccessfully()
    {
        // Arrange
        var teacherId = Guid.NewGuid();
        var dto = new CreateAssignmentDto
        {
            Title = "Math Homework 1",
            Description = "Solve 10 problems",
            DueDate = DateTime.UtcNow.AddDays(7),
            TotalMarks = 100,
            ClassId = Guid.NewGuid(),
            SubjectId = Guid.NewGuid(),
            PublishNow = true
        };

        _subjectRepoMock.Setup(r => r.GetByIdAsync(dto.SubjectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Subject { Id = dto.SubjectId, Name = "Math", Code = "MATH-101" });

        _classRepoMock.Setup(r => r.GetByIdAsync(dto.ClassId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SchoolClass { Id = dto.ClassId, Name = "Grade 10 - A" });

        _tsRepoMock.Setup(r => r.AssignmentExistsAsync(teacherId, dto.SubjectId, dto.ClassId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var createdAssignment = new Assignment
        {
            Id = Guid.NewGuid(),
            Title = dto.Title,
            TeacherId = teacherId,
            SubjectId = dto.SubjectId,
            ClassId = dto.ClassId,
            Status = AssignmentStatus.Published
        };

        _assignmentRepoMock.Setup(r => r.AddAsync(It.IsAny<Assignment>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdAssignment);

        _assignmentRepoMock.Setup(r => r.GetByIdDetailedAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdAssignment);

        _mapperMock.Setup(m => m.Map<AssignmentDto>(It.IsAny<Assignment>()))
            .Returns(new AssignmentDto { Id = createdAssignment.Id, Title = dto.Title });

        // Act
        var result = await _sut.CreateAssignmentAsync(teacherId, dto);

        // Assert
        result.Should().NotBeNull();
        result.Title.Should().Be(dto.Title);
        _assignmentRepoMock.Verify(r => r.AddAsync(It.IsAny<Assignment>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAssignmentAsync_UnassignedTeacher_ThrowsForbiddenException()
    {
        // Arrange
        var teacherId = Guid.NewGuid();
        var dto = new CreateAssignmentDto
        {
            Title = "Math Homework 1",
            Description = "Solve 10 problems",
            DueDate = DateTime.UtcNow.AddDays(7),
            TotalMarks = 100,
            ClassId = Guid.NewGuid(),
            SubjectId = Guid.NewGuid()
        };

        _subjectRepoMock.Setup(r => r.GetByIdAsync(dto.SubjectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Subject { Id = dto.SubjectId });
        _classRepoMock.Setup(r => r.GetByIdAsync(dto.ClassId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SchoolClass { Id = dto.ClassId });

        // Unassigned teacher check
        _tsRepoMock.Setup(r => r.AssignmentExistsAsync(teacherId, dto.SubjectId, dto.ClassId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        Func<Task> act = async () => await _sut.CreateAssignmentAsync(teacherId, dto);

        // Assert
        await act.Should().ThrowAsync<ForbiddenException>()
            .WithMessage("You are not assigned to teach this subject for the selected class.");
    }

    [Fact]
    public async Task UpdateAssignmentAsync_OtherTeacher_ThrowsForbiddenException()
    {
        // Arrange
        var ownerTeacherId = Guid.NewGuid();
        var otherTeacherId = Guid.NewGuid();
        var assignmentId = Guid.NewGuid();

        var assignment = new Assignment
        {
            Id = assignmentId,
            TeacherId = ownerTeacherId
        };

        _assignmentRepoMock.Setup(r => r.GetByIdAsync(assignmentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(assignment);

        var dto = new UpdateAssignmentDto { Title = "Hacked Title" };

        // Act
        Func<Task> act = async () => await _sut.UpdateAssignmentAsync(otherTeacherId, assignmentId, dto);

        // Assert
        await act.Should().ThrowAsync<ForbiddenException>()
            .WithMessage("You can only modify assignments created by yourself.");
    }
}
