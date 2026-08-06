using AssignmentManagement.Api.Domain.Enums;

namespace AssignmentManagement.Api.Domain.Entities;

/// <summary>
/// Represents a student's submission in response to an assignment.
/// </summary>
public class Submission : BaseEntity
{
    // ── Foreign Keys ──────────────────────────────────────────────────────────
    public Guid AssignmentId { get; set; }
    public Guid StudentId    { get; set; }

    // ── Content ───────────────────────────────────────────────────────────────
    public string? Content  { get; set; }    // Text submission
    public string? FilePath { get; set; }    // File attachment path

    // ── Status & Grading ──────────────────────────────────────────────────────
    public SubmissionStatus Status         { get; set; } = SubmissionStatus.Pending;
    public DateTime?        SubmittedAt    { get; set; }
    public decimal?         MarksObtained  { get; set; }
    public string?          Feedback       { get; set; }
    public DateTime?        GradedAt       { get; set; }

    // ── Navigation ────────────────────────────────────────────────────────────
    public Assignment Assignment { get; set; } = null!;
    public User       Student    { get; set; } = null!;
}
