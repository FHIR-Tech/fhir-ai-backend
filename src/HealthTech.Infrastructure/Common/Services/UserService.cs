using HealthTech.Application.Common.Interfaces;
using HealthTech.Domain.Entities;
using HealthTech.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text;

namespace HealthTech.Infrastructure.Common.Services;

/// <summary>
/// User service implementation
/// </summary>
public class UserService : IUserService
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<UserService> _logger;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="context">Application database context</param>
    /// <param name="logger">Logger</param>
    public UserService(IApplicationDbContext context, ILogger<UserService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Get user by ID
    /// </summary>
    /// <param name="id">User ID</param>
    /// <param name="tenantId">Tenant ID</param>
    /// <returns>User or null if not found</returns>
    public async Task<User?> GetUserByIdAsync(Guid id, string tenantId)
    {
        return await _context.Users
            .Include(u => u.UserScopes)
            .FirstOrDefaultAsync(u => u.Id == id && u.TenantId == tenantId && !u.IsDeleted);
    }

    /// <summary>
    /// Get user by username
    /// </summary>
    /// <param name="username">Username</param>
    /// <param name="tenantId">Tenant ID</param>
    /// <returns>User or null if not found</returns>
    public async Task<User?> GetUserByUsernameAsync(string username, string tenantId)
    {
        return await _context.Users
            .Include(u => u.UserScopes)
            .FirstOrDefaultAsync(u => u.Username == username && u.TenantId == tenantId && !u.IsDeleted);
    }

    /// <summary>
    /// Get user by email
    /// </summary>
    /// <param name="email">Email address</param>
    /// <param name="tenantId">Tenant ID</param>
    /// <returns>User or null if not found</returns>
    public async Task<User?> GetUserByEmailAsync(string email, string tenantId)
    {
        return await _context.Users
            .Include(u => u.UserScopes)
            .FirstOrDefaultAsync(u => u.Email == email && u.TenantId == tenantId && !u.IsDeleted);
    }

    /// <summary>
    /// Create new user
    /// </summary>
    /// <param name="user">User to create</param>
    /// <returns>Created user</returns>
    public async Task<User> CreateUserAsync(User user)
    {
        // Hash password before saving
        user.PasswordHash = HashPassword(user.PasswordHash);
        
        _context.Users.Add(user);
        await _context.SaveChangesAsync(CancellationToken.None);
        
        _logger.LogInformation("Created user {Username} for tenant {TenantId}", user.Username, user.TenantId);
        return user;
    }

    /// <summary>
    /// Update user
    /// </summary>
    /// <param name="user">User to update</param>
    /// <returns>Updated user</returns>
    public async Task<User> UpdateUserAsync(User user)
    {
        var existingUser = await _context.Users.FindAsync(user.Id);
        if (existingUser == null)
            throw new ArgumentException("User not found", nameof(user));

        // Update properties
        existingUser.FirstName = user.FirstName;
        existingUser.LastName = user.LastName;
        existingUser.Email = user.Email;
        existingUser.Role = user.Role;
        existingUser.Status = user.Status;
        existingUser.PractitionerId = user.PractitionerId;
        existingUser.ModifiedAt = DateTime.UtcNow;

        // Only update password if provided
        if (!string.IsNullOrEmpty(user.PasswordHash) && user.PasswordHash != existingUser.PasswordHash)
        {
            existingUser.PasswordHash = HashPassword(user.PasswordHash);
        }

        await _context.SaveChangesAsync(CancellationToken.None);
        
        _logger.LogInformation("Updated user {Username} for tenant {TenantId}", user.Username, user.TenantId);
        return existingUser;
    }

    /// <summary>
    /// Delete user
    /// </summary>
    /// <param name="id">User ID</param>
    /// <param name="tenantId">Tenant ID</param>
    /// <returns>True if deleted, false otherwise</returns>
    public async Task<bool> DeleteUserAsync(Guid id, string tenantId)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id && u.TenantId == tenantId);
        if (user == null)
            return false;

        // Soft delete
        user.IsDeleted = true;
        user.DeletedAt = DateTime.UtcNow;
        user.Status = UserStatus.Deleted;

        await _context.SaveChangesAsync(CancellationToken.None);
        
        _logger.LogInformation("Deleted user {Username} for tenant {TenantId}", user.Username, tenantId);
        return true;
    }

    /// <summary>
    /// Get all users for tenant
    /// </summary>
    /// <param name="tenantId">Tenant ID</param>
    /// <param name="skip">Number of records to skip</param>
    /// <param name="take">Number of records to take</param>
    /// <returns>Collection of users</returns>
    public async Task<IEnumerable<User>> GetUsersAsync(string tenantId, int skip = 0, int take = 100)
    {
        return await _context.Users
            .Include(u => u.UserScopes)
            .Where(u => u.TenantId == tenantId && !u.IsDeleted)
            .Skip(skip)
            .Take(take)
            .ToListAsync();
    }

    /// <summary>
    /// Validate user credentials
    /// </summary>
    /// <param name="username">Username</param>
    /// <param name="password">Password</param>
    /// <param name="tenantId">Tenant ID</param>
    /// <returns>User if valid, null otherwise</returns>
    public async Task<User?> ValidateCredentialsAsync(string username, string password, string tenantId)
    {
        var user = await GetUserByUsernameAsync(username, tenantId);
        if (user == null)
            return null;

        // Check if account is locked
        if (user.Status == UserStatus.Locked && user.LockedUntil > DateTime.UtcNow)
        {
            _logger.LogWarning("Login attempt for locked account {Username}", username);
            return null;
        }

        // Check if account is active
        if (user.Status != UserStatus.Active)
        {
            _logger.LogWarning("Login attempt for inactive account {Username}", username);
            return null;
        }

        // Verify password
        if (!VerifyPassword(password, user.PasswordHash))
        {
            await IncrementFailedLoginAttemptsAsync(user.Id);
            return null;
        }

        // Reset failed login attempts on successful login
        await ResetFailedLoginAttemptsAsync(user.Id);
        
        return user;
    }

    /// <summary>
    /// Update user's last login information
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="ipAddress">IP address</param>
    /// <returns>Task</returns>
    public async Task UpdateLastLoginAsync(Guid userId, string ipAddress)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user != null)
        {
            user.LastLoginAt = DateTime.UtcNow;
            user.LastLoginIp = ipAddress;
            await _context.SaveChangesAsync(CancellationToken.None);
        }
    }

    /// <summary>
    /// Increment failed login attempts
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>Task</returns>
    public async Task IncrementFailedLoginAttemptsAsync(Guid userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user != null)
        {
            user.FailedLoginAttempts++;
            
            // Lock account after 5 failed attempts
            if (user.FailedLoginAttempts >= 5)
            {
                user.Status = UserStatus.Locked;
                user.LockedUntil = DateTime.UtcNow.AddMinutes(30); // Lock for 30 minutes
                _logger.LogWarning("Account locked for user {UserId} due to multiple failed login attempts", userId);
            }
            
            await _context.SaveChangesAsync(CancellationToken.None);
        }
    }

    /// <summary>
    /// Reset failed login attempts
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>Task</returns>
    public async Task ResetFailedLoginAttemptsAsync(Guid userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user != null && user.FailedLoginAttempts > 0)
        {
            user.FailedLoginAttempts = 0;
            await _context.SaveChangesAsync(CancellationToken.None);
        }
    }

    /// <summary>
    /// Lock user account
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="lockUntil">Lock until this time</param>
    /// <returns>Task</returns>
    public async Task LockUserAsync(Guid userId, DateTime lockUntil)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user != null)
        {
            user.Status = UserStatus.Locked;
            user.LockedUntil = lockUntil;
            await _context.SaveChangesAsync(CancellationToken.None);
            
            _logger.LogInformation("Account locked for user {UserId} until {LockUntil}", userId, lockUntil);
        }
    }

    /// <summary>
    /// Unlock user account
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>Task</returns>
    public async Task UnlockUserAsync(Guid userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user != null)
        {
            user.Status = UserStatus.Active;
            user.LockedUntil = null;
            user.FailedLoginAttempts = 0;
            await _context.SaveChangesAsync(CancellationToken.None);
            
            _logger.LogInformation("Account unlocked for user {UserId}", userId);
        }
    }

    /// <summary>
    /// Check if user is locked
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>True if locked, false otherwise</returns>
    public async Task<bool> IsUserLockedAsync(Guid userId)
    {
        var user = await _context.Users.FindAsync(userId);
        return user?.Status == UserStatus.Locked && user.LockedUntil > DateTime.UtcNow;
    }

    /// <summary>
    /// Get user scopes
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>Collection of active scopes</returns>
    public async Task<IEnumerable<string>> GetUserScopesAsync(Guid userId)
    {
        return await _context.UserScopes
            .Where(us => us.UserId == userId && us.IsActive)
            .Select(us => us.Scope)
            .ToListAsync();
    }

    /// <summary>
    /// Add scope to user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="scope">Scope to add</param>
    /// <param name="grantedBy">User who granted the scope</param>
    /// <param name="expiresAt">When scope expires (null = no expiration)</param>
    /// <returns>Task</returns>
    public async Task AddUserScopeAsync(Guid userId, string scope, string grantedBy, DateTime? expiresAt = null)
    {
        var userScope = new UserScope
        {
            UserId = userId,
            Scope = scope,
            GrantedBy = grantedBy,
            ExpiresAt = expiresAt,
            TenantId = "default" // TODO: Get from context
        };

        _context.UserScopes.Add(userScope);
        await _context.SaveChangesAsync(CancellationToken.None);
        
        _logger.LogInformation("Added scope {Scope} to user {UserId} by {GrantedBy}", scope, userId, grantedBy);
    }

    /// <summary>
    /// Remove scope from user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="scope">Scope to remove</param>
    /// <returns>Task</returns>
    public async Task RemoveUserScopeAsync(Guid userId, string scope)
    {
        var userScope = await _context.UserScopes
            .FirstOrDefaultAsync(us => us.UserId == userId && us.Scope == scope && us.IsActive);
        
        if (userScope != null)
        {
            userScope.IsActive = false;
            await _context.SaveChangesAsync(CancellationToken.None);
            
            _logger.LogInformation("Removed scope {Scope} from user {UserId}", scope, userId);
        }
    }

    /// <summary>
    /// Hash password using SHA256
    /// </summary>
    /// <param name="password">Plain text password</param>
    /// <returns>Hashed password</returns>
    private static string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }

    /// <summary>
    /// Verify password against hash
    /// </summary>
    /// <param name="password">Plain text password</param>
    /// <param name="hash">Hashed password</param>
    /// <returns>True if password matches hash</returns>
    private static bool VerifyPassword(string password, string hash)
    {
        var hashedPassword = HashPassword(password);
        return hashedPassword == hash;
    }
}
