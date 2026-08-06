using AssignmentManagement.Api.Domain.Enums;

namespace AssignmentManagement.Api.Domain.Entities;

/// <summary>
/// Represents a system user (Admin, Teacher, or Student).
/// </summary>
public class User : BaseEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.Student;
    public bool IsActive { get; set; } = true;
    public DateTime? LastLoginAt { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiresAt { get; set; }

    /// <summary>Full name computed from first + last name.</summary>
    public string FullName => $"{FirstName} {LastName}".Trim();
}
