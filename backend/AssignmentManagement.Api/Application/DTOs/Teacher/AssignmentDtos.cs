using AssignmentManagement.Api.Domain.Enums;

namespace AssignmentManagement.Api.Application.DTOs.Teacher;

public class CreateAssignmentDto
{
    public string   Title       { get; set; } = string.Empty;
    public string   Description { get; set; } = string.Empty;
    public DateTime DueDate     { get; set; }
    public decimal  TotalMarks  { get; set; }
    public Guid     ClassId     { get; set; }
    public Guid     SubjectId   { get; set; }
    public bool     PublishNow  { get; set; } = false;
}

public class UpdateAssignmentDto
{
    public string?   Title       { get; set; }
    public string?   Description { get; set; }
    public DateTime? DueDate     { get; set; }
    public decimal?  TotalMarks  { get; set; }
    public Guid?     ClassId     { get; set; }
    public Guid?     SubjectId   { get; set; }
}

public class AssignmentDto
{
    public Guid             Id          { get; set; }
    public string           Title       { get; set; } = string.Empty;
    public string           Description { get; set; } = string.Empty;
    public DateTime         DueDate     { get; set; }
    public decimal          TotalMarks  { get; set; }
    public AssignmentStatus Status      { get; set; }
    public Guid             TeacherId   { get; set; }
    public string           TeacherName { get; set; } = string.Empty;
    public Guid             SubjectId   { get; set; }
    public string           SubjectName { get; set; } = string.Empty;
    public string           SubjectCode { get; set; } = string.Empty;
    public Guid             ClassId     { get; set; }
    public string           ClassName   { get; set; } = string.Empty;
    public DateTime         CreatedAt   { get; set; }
    public DateTime?        UpdatedAt   { get; set; }
}
