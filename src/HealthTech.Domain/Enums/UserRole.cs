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
    /// Healthcare provider (doctor, physician)
    /// </summary>
    HealthcareProvider = 2,

    /// <summary>
    /// Nurse or nursing staff
    /// </summary>
    Nurse = 3,

    /// <summary>
    /// Patient with access to their own data
    /// </summary>
    Patient = 4,

    /// <summary>
    /// Family member or caregiver
    /// </summary>
    FamilyMember = 5,

    /// <summary>
    /// Research personnel
    /// </summary>
    Researcher = 6,

    /// <summary>
    /// IT support staff
    /// </summary>
    ITSupport = 7,

    /// <summary>
    /// Read-only user for reporting
    /// </summary>
    ReadOnlyUser = 8,

    /// <summary>
    /// Data analyst with analytics access
    /// </summary>
    DataAnalyst = 9,

    /// <summary>
    /// IT administrator with technical access
    /// </summary>
    ITAdministrator = 10,

    /// <summary>
    /// Guest user with very limited access
    /// </summary>
    Guest = 11
}
