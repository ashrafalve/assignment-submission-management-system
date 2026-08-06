using Microsoft.EntityFrameworkCore;
using AssignmentManagement.Api.Domain.Entities;
using AssignmentManagement.Api.Domain.Interfaces;
using AssignmentManagement.Api.Infrastructure.Persistence;

namespace AssignmentManagement.Api.Infrastructure.Repositories;

/// <summary>
/// User-specific repository providing email and refresh token lookups.
/// </summary>
public class UserRepository : GenericRepository<User>, IUserRepository
{
    public UserRepository(ApplicationDbContext context) : base(context)
    {
    }

    public override async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _dbSet.Include(u => u.Class).FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

    public async Task<User?> GetByEmailAsync(string email,
        CancellationToken cancellationToken = default)
        => await _dbSet
            .Include(u => u.Class)
            .FirstOrDefaultAsync(u => u.Email == email.ToLowerInvariant(), cancellationToken);

    public async Task<bool> EmailExistsAsync(string email,
        CancellationToken cancellationToken = default)
        => await _dbSet
            .AnyAsync(u => u.Email == email.ToLowerInvariant(), cancellationToken);

    public async Task<User?> GetByRefreshTokenAsync(string refreshToken,
        CancellationToken cancellationToken = default)
        => await _dbSet
            .Include(u => u.Class)
            .FirstOrDefaultAsync(u => u.RefreshToken == refreshToken, cancellationToken);
}
