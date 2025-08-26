namespace HealthTech.Domain.Repositories;

/// <summary>
/// Generic repository interface for domain entities
/// </summary>
/// <typeparam name="T">Entity type</typeparam>
public interface IRepository<T> where T : class
{
    /// <summary>
    /// Get entity by ID
    /// </summary>
    /// <param name="id">Entity ID</param>
    /// <param name="tenantId">Tenant ID</param>
    /// <returns>Entity or null if not found</returns>
    Task<T?> GetByIdAsync(Guid id, string tenantId);

    /// <summary>
    /// Get all entities for a tenant
    /// </summary>
    /// <param name="tenantId">Tenant ID</param>
    /// <returns>Collection of entities</returns>
    Task<IEnumerable<T>> GetAllAsync(string tenantId);

    /// <summary>
    /// Add new entity
    /// </summary>
    /// <param name="entity">Entity to add</param>
    Task AddAsync(T entity);

    /// <summary>
    /// Update existing entity
    /// </summary>
    /// <param name="entity">Entity to update</param>
    Task UpdateAsync(T entity);

    /// <summary>
    /// Delete entity
    /// </summary>
    /// <param name="entity">Entity to delete</param>
    Task DeleteAsync(T entity);

    /// <summary>
    /// Check if entity exists
    /// </summary>
    /// <param name="id">Entity ID</param>
    /// <param name="tenantId">Tenant ID</param>
    /// <returns>True if exists, false otherwise</returns>
    Task<bool> ExistsAsync(Guid id, string tenantId);
}
