using AssignmentManagement.Api.Application.DTOs.Teacher;
using AssignmentManagement.Api.Domain.Enums;

namespace AssignmentManagement.Api.Application.DTOs.Student;

public class SubmitAssignmentDto
{
    public Guid    AssignmentId { get; set; }
    public string? Content      { get; set; }
    public string? FilePath     { get; set; }
}

public class UpdateSubmissionDto
{
    public string? Content  { get; set; }
    public string? FilePath { get; set; }
}

public class SubmissionDto
{
    public Guid             Id              { get; set; }
    public Guid             AssignmentId    { get; set; }
    public string           AssignmentTitle { get; set; } = string.Empty;
    public Guid             StudentId       { get; set; }
    public string           StudentName     { get; set; } = string.Empty;
    public string?          Content         { get; set; }
    public string?          FilePath        { get; set; }
    public SubmissionStatus Status          { get; set; }
    public DateTime?        SubmittedAt     { get; set; }
    public decimal?         MarksObtained   { get; set; }
    public string?          Feedback        { get; set; }
    public DateTime?        GradedAt        { get; set; }
    public DateTime         DueDate         { get; set; }
    public decimal          TotalMarks      { get; set; }
}

public class StudentAssignmentDetailDto
{
    public AssignmentDto   Assignment { get; set; } = null!;
    public SubmissionDto?  Submission { get; set; }
}
