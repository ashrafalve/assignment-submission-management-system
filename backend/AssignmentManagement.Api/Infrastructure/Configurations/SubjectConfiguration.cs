using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AssignmentManagement.Api.Domain.Entities;

namespace AssignmentManagement.Api.Infrastructure.Configurations;

/// <summary>EF Core configuration for Subject entity.</summary>
public class SubjectConfiguration : IEntityTypeConfiguration<Subject>
{
    public void Configure(EntityTypeBuilder<Subject> builder)
    {
        builder.ToTable("Subjects");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(s => s.Code)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(s => s.Description)
            .HasMaxLength(500);

        builder.Property(s => s.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        // Unique subject code
        builder.HasIndex(s => s.Code)
            .IsUnique()
            .HasDatabaseName("IX_Subjects_Code");
    }
}
