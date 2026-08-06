namespace AssignmentManagement.Api.Domain.Entities;

/// <summary>
/// Junction entity linking a Teacher to a Subject within a specific SchoolClass.
/// One teacher teaches one subject in one class (unique constraint enforced).
/// </summary>
public class TeacherSubject : BaseEntity
{
    // ── Foreign Keys ──────────────────────────────────────────────────────────
    public Guid TeacherId { get; set; }
    public Guid SubjectId { get; set; }
    public Guid ClassId   { get; set; }

    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;

    // ── Navigation ────────────────────────────────────────────────────────────
    public User        Teacher { get; set; } = null!;
    public Subject     Subject { get; set; } = null!;
    public SchoolClass Class   { get; set; } = null!;
}
