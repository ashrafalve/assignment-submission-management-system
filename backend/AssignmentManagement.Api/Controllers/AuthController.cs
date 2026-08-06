using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using AssignmentManagement.Api.Application.DTOs.Auth;
using AssignmentManagement.Api.Application.Interfaces;
using AssignmentManagement.Api.Shared;

namespace AssignmentManagement.Api.Controllers;

/// <summary>
/// Handles user authentication: registration, login, token refresh, and profile.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Tags("Authentication")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger      = logger;
    }

    // ── POST /api/auth/register ────────────────────────────────────────────────
    /// <summary>Registers a new user account.</summary>
    /// <response code="201">User registered successfully.</response>
    /// <response code="400">Validation error or email already in use.</response>
    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>),          StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register(
        [FromBody] RegisterRequestDto request,
        CancellationToken cancellationToken)
    {
        var result = await _authService.RegisterAsync(request, cancellationToken);
        return StatusCode(StatusCodes.Status201Created,
            ApiResponse<AuthResponseDto>.Ok(result, "Account created successfully.", 201));
    }

    // ── POST /api/auth/login ───────────────────────────────────────────────────
    /// <summary>Authenticates a user and returns JWT access + refresh tokens.</summary>
    /// <response code="200">Login successful.</response>
    /// <response code="401">Invalid credentials or inactive account.</response>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>),          StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login(
        [FromBody] LoginRequestDto request,
        CancellationToken cancellationToken)
    {
        var result = await _authService.LoginAsync(request, cancellationToken);
        return Ok(ApiResponse<AuthResponseDto>.Ok(result, "Login successful."));
    }

    // ── POST /api/auth/refresh ─────────────────────────────────────────────────
    /// <summary>Exchanges a valid refresh token for a new access token pair.</summary>
    /// <response code="200">Tokens refreshed successfully.</response>
    /// <response code="401">Invalid or expired refresh token.</response>
    [HttpPost("refresh")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>),          StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RefreshToken(
        [FromBody] RefreshTokenRequestDto request,
        CancellationToken cancellationToken)
    {
        var result = await _authService.RefreshTokenAsync(request.RefreshToken, cancellationToken);
        return Ok(ApiResponse<AuthResponseDto>.Ok(result, "Tokens refreshed successfully."));
    }

    // ── POST /api/auth/revoke ──────────────────────────────────────────────────
    /// <summary>Revokes a refresh token (logout).</summary>
    /// <response code="200">Token revoked successfully.</response>
    [HttpPost("revoke")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> RevokeToken(
        [FromBody] RefreshTokenRequestDto request,
        CancellationToken cancellationToken)
    {
        await _authService.RevokeTokenAsync(request.RefreshToken, cancellationToken);
        return Ok(ApiResponse<object?>.Ok(null, "Token revoked successfully."));
    }

    // ── GET /api/auth/me ───────────────────────────────────────────────────────
    /// <summary>Returns the authenticated user's profile.</summary>
    /// <response code="200">Profile retrieved.</response>
    /// <response code="401">Not authenticated.</response>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>),  StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetProfile(CancellationToken cancellationToken)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);

        if (!Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized(ApiResponse<object>.Fail("Invalid token claims.", statusCode: 401));

        var profile = await _authService.GetProfileAsync(userId, cancellationToken);
        return Ok(ApiResponse<UserDto>.Ok(profile, "Profile retrieved successfully."));
    }

    // ── GET /api/auth/admin-only ───────────────────────────────────────────────
    /// <summary>Admin-only test endpoint demonstrating role-based authorization.</summary>
    /// <response code="200">Authorized as Admin.</response>
    /// <response code="403">Forbidden — not an Admin.</response>
    [HttpGet("admin-only")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    public IActionResult AdminOnly()
        => Ok(ApiResponse<object>.Ok(new { message = "Welcome, Admin!" }, "Admin access granted."));

    // ── GET /api/auth/teacher-only ─────────────────────────────────────────────
    /// <summary>Teacher/Admin endpoint demonstrating role-based authorization.</summary>
    [HttpGet("teacher-only")]
    [Authorize(Roles = "Admin,Teacher")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    public IActionResult TeacherOnly()
        => Ok(ApiResponse<object>.Ok(new { message = "Welcome, Teacher or Admin!" }, "Access granted."));
}

// Alias for JWT claim name constant
file static class JwtRegisteredClaimNames
{
    public const string Sub = "sub";
}
