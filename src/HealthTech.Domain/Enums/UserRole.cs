namespace HealthTech.Domain.Enums;

/// <summary>
/// Defines the different roles a user can have in the system
/// </summary>
public enum UserRole
{
    /// <summary>
    /// System administrator with full access
    /// </summary>
    SystemAdministrator = 1,

    /// <summary>
    /// Healthcare provider (doctor, nurse, etc.)
    /// </summary>
    HealthcareProvider = 2,

    /// <summary>
    /// Patient with access to their own data
    /// </summary>
    Patient = 3,

    /// <summary>
    /// Researcher with limited access for research purposes
    /// </summary>
    Researcher = 4,

    /// <summary>
    /// Data analyst with analytics access
    /// </summary>
    DataAnalyst = 5,

    /// <summary>
    /// IT administrator with technical access
    /// </summary>
    ITAdministrator = 6,

    /// <summary>
    /// Guest user with very limited access
    /// </summary>
    Guest = 7
}
