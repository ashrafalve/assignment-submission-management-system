using AssignmentManagement.Api.Domain.Entities;

namespace AssignmentManagement.Api.Domain.Interfaces;

/// <summary>
/// User-specific repository operations beyond generic CRUD.
/// </summary>
public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default);
    Task<User?> GetByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
}
