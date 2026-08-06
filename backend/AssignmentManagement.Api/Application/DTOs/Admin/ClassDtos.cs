namespace AssignmentManagement.Api.Application.DTOs.Admin;

public class CreateClassDto
{
    public string  Name         { get; set; } = string.Empty;
    public string? Description  { get; set; }
    public string  AcademicYear { get; set; } = string.Empty;
}

public class UpdateClassDto
{
    public string?  Name         { get; set; }
    public string?  Description  { get; set; }
    public string?  AcademicYear { get; set; }
    public bool?    IsActive     { get; set; }
}

public class ClassDto
{
    public Guid    Id           { get; set; }
    public string  Name         { get; set; } = string.Empty;
    public string? Description  { get; set; }
    public string  AcademicYear { get; set; } = string.Empty;
    public bool    IsActive     { get; set; }
    public DateTime CreatedAt   { get; set; }
    public int     StudentCount { get; set; }
}
