using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AssignmentManagement.Api.Domain.Entities;

namespace AssignmentManagement.Api.Infrastructure.Configurations;

/// <summary>EF Core configuration for Assignment entity.</summary>
public class AssignmentConfiguration : IEntityTypeConfiguration<Assignment>
{
    public void Configure(EntityTypeBuilder<Assignment> builder)
    {
        builder.ToTable("Assignments");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(a => a.Description)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(a => a.DueDate)
            .IsRequired();

        builder.Property(a => a.TotalMarks)
            .IsRequired()
            .HasPrecision(8, 2);

        builder.Property(a => a.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        // ── Relationships ─────────────────────────────────────────────────────

        // Teacher (User) → Assignments — restrict delete (preserve history)
        builder.HasOne(a => a.Teacher)
            .WithMany(u => u.Assignments)
            .HasForeignKey(a => a.TeacherId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_Assignments_Users_TeacherId");

        // Subject → Assignments — restrict delete
        builder.HasOne(a => a.Subject)
            .WithMany(s => s.Assignments)
            .HasForeignKey(a => a.SubjectId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_Assignments_Subjects_SubjectId");

        // SchoolClass → Assignments — restrict delete
        builder.HasOne(a => a.Class)
            .WithMany(c => c.Assignments)
            .HasForeignKey(a => a.ClassId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_Assignments_Classes_ClassId");

        // Submissions — cascade delete
        builder.HasMany(a => a.Submissions)
            .WithOne(s => s.Assignment)
            .HasForeignKey(s => s.AssignmentId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_Submissions_Assignments_AssignmentId");
    }
}
