namespace HealthTech.Domain.Enums;

/// <summary>
/// Defines the different statuses a user account can have
/// </summary>
public enum UserStatus
{
    /// <summary>
    /// User account is active and can be used
    /// </summary>
    Active = 1,

    /// <summary>
    /// User account is inactive and cannot be used
    /// </summary>
    Inactive = 2,

    /// <summary>
    /// User account is locked due to security reasons
    /// </summary>
    Locked = 3,

    /// <summary>
    /// User account is suspended temporarily
    /// </summary>
    Suspended = 4,

    /// <summary>
    /// User account is pending activation
    /// </summary>
    Pending = 5,

    /// <summary>
    /// User account is expired
    /// </summary>
    Expired = 6
}
