namespace HealthTech.Domain.Enums;

/// <summary>
/// Defines the different levels of access a user can have to patient data
/// </summary>
public enum PatientAccessLevel
{
    /// <summary>
    /// Read-only access to patient data
    /// </summary>
    Read = 1,

    /// <summary>
    /// Read and write access to patient data
    /// </summary>
    Write = 2,

    /// <summary>
    /// Full access including administrative functions
    /// </summary>
    Admin = 3,

    /// <summary>
    /// Emergency access with full permissions for limited time
    /// </summary>
    Emergency = 4,

    /// <summary>
    /// Access for research purposes with anonymized data
    /// </summary>
    Research = 5,

    /// <summary>
    /// Access for quality improvement and analytics
    /// </summary>
    Analytics = 6
}
