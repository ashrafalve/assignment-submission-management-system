using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AssignmentManagement.Api.Domain.Entities;
using AssignmentManagement.Api.Domain.Enums;

namespace AssignmentManagement.Api.Infrastructure.Configurations;

/// <summary>
/// EF Core type configuration for the User entity.
/// </summary>
public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.FirstName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(u => u.LastName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(256);

        // Unique index on email (case-insensitive handled at app layer)
        builder.HasIndex(u => u.Email)
            .IsUnique()
            .HasDatabaseName("IX_Users_Email");

        builder.Property(u => u.PasswordHash)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(u => u.Role)
            .IsRequired()
            .HasConversion<string>()    // Store as string for readability
            .HasMaxLength(20);

        builder.Property(u => u.RefreshToken)
            .HasMaxLength(500);

        builder.Property(u => u.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(u => u.CreatedAt)
            .IsRequired();

        builder.Property(u => u.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        // Class relationship (optional for Admin/Teacher, used for Student)
        builder.HasOne(u => u.Class)
            .WithMany()
            .HasForeignKey(u => u.ClassId)
            .OnDelete(DeleteBehavior.SetNull)
            .HasConstraintName("FK_Users_Classes_ClassId");

        // Ignore computed property
        builder.Ignore(u => u.FullName);
    }
}
