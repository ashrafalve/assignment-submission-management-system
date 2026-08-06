using System.Security.Claims;
using AssignmentManagement.Api.Domain.Entities;

namespace AssignmentManagement.Api.Application.Interfaces;

/// <summary>
/// JWT token generation and validation service contract.
/// </summary>
public interface IJwtService
{
    /// <summary>Generates a signed JWT access token for the given user.</summary>
    string GenerateAccessToken(User user);

    /// <summary>Generates a cryptographically secure refresh token string.</summary>
    string GenerateRefreshToken();

    /// <summary>
    /// Reads claims from an expired (or valid) token without validating lifetime.
    /// Used during refresh token flow.
    /// </summary>
    ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);

    /// <summary>Returns the UTC expiry time for a newly generated access token.</summary>
    DateTime GetAccessTokenExpiry();
}
