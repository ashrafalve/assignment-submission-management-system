namespace AssignmentManagement.Api.Domain.Enums;

/// <summary>Lifecycle status of a student submission.</summary>
public enum SubmissionStatus
{
    Pending   = 1,   // Not yet submitted
    Submitted = 2,
    Late      = 3,
    Graded    = 4,
    Rejected  = 5
}
