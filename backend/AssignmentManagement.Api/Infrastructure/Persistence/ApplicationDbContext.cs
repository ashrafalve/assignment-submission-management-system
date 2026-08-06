using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;
using AssignmentManagement.Api.Domain.Entities;
using AssignmentManagement.Api.Domain.Enums;

namespace AssignmentManagement.Api.Infrastructure.Persistence;

/// <summary>
/// Application database context managing all entity sets.
/// </summary>
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // ── DbSets ────────────────────────────────────────────────────────────────
    public DbSet<User>           Users           => Set<User>();
    public DbSet<SchoolClass>    Classes         => Set<SchoolClass>();
    public DbSet<Subject>        Subjects        => Set<Subject>();
    public DbSet<TeacherSubject> TeacherSubjects => Set<TeacherSubject>();
    public DbSet<Assignment>     Assignments     => Set<Assignment>();
    public DbSet<Submission>     Submissions     => Set<Submission>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all entity configurations from Infrastructure.Configurations
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        // ── Global soft-delete query filter ───────────────────────────────────
        // Applied to every entity that inherits from BaseEntity.
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (!typeof(BaseEntity).IsAssignableFrom(entityType.ClrType)) continue;

            // Build: e => !e.IsDeleted  (with correct parameter reference)
            var param     = Expression.Parameter(entityType.ClrType, "e");
            var property  = Expression.Property(param, nameof(BaseEntity.IsDeleted));
            var condition = Expression.Not(property);
            var lambda    = Expression.Lambda(condition, param);

            modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
        }

        // ── Seed Data ─────────────────────────────────────────────────────────
        SeedData(modelBuilder);
    }

    /// <summary>
    /// Overrides SaveChangesAsync to automatically set audit timestamps
    /// and intercept hard deletes to convert them to soft deletes.
    /// </summary>
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker.Entries<BaseEntity>();

        foreach (var entry in entries)
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    break;
                case EntityState.Modified:
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    break;
                case EntityState.Deleted:
                    // Convert hard delete to soft delete
                    entry.State            = EntityState.Modified;
                    entry.Entity.IsDeleted = true;
                    entry.Entity.DeletedAt = DateTime.UtcNow;
                    break;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }

    // ── Seed ──────────────────────────────────────────────────────────────────
    private static void SeedData(ModelBuilder modelBuilder)
    {
        // Seed default Admin user
        var adminId = new Guid("00000000-0000-0000-0000-000000000001");

        modelBuilder.Entity<User>().HasData(new User
        {
            Id           = adminId,
            FirstName    = "System",
            LastName     = "Admin",
            Email        = "admin@assignmentmanagement.com",
            PasswordHash = "$2a$12$K.7xX9k8n0uQ1v7a8b9c0uS1v2w3x4y5z6a7b8c9d0e1f2g3h4i",
            Role         = UserRole.Admin,
            IsActive     = true,
            CreatedAt    = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        });

        // Seed default Subjects
        var mathId    = new Guid("00000000-0000-0000-0000-000000000010");
        var scienceId = new Guid("00000000-0000-0000-0000-000000000011");
        var englishId = new Guid("00000000-0000-0000-0000-000000000012");

        modelBuilder.Entity<Subject>().HasData(
            new Subject { Id = mathId,    Name = "Mathematics", Code = "MATH-101", IsActive = true,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Subject { Id = scienceId, Name = "Science",     Code = "SCI-101",  IsActive = true,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Subject { Id = englishId, Name = "English",     Code = "ENG-101",  IsActive = true,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
        );

        // Seed default Classes
        var class10AId = new Guid("00000000-0000-0000-0000-000000000020");
        var class10BId = new Guid("00000000-0000-0000-0000-000000000021");

        modelBuilder.Entity<SchoolClass>().HasData(
            new SchoolClass { Id = class10AId, Name = "Grade 10 - Section A",
                AcademicYear = "2026-2027", IsActive = true,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new SchoolClass { Id = class10BId, Name = "Grade 10 - Section B",
                AcademicYear = "2026-2027", IsActive = true,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
        );
    }
}
