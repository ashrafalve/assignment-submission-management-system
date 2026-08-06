namespace AssignmentManagement.Api.Application.DTOs.Admin;

public class CreateSubjectDto
{
    public string  Name        { get; set; } = string.Empty;
    public string  Code        { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class UpdateSubjectDto
{
    public string?  Name        { get; set; }
    public string?  Code        { get; set; }
    public string?  Description { get; set; }
    public bool?    IsActive    { get; set; }
}

public class SubjectDto
{
    public Guid    Id          { get; set; }
    public string  Name        { get; set; } = string.Empty;
    public string  Code        { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool    IsActive    { get; set; }
    public DateTime CreatedAt  { get; set; }
}
