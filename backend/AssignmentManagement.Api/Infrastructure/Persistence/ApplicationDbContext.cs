using Microsoft.EntityFrameworkCore;
using AssignmentManagement.Api.Domain.Entities;

namespace AssignmentManagement.Api.Infrastructure.Persistence;

/// <summary>
/// Application database context. All DbSet properties will be added
/// as business entities are introduced in subsequent steps.
/// </summary>
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all entity configurations from the Infrastructure.Configurations folder
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        // Global soft-delete query filter applied to all BaseEntity-derived types
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
            {
                modelBuilder.Entity(entityType.ClrType)
                    .HasQueryFilter(
                        System.Linq.Expressions.Expression.Lambda(
                            System.Linq.Expressions.Expression.Not(
                                System.Linq.Expressions.Expression.Property(
                                    System.Linq.Expressions.Expression.Parameter(entityType.ClrType, "e"),
                                    nameof(BaseEntity.IsDeleted)
                                )
                            ),
                            System.Linq.Expressions.Expression.Parameter(entityType.ClrType, "e")
                        )
                    );
            }
        }
    }

    /// <summary>
    /// Overrides SaveChangesAsync to automatically set audit timestamps.
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
                    entry.State = EntityState.Modified;
                    entry.Entity.IsDeleted = true;
                    entry.Entity.DeletedAt = DateTime.UtcNow;
                    break;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}
