using AutoMapper;
using BCrypt.Net;
using AssignmentManagement.Api.Application.DTOs.Auth;
using AssignmentManagement.Api.Application.Interfaces;
using AssignmentManagement.Api.Domain.Entities;
using AssignmentManagement.Api.Domain.Enums;
using AssignmentManagement.Api.Domain.Interfaces;

namespace AssignmentManagement.Api.Application.Services;

/// <summary>
/// Handles user authentication, registration, and token lifecycle management.
/// </summary>
public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtService _jwtService;
    private readonly IMapper _mapper;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        IJwtService jwtService,
        IMapper mapper,
        ILogger<AuthService> logger)
    {
        _userRepository = userRepository;
        _unitOfWork     = unitOfWork;
        _jwtService     = jwtService;
        _mapper         = mapper;
        _logger         = logger;
    }

    // ── Login ──────────────────────────────────────────────────────────────────
    public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);

        if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid email or password.");

        if (!user.IsActive)
            throw new UnauthorizedAccessException("Your account has been deactivated. Please contact support.");

        // Rotate refresh token on every login
        user.LastLoginAt        = DateTime.UtcNow;
        user.RefreshToken       = _jwtService.GenerateRefreshToken();
        user.RefreshTokenExpiresAt = DateTime.UtcNow.AddDays(7);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("User {Email} logged in successfully.", user.Email);
        return BuildAuthResponse(user);
    }

    // ── Register ───────────────────────────────────────────────────────────────
    public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request,
        CancellationToken cancellationToken = default)
    {
        if (await _userRepository.EmailExistsAsync(request.Email, cancellationToken))
            throw new InvalidOperationException($"An account with email '{request.Email}' already exists.");

        if (!Enum.TryParse<UserRole>(request.Role, ignoreCase: true, out var role))
            throw new ArgumentException($"Invalid role: {request.Role}.");

        var user = new User
        {
            FirstName    = request.FirstName.Trim(),
            LastName     = request.LastName.Trim(),
            Email        = request.Email.Trim().ToLowerInvariant(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password, workFactor: 12),
            Role         = role,
            ClassId      = request.ClassId,
            IsActive     = true,
            RefreshToken = _jwtService.GenerateRefreshToken(),
            RefreshTokenExpiresAt = DateTime.UtcNow.AddDays(7)
        };

        await _userRepository.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("New user registered: {Email} ({Role}).", user.Email, user.Role);
        return BuildAuthResponse(user);
    }

    // ── Refresh Token ──────────────────────────────────────────────────────────
    public async Task<AuthResponseDto> RefreshTokenAsync(string refreshToken,
        CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByRefreshTokenAsync(refreshToken, cancellationToken);

        if (user is null || user.RefreshTokenExpiresAt <= DateTime.UtcNow)
            throw new UnauthorizedAccessException("Invalid or expired refresh token.");

        if (!user.IsActive)
            throw new UnauthorizedAccessException("Account is deactivated.");

        // Rotate refresh token
        user.RefreshToken          = _jwtService.GenerateRefreshToken();
        user.RefreshTokenExpiresAt = DateTime.UtcNow.AddDays(7);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Refresh token rotated for user {Email}.", user.Email);
        return BuildAuthResponse(user);
    }

    // ── Revoke Token ───────────────────────────────────────────────────────────
    public async Task RevokeTokenAsync(string refreshToken,
        CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByRefreshTokenAsync(refreshToken, cancellationToken);
        if (user is null) return;

        user.RefreshToken          = null;
        user.RefreshTokenExpiresAt = null;
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Refresh token revoked for user {Email}.", user.Email);
    }

    // ── Get Profile ────────────────────────────────────────────────────────────
    public async Task<UserDto> GetProfileAsync(Guid userId,
        CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken)
            ?? throw new KeyNotFoundException($"User with id '{userId}' was not found.");

        return _mapper.Map<UserDto>(user);
    }

    // ── Private Helpers ────────────────────────────────────────────────────────
    private AuthResponseDto BuildAuthResponse(User user) => new()
    {
        UserId               = user.Id,
        FullName             = user.FullName,
        Email                = user.Email,
        Role                 = user.Role.ToString(),
        AccessToken          = _jwtService.GenerateAccessToken(user),
        RefreshToken         = user.RefreshToken!,
        AccessTokenExpiresAt = _jwtService.GetAccessTokenExpiry(),
        RefreshTokenExpiresAt = user.RefreshTokenExpiresAt!.Value
    };
}
