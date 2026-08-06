using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using BCrypt.Net;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using AssignmentManagement.Api.Application.DTOs.Auth;
using AssignmentManagement.Api.Application.Interfaces;
using AssignmentManagement.Api.Application.Services;
using AssignmentManagement.Api.Domain.Entities;
using AssignmentManagement.Api.Domain.Enums;
using AssignmentManagement.Api.Domain.Interfaces;

namespace AssignmentManagement.Tests;

public class AuthServiceTests
{
    private readonly Mock<IUserRepository> _userRepoMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<IJwtService> _jwtServiceMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<ILogger<AuthService>> _loggerMock = new();

    private readonly AuthService _sut;

    public AuthServiceTests()
    {
        _sut = new AuthService(
            _userRepoMock.Object,
            _unitOfWorkMock.Object,
            _jwtServiceMock.Object,
            _mapperMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task LoginAsync_ValidCredentials_ReturnsAuthResponse()
    {
        // Arrange
        var password = "Password@123";
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "student@test.com",
            PasswordHash = passwordHash,
            FirstName = "Jane",
            LastName = "Doe",
            Role = UserRole.Student,
            IsActive = true
        };

        _userRepoMock.Setup(r => r.GetByEmailAsync(user.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _jwtServiceMock.Setup(j => j.GenerateAccessToken(It.IsAny<User>()))
            .Returns("fake-jwt-token");
        _jwtServiceMock.Setup(j => j.GenerateRefreshToken())
            .Returns("fake-refresh-token");
        _jwtServiceMock.Setup(j => j.GetAccessTokenExpiry())
            .Returns(DateTime.UtcNow.AddHours(1));

        var loginDto = new LoginRequestDto { Email = user.Email, Password = password };

        // Act
        var result = await _sut.LoginAsync(loginDto);

        // Assert
        result.Should().NotBeNull();
        result.Email.Should().Be(user.Email);
        result.AccessToken.Should().Be("fake-jwt-token");
        result.RefreshToken.Should().Be("fake-refresh-token");

        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task LoginAsync_InvalidPassword_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "student@test.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("CorrectPassword"),
            IsActive = true
        };

        _userRepoMock.Setup(r => r.GetByEmailAsync(user.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var loginDto = new LoginRequestDto { Email = user.Email, Password = "WrongPassword" };

        // Act
        Func<Task> act = async () => await _sut.LoginAsync(loginDto);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("Invalid email or password.");
    }

    [Fact]
    public async Task LoginAsync_InactiveUser_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        var password = "Password@123";
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "inactive@test.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
            IsActive = false
        };

        _userRepoMock.Setup(r => r.GetByEmailAsync(user.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var loginDto = new LoginRequestDto { Email = user.Email, Password = password };

        // Act
        Func<Task> act = async () => await _sut.LoginAsync(loginDto);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("*deactivated*");
    }
}
