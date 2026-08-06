namespace AssignmentManagement.Api.Domain.Entities;

/// <summary>
/// Represents a school class or grade section (e.g., "Grade 10 - Section A").
/// </summary>
public class SchoolClass : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string AcademicYear { get; set; } = string.Empty;  // e.g., "2025-2026"
    public bool IsActive { get; set; } = true;

    // ── Navigation ────────────────────────────────────────────────────────────
    public ICollection<TeacherSubject> TeacherSubjects { get; set; } = [];
    public ICollection<Assignment>     Assignments      { get; set; } = [];
}
