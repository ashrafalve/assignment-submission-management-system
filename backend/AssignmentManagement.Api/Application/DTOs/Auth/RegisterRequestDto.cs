namespace AssignmentManagement.Api.Application.DTOs.Auth;

/// <summary>Request DTO for registering a new user.</summary>
public class RegisterRequestDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;

    /// <summary>
    /// Role name as string: "Admin", "Teacher", or "Student".
    /// Defaults to "Student" if not supplied.
    /// </summary>
    public string Role { get; set; } = "Student";
    public Guid? ClassId { get; set; }
}
