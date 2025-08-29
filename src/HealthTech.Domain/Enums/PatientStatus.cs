namespace HealthTech.Domain.Enums;

/// <summary>
/// Patient status
/// </summary>
public enum PatientStatus
{
    /// <summary>
    /// Active patient
    /// </summary>
    Active,

    /// <summary>
    /// Inactive patient
    /// </summary>
    Inactive,

    /// <summary>
    /// Deceased patient
    /// </summary>
    Deceased,

    /// <summary>
    /// Unknown status
    /// </summary>
    Unknown,

    /// <summary>
    /// Transferred to another facility
    /// </summary>
    Transferred,

    /// <summary>
    /// Discharged
    /// </summary>
    Discharged
}
