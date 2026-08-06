using AssignmentManagement.Api.Domain.Enums;

namespace AssignmentManagement.Api.Application.DTOs.Teacher;

public class GradeSubmissionDto
{
    public decimal          MarksObtained { get; set; }
    public string?          Feedback      { get; set; }
    public SubmissionStatus Status        { get; set; } = SubmissionStatus.Graded;
}

public class ChangeSubmissionStatusDto
{
    public SubmissionStatus Status   { get; set; }
    public string?          Feedback { get; set; }
}
