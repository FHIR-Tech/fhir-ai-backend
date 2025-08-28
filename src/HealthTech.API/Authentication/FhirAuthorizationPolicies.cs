using Microsoft.AspNetCore.Authorization;

namespace HealthTech.API.Authentication;

/// <summary>
/// FHIR authorization policies
/// </summary>
public static class FhirAuthorizationPolicies
{
    /// <summary>
    /// Require system-level access
    /// </summary>
    public const string RequireSystemAccess = "RequireSystemAccess";

    /// <summary>
    /// Require user-level access
    /// </summary>
    public const string RequireUserAccess = "RequireUserAccess";

    /// <summary>
    /// Require patient-level access
    /// </summary>
    public const string RequirePatientAccess = "RequirePatientAccess";

    /// <summary>
    /// Require practitioner-level access
    /// </summary>
    public const string RequirePractitionerAccess = "RequirePractitionerAccess";

    /// <summary>
    /// Require healthcare provider access
    /// </summary>
    public const string RequireHealthcareProviderAccess = "RequireHealthcareProviderAccess";

    /// <summary>
    /// Require system administrator access
    /// </summary>
    public const string RequireSystemAdministratorAccess = "RequireSystemAdministratorAccess";
}

/// <summary>
/// Authorization policy extensions
/// </summary>
public static class AuthorizationPolicyExtensions
{
    /// <summary>
    /// Add FHIR authorization policies
    /// </summary>
    /// <param name="options">Authorization options</param>
    public static void AddFhirAuthorizationPolicies(this AuthorizationOptions options)
    {
        // System-level access policy
        options.AddPolicy(FhirAuthorizationPolicies.RequireSystemAccess, policy =>
            policy.RequireAssertion(context =>
                context.User.HasClaim("scope", "system/*")));

        // User-level access policy
        options.AddPolicy(FhirAuthorizationPolicies.RequireUserAccess, policy =>
            policy.RequireAssertion(context =>
                context.User.HasClaim("scope", "user/*") ||
                context.User.HasClaim("scope", "system/*")));

        // Patient-level access policy
        options.AddPolicy(FhirAuthorizationPolicies.RequirePatientAccess, policy =>
            policy.RequireAssertion(context =>
                context.User.HasClaim("scope", "patient/*") ||
                context.User.HasClaim("scope", "user/*") ||
                context.User.HasClaim("scope", "system/*")));

        // Practitioner-level access policy
        options.AddPolicy(FhirAuthorizationPolicies.RequirePractitionerAccess, policy =>
            policy.RequireAssertion(context =>
                !string.IsNullOrEmpty(context.User.FindFirst("practitioner_id")?.Value) ||
                context.User.HasClaim("scope", "system/*")));

        // Healthcare provider access policy
        options.AddPolicy(FhirAuthorizationPolicies.RequireHealthcareProviderAccess, policy =>
            policy.RequireAssertion(context =>
                context.User.FindFirst("user_role")?.Value == "HealthcareProvider" ||
                context.User.FindFirst("user_role")?.Value == "Nurse" ||
                context.User.HasClaim("scope", "system/*")));

        // System administrator access policy
        options.AddPolicy(FhirAuthorizationPolicies.RequireSystemAdministratorAccess, policy =>
            policy.RequireAssertion(context =>
                context.User.FindFirst("user_role")?.Value == "SystemAdministrator"));
    }
}
