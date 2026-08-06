using AssignmentManagement.Api.Application.DTOs.Auth;

namespace AssignmentManagement.Api.Application.Interfaces;

/// <summary>
/// Authentication service contract for login, registration, and token management.
/// </summary>
public interface IAuthService
{
    Task<AuthResponseDto> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default);
    Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request, CancellationToken cancellationToken = default);
    Task<AuthResponseDto> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
    Task RevokeTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
    Task<UserDto> GetProfileAsync(Guid userId, CancellationToken cancellationToken = default);
}
