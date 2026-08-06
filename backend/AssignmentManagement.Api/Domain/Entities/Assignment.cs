using AssignmentManagement.Api.Domain.Enums;

namespace AssignmentManagement.Api.Domain.Entities;

/// <summary>
/// Represents an assignment created by a teacher for a subject and class.
/// </summary>
public class Assignment : BaseEntity
{
    public string Title       { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime DueDate   { get; set; }
    public decimal TotalMarks { get; set; }
    public AssignmentStatus Status { get; set; } = AssignmentStatus.Draft;

    // ── Foreign Keys ──────────────────────────────────────────────────────────
    public Guid TeacherId { get; set; }
    public Guid SubjectId { get; set; }
    public Guid ClassId   { get; set; }

    // ── Navigation ────────────────────────────────────────────────────────────
    public User        Teacher     { get; set; } = null!;
    public Subject     Subject     { get; set; } = null!;
    public SchoolClass Class       { get; set; } = null!;
    public ICollection<Submission> Submissions { get; set; } = [];
}
