namespace AssignmentManagement.Api.Application.DTOs.Auth;

/// <summary>Request DTO for refreshing an access token.</summary>
public class RefreshTokenRequestDto
{
    public string RefreshToken { get; set; } = string.Empty;
}
