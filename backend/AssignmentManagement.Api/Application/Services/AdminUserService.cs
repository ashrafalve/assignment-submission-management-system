using AutoMapper;
using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using AssignmentManagement.Api.Application.DTOs.Admin;
using AssignmentManagement.Api.Application.DTOs.Auth;
using AssignmentManagement.Api.Application.Interfaces;
using AssignmentManagement.Api.Domain.Entities;
using AssignmentManagement.Api.Domain.Enums;
using AssignmentManagement.Api.Domain.Interfaces;
using AssignmentManagement.Api.Infrastructure.Persistence;
using AssignmentManagement.Api.Shared;

namespace AssignmentManagement.Api.Application.Services;

/// <summary>
/// Admin service for complete user lifecycle management.
/// </summary>
public class AdminUserService : IAdminUserService
{
    private readonly ApplicationDbContext _context;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<AdminUserService> _logger;

    public AdminUserService(
        ApplicationDbContext context,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<AdminUserService> logger)
    {
        _context        = context;
        _userRepository = userRepository;
        _unitOfWork     = unitOfWork;
        _mapper         = mapper;
        _logger         = logger;
    }

    public async Task<PagedResponse<UserListItemDto>> GetUsersAsync(
        PaginationParams pagination, string? role = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Users.AsNoTracking();

        // Filter by role
        if (!string.IsNullOrWhiteSpace(role) &&
            Enum.TryParse<UserRole>(role, ignoreCase: true, out var parsedRole))
        {
            query = query.Where(u => u.Role == parsedRole);
        }

        // Search
        if (!string.IsNullOrWhiteSpace(pagination.SearchTerm))
        {
            var term = pagination.SearchTerm.ToLower();
            query = query.Where(u =>
                u.FirstName.ToLower().Contains(term) ||
                u.LastName.ToLower().Contains(term)  ||
                u.Email.ToLower().Contains(term));
        }

        // Sort
        query = pagination.SortBy?.ToLower() switch
        {
            "email"     => pagination.SortDescending ? query.OrderByDescending(u => u.Email)     : query.OrderBy(u => u.Email),
            "role"      => pagination.SortDescending ? query.OrderByDescending(u => u.Role)      : query.OrderBy(u => u.Role),
            "createdat" => pagination.SortDescending ? query.OrderByDescending(u => u.CreatedAt) : query.OrderBy(u => u.CreatedAt),
            _           => query.OrderBy(u => u.LastName).ThenBy(u => u.FirstName)
        };

        var totalCount = await query.CountAsync(cancellationToken);

        var users = await query
            .Skip((pagination.PageNumber - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .ToListAsync(cancellationToken);

        return PagedResponse<UserListItemDto>.Create(
            _mapper.Map<IEnumerable<UserListItemDto>>(users),
            totalCount,
            pagination.PageNumber,
            pagination.PageSize);
    }

    public async Task<UserDto> GetUserByIdAsync(Guid id,
        CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException($"User with id '{id}' was not found.");
        return _mapper.Map<UserDto>(user);
    }

    public async Task<UserDto> CreateUserAsync(CreateUserDto dto,
        CancellationToken cancellationToken = default)
    {
        if (await _userRepository.EmailExistsAsync(dto.Email, cancellationToken))
            throw new InvalidOperationException($"Email '{dto.Email}' is already in use.");

        if (!Enum.TryParse<UserRole>(dto.Role, ignoreCase: true, out var role))
            throw new ArgumentException($"Invalid role: {dto.Role}.");

        var user = new User
        {
            FirstName    = dto.FirstName.Trim(),
            LastName     = dto.LastName.Trim(),
            Email        = dto.Email.Trim().ToLowerInvariant(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password, workFactor: 12),
            Role         = role,
            ClassId      = dto.ClassId,
            IsActive     = true
        };

        await _userRepository.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Admin created user {Email} with role {Role}.", user.Email, user.Role);
        return _mapper.Map<UserDto>(user);
    }

    public async Task<UserDto> UpdateUserAsync(Guid id, UpdateUserDto dto,
        CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException($"User with id '{id}' was not found.");

        if (dto.FirstName is not null) user.FirstName = dto.FirstName.Trim();
        if (dto.LastName  is not null) user.LastName  = dto.LastName.Trim();
        if (dto.IsActive  is not null) user.IsActive  = dto.IsActive.Value;
        if (dto.ClassId   is not null) user.ClassId   = dto.ClassId.Value;

        if (dto.Role is not null)
        {
            if (!Enum.TryParse<UserRole>(dto.Role, ignoreCase: true, out var role))
                throw new ArgumentException($"Invalid role: {dto.Role}.");
            user.Role = role;
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Admin updated user {UserId}.", id);
        return _mapper.Map<UserDto>(user);
    }

    public async Task ChangePasswordAsync(Guid id, ChangePasswordDto dto,
        CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException($"User with id '{id}' was not found.");

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword, workFactor: 12);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Admin changed password for user {UserId}.", id);
    }

    public async Task DeleteUserAsync(Guid id,
        CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException($"User with id '{id}' was not found.");

        // Prevent deleting the last admin
        if (user.Role == UserRole.Admin)
        {
            var adminCount = await _context.Users
                .CountAsync(u => u.Role == UserRole.Admin, cancellationToken);
            if (adminCount <= 1)
                throw new InvalidOperationException("Cannot delete the last Admin account.");
        }

        await _userRepository.DeleteAsync(id, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Admin soft-deleted user {UserId}.", id);
    }
}
