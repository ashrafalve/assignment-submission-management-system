namespace AssignmentManagement.Api.Application.DTOs.Admin;

public class CreateUserDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName  { get; set; } = string.Empty;
    public string Email     { get; set; } = string.Empty;
    public string Password  { get; set; } = string.Empty;
    public string Role      { get; set; } = "Student";
    public Guid?  ClassId   { get; set; }
}

public class UpdateUserDto
{
    public string? FirstName { get; set; }
    public string? LastName  { get; set; }
    public string? Role      { get; set; }
    public bool?   IsActive  { get; set; }
    public Guid?   ClassId   { get; set; }
}

public class ChangePasswordDto
{
    public string NewPassword     { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
}

public class UserListItemDto
{
    public Guid     Id          { get; set; }
    public string   FullName    { get; set; } = string.Empty;
    public string   Email       { get; set; } = string.Empty;
    public string   Role        { get; set; } = string.Empty;
    public Guid?    ClassId     { get; set; }
    public string?  ClassName   { get; set; }
    public bool     IsActive    { get; set; }
    public DateTime CreatedAt   { get; set; }
    public DateTime? LastLoginAt { get; set; }
}
