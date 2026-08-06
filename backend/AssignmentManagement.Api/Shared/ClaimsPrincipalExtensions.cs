using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace AssignmentManagement.Api.Shared;

public static class ClaimsPrincipalExtensions
{
    /// <summary>
    /// Extracts the authenticated User ID from claims (NameIdentifier or Sub).
    /// Throws UnauthorizedAccessException if claim is missing or not a valid Guid.
    /// </summary>
    public static Guid GetUserId(this ClaimsPrincipal principal)
    {
        var idClaim = principal.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? principal.FindFirstValue(JwtRegisteredClaimNames.Sub);

        if (!Guid.TryParse(idClaim, out var userId))
        {
            throw new UnauthorizedAccessException("User identity claim is missing or invalid in token.");
        }

        return userId;
    }
}
