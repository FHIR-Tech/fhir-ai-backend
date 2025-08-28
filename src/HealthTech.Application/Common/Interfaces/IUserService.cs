using HealthTech.Domain.Entities;

namespace HealthTech.Application.Common.Interfaces;

/// <summary>
/// Service for managing users
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Get user by ID
    /// </summary>
    /// <param name="id">User ID</param>
    /// <param name="tenantId">Tenant ID</param>
    /// <returns>User or null if not found</returns>
    Task<User?> GetUserByIdAsync(Guid id, string tenantId);

    /// <summary>
    /// Get user by username
    /// </summary>
    /// <param name="username">Username</param>
    /// <param name="tenantId">Tenant ID</param>
    /// <returns>User or null if not found</returns>
    Task<User?> GetUserByUsernameAsync(string username, string tenantId);

    /// <summary>
    /// Get user by email
    /// </summary>
    /// <param name="email">Email address</param>
    /// <param name="tenantId">Tenant ID</param>
    /// <returns>User or null if not found</returns>
    Task<User?> GetUserByEmailAsync(string email, string tenantId);

    /// <summary>
    /// Create new user
    /// </summary>
    /// <param name="user">User to create</param>
    /// <returns>Created user</returns>
    Task<User> CreateUserAsync(User user);

    /// <summary>
    /// Update user
    /// </summary>
    /// <param name="user">User to update</param>
    /// <returns>Updated user</returns>
    Task<User> UpdateUserAsync(User user);

    /// <summary>
    /// Delete user
    /// </summary>
    /// <param name="id">User ID</param>
    /// <param name="tenantId">Tenant ID</param>
    /// <returns>True if deleted, false otherwise</returns>
    Task<bool> DeleteUserAsync(Guid id, string tenantId);

    /// <summary>
    /// Get all users for tenant
    /// </summary>
    /// <param name="tenantId">Tenant ID</param>
    /// <param name="skip">Number of records to skip</param>
    /// <param name="take">Number of records to take</param>
    /// <returns>Collection of users</returns>
    Task<IEnumerable<User>> GetUsersAsync(string tenantId, int skip = 0, int take = 100);

    /// <summary>
    /// Validate user credentials
    /// </summary>
    /// <param name="username">Username</param>
    /// <param name="password">Password</param>
    /// <param name="tenantId">Tenant ID</param>
    /// <returns>User if valid, null otherwise</returns>
    Task<User?> ValidateCredentialsAsync(string username, string password, string tenantId);

    /// <summary>
    /// Update user's last login information
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="ipAddress">IP address</param>
    /// <returns>Task</returns>
    Task UpdateLastLoginAsync(Guid userId, string ipAddress);

    /// <summary>
    /// Increment failed login attempts
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>Task</returns>
    Task IncrementFailedLoginAttemptsAsync(Guid userId);

    /// <summary>
    /// Reset failed login attempts
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>Task</returns>
    Task ResetFailedLoginAttemptsAsync(Guid userId);

    /// <summary>
    /// Lock user account
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="lockUntil">Lock until this time</param>
    /// <returns>Task</returns>
    Task LockUserAsync(Guid userId, DateTime lockUntil);

    /// <summary>
    /// Unlock user account
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>Task</returns>
    Task UnlockUserAsync(Guid userId);

    /// <summary>
    /// Check if user is locked
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>True if locked, false otherwise</returns>
    Task<bool> IsUserLockedAsync(Guid userId);

    /// <summary>
    /// Get user scopes
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>Collection of active scopes</returns>
    Task<IEnumerable<string>> GetUserScopesAsync(Guid userId);

    /// <summary>
    /// Add scope to user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="scope">Scope to add</param>
    /// <param name="grantedBy">User who granted the scope</param>
    /// <param name="expiresAt">When scope expires (null = no expiration)</param>
    /// <returns>Task</returns>
    Task AddUserScopeAsync(Guid userId, string scope, string grantedBy, DateTime? expiresAt = null);

    /// <summary>
    /// Remove scope from user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="scope">Scope to remove</param>
    /// <returns>Task</returns>
    Task RemoveUserScopeAsync(Guid userId, string scope);
}
