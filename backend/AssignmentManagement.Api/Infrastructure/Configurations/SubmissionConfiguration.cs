using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AssignmentManagement.Api.Domain.Entities;

namespace AssignmentManagement.Api.Infrastructure.Configurations;

/// <summary>EF Core configuration for Submission entity.</summary>
public class SubmissionConfiguration : IEntityTypeConfiguration<Submission>
{
    public void Configure(EntityTypeBuilder<Submission> builder)
    {
        builder.ToTable("Submissions");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Content)
            .HasMaxLength(5000);

        builder.Property(s => s.FilePath)
            .HasMaxLength(500);

        builder.Property(s => s.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(s => s.MarksObtained)
            .HasPrecision(8, 2);

        builder.Property(s => s.Feedback)
            .HasMaxLength(1000);

        // Unique: one submission per student per assignment
        builder.HasIndex(s => new { s.AssignmentId, s.StudentId })
            .IsUnique()
            .HasDatabaseName("IX_Submissions_Assignment_Student");

        // ── Relationships ─────────────────────────────────────────────────────

        // Student (User) → Submissions — restrict delete (preserve grading records)
        builder.HasOne(s => s.Student)
            .WithMany(u => u.Submissions)
            .HasForeignKey(s => s.StudentId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_Submissions_Users_StudentId");
        // Note: Assignment → Submissions cascade is configured in AssignmentConfiguration
    }
}
