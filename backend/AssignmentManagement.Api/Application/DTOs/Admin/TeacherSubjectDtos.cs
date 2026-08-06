namespace AssignmentManagement.Api.Application.DTOs.Admin;

public class AssignTeacherDto
{
    public Guid TeacherId { get; set; }
    public Guid SubjectId { get; set; }
    public Guid ClassId   { get; set; }
}

public class TeacherSubjectDto
{
    public Guid     Id          { get; set; }
    public Guid     TeacherId   { get; set; }
    public string   TeacherName { get; set; } = string.Empty;
    public Guid     SubjectId   { get; set; }
    public string   SubjectName { get; set; } = string.Empty;
    public string   SubjectCode { get; set; } = string.Empty;
    public Guid     ClassId     { get; set; }
    public string   ClassName   { get; set; } = string.Empty;
    public DateTime AssignedAt  { get; set; }
    public bool     IsActive    { get; set; }
}
