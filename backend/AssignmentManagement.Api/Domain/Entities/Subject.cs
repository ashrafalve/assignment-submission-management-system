namespace AssignmentManagement.Api.Domain.Entities;

/// <summary>
/// Represents an academic subject or course (e.g., "Mathematics", "MATH-101").
/// </summary>
public class Subject : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;      // Unique subject code
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;

    // ── Navigation ────────────────────────────────────────────────────────────
    public ICollection<TeacherSubject> TeacherSubjects { get; set; } = [];
    public ICollection<Assignment>     Assignments      { get; set; } = [];
}
