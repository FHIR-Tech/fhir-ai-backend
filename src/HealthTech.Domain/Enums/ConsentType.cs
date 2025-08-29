namespace HealthTech.Domain.Enums;

/// <summary>
/// Types of patient consent
/// </summary>
public enum ConsentType
{
    /// <summary>
    /// Consent for data sharing with healthcare providers
    /// </summary>
    DataSharing,

    /// <summary>
    /// Consent for research participation
    /// </summary>
    ResearchParticipation,

    /// <summary>
    /// Consent for emergency access
    /// </summary>
    EmergencyAccess,

    /// <summary>
    /// Consent for family member access
    /// </summary>
    FamilyAccess,

    /// <summary>
    /// Consent for third-party access
    /// </summary>
    ThirdPartyAccess,

    /// <summary>
    /// Consent for marketing communications
    /// </summary>
    MarketingCommunications,

    /// <summary>
    /// Consent for automated decision making
    /// </summary>
    AutomatedDecisionMaking,

    /// <summary>
    /// Consent for data portability
    /// </summary>
    DataPortability,

    /// <summary>
    /// Consent for data retention
    /// </summary>
    DataRetention,

    /// <summary>
    /// General treatment consent
    /// </summary>
    TreatmentConsent
}
