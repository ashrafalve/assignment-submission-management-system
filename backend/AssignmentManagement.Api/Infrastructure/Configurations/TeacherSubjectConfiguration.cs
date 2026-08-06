using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AssignmentManagement.Api.Domain.Entities;

namespace AssignmentManagement.Api.Infrastructure.Configurations;

/// <summary>EF Core configuration for TeacherSubject junction entity.</summary>
public class TeacherSubjectConfiguration : IEntityTypeConfiguration<TeacherSubject>
{
    public void Configure(EntityTypeBuilder<TeacherSubject> builder)
    {
        builder.ToTable("TeacherSubjects");

        builder.HasKey(ts => ts.Id);

        builder.Property(ts => ts.AssignedAt)
            .IsRequired();

        builder.Property(ts => ts.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        // ── Unique: one teacher teaches one subject in one class ──────────────
        builder.HasIndex(ts => new { ts.SubjectId, ts.ClassId })
            .IsUnique()
            .HasDatabaseName("IX_TeacherSubjects_Subject_Class");

        // ── Relationships ─────────────────────────────────────────────────────

        // Teacher (User) → TeacherSubjects — cascade delete
        builder.HasOne(ts => ts.Teacher)
            .WithMany(u => u.TeacherSubjects)
            .HasForeignKey(ts => ts.TeacherId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_TeacherSubjects_Users_TeacherId");

        // Subject → TeacherSubjects — cascade delete
        builder.HasOne(ts => ts.Subject)
            .WithMany(s => s.TeacherSubjects)
            .HasForeignKey(ts => ts.SubjectId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_TeacherSubjects_Subjects_SubjectId");

        // SchoolClass → TeacherSubjects — cascade delete
        builder.HasOne(ts => ts.Class)
            .WithMany(c => c.TeacherSubjects)
            .HasForeignKey(ts => ts.ClassId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_TeacherSubjects_Classes_ClassId");
    }
}
