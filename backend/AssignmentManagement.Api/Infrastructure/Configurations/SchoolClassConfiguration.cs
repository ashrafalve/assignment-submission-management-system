using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AssignmentManagement.Api.Domain.Entities;

namespace AssignmentManagement.Api.Infrastructure.Configurations;

/// <summary>EF Core configuration for SchoolClass entity.</summary>
public class SchoolClassConfiguration : IEntityTypeConfiguration<SchoolClass>
{
    public void Configure(EntityTypeBuilder<SchoolClass> builder)
    {
        builder.ToTable("Classes");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.AcademicYear)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(c => c.Description)
            .HasMaxLength(500);

        builder.Property(c => c.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        // Unique: class name per academic year
        builder.HasIndex(c => new { c.Name, c.AcademicYear })
            .IsUnique()
            .HasDatabaseName("IX_Classes_Name_AcademicYear");
    }
}
