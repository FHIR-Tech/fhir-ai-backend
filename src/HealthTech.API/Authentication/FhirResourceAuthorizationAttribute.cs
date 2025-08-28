using Microsoft.AspNetCore.Authorization;

namespace HealthTech.API.Authentication;

/// <summary>
/// FHIR resource authorization attribute
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class FhirResourceAuthorizationAttribute : AuthorizeAttribute
{
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="resourceType">FHIR resource type</param>
    /// <param name="operation">Operation (read, write, delete)</param>
    public FhirResourceAuthorizationAttribute(string resourceType, string operation = "read")
    {
        Policy = GetPolicyForResource(resourceType, operation);
    }

    /// <summary>
    /// Get policy for resource and operation
    /// </summary>
    /// <param name="resourceType">Resource type</param>
    /// <param name="operation">Operation</param>
    /// <returns>Policy name</returns>
    private static string GetPolicyForResource(string resourceType, string operation)
    {
        return resourceType.ToLower() switch
        {
            "patient" => FhirAuthorizationPolicies.RequirePatientAccess,
            "practitioner" => FhirAuthorizationPolicies.RequirePractitionerAccess,
            "organization" => FhirAuthorizationPolicies.RequireUserAccess,
            "location" => FhirAuthorizationPolicies.RequireUserAccess,
            "encounter" => FhirAuthorizationPolicies.RequirePatientAccess,
            "observation" => FhirAuthorizationPolicies.RequirePatientAccess,
            "condition" => FhirAuthorizationPolicies.RequirePatientAccess,
            "medication" => FhirAuthorizationPolicies.RequireUserAccess,
            "medicationrequest" => FhirAuthorizationPolicies.RequirePatientAccess,
            "medicationadministration" => FhirAuthorizationPolicies.RequirePatientAccess,
            "procedure" => FhirAuthorizationPolicies.RequirePatientAccess,
            "allergyintolerance" => FhirAuthorizationPolicies.RequirePatientAccess,
            "immunization" => FhirAuthorizationPolicies.RequirePatientAccess,
            "diagnosticreport" => FhirAuthorizationPolicies.RequirePatientAccess,
            "imagingstudy" => FhirAuthorizationPolicies.RequirePatientAccess,
            "documentreference" => FhirAuthorizationPolicies.RequirePatientAccess,
            "careplan" => FhirAuthorizationPolicies.RequirePatientAccess,
            "goal" => FhirAuthorizationPolicies.RequirePatientAccess,
            "riskassessment" => FhirAuthorizationPolicies.RequirePatientAccess,
            "questionnaire" => FhirAuthorizationPolicies.RequireUserAccess,
            "questionnaireresponse" => FhirAuthorizationPolicies.RequirePatientAccess,
            "consent" => FhirAuthorizationPolicies.RequirePatientAccess,
            "contract" => FhirAuthorizationPolicies.RequireUserAccess,
            "coverage" => FhirAuthorizationPolicies.RequirePatientAccess,
            "claim" => FhirAuthorizationPolicies.RequirePatientAccess,
            "explanationofbenefit" => FhirAuthorizationPolicies.RequirePatientAccess,
            "paymentnotice" => FhirAuthorizationPolicies.RequireUserAccess,
            "paymentreconciliation" => FhirAuthorizationPolicies.RequireUserAccess,
            "account" => FhirAuthorizationPolicies.RequirePatientAccess,
            "chargeitem" => FhirAuthorizationPolicies.RequireUserAccess,
            "invoice" => FhirAuthorizationPolicies.RequireUserAccess,
            "schedule" => FhirAuthorizationPolicies.RequireUserAccess,
            "slot" => FhirAuthorizationPolicies.RequireUserAccess,
            "appointment" => FhirAuthorizationPolicies.RequirePatientAccess,
            "appointmentresponse" => FhirAuthorizationPolicies.RequirePatientAccess,
            "healthcareservice" => FhirAuthorizationPolicies.RequireUserAccess,
            "endpoint" => FhirAuthorizationPolicies.RequireSystemAccess,
            "capabilitystatement" => FhirAuthorizationPolicies.RequireSystemAccess,
            "structuredefinition" => FhirAuthorizationPolicies.RequireSystemAccess,
            "valueset" => FhirAuthorizationPolicies.RequireSystemAccess,
            "codesystem" => FhirAuthorizationPolicies.RequireSystemAccess,
            "conceptmap" => FhirAuthorizationPolicies.RequireSystemAccess,
            "namingsystem" => FhirAuthorizationPolicies.RequireSystemAccess,
            "terminologycapabilities" => FhirAuthorizationPolicies.RequireSystemAccess,
            "testscript" => FhirAuthorizationPolicies.RequireSystemAccess,
            "testreport" => FhirAuthorizationPolicies.RequireSystemAccess,
            "measure" => FhirAuthorizationPolicies.RequireSystemAccess,
            "measurereport" => FhirAuthorizationPolicies.RequireSystemAccess,
            "library" => FhirAuthorizationPolicies.RequireSystemAccess,
            "plandefinition" => FhirAuthorizationPolicies.RequireSystemAccess,
            "activitydefinition" => FhirAuthorizationPolicies.RequireSystemAccess,
            "researchstudy" => FhirAuthorizationPolicies.RequireSystemAccess,
            "researchsubject" => FhirAuthorizationPolicies.RequireSystemAccess,
            "specimen" => FhirAuthorizationPolicies.RequirePatientAccess,
            "bodysite" => FhirAuthorizationPolicies.RequirePatientAccess,
            "device" => FhirAuthorizationPolicies.RequireUserAccess,
            "devicemetric" => FhirAuthorizationPolicies.RequireUserAccess,
            "devicerequest" => FhirAuthorizationPolicies.RequirePatientAccess,
            "deviceusestatement" => FhirAuthorizationPolicies.RequirePatientAccess,
            "guidanceresponse" => FhirAuthorizationPolicies.RequirePatientAccess,
            "supplyrequest" => FhirAuthorizationPolicies.RequireUserAccess,
            "supplydelivery" => FhirAuthorizationPolicies.RequireUserAccess,
            "group" => FhirAuthorizationPolicies.RequireUserAccess,
            "list" => FhirAuthorizationPolicies.RequireUserAccess,
            "composition" => FhirAuthorizationPolicies.RequirePatientAccess,
            "communication" => FhirAuthorizationPolicies.RequirePatientAccess,
            "communicationrequest" => FhirAuthorizationPolicies.RequirePatientAccess,
            "messageheader" => FhirAuthorizationPolicies.RequireSystemAccess,
            "messagedefinition" => FhirAuthorizationPolicies.RequireSystemAccess,
            "subscription" => FhirAuthorizationPolicies.RequireSystemAccess,
            "subscriptionstatus" => FhirAuthorizationPolicies.RequireSystemAccess,
            "subscriptiontopic" => FhirAuthorizationPolicies.RequireSystemAccess,
            "verificationresult" => FhirAuthorizationPolicies.RequireSystemAccess,
            "parameters" => FhirAuthorizationPolicies.RequireSystemAccess,
            "binary" => FhirAuthorizationPolicies.RequireUserAccess,
            "bundle" => FhirAuthorizationPolicies.RequireUserAccess,
            "linkage" => FhirAuthorizationPolicies.RequireSystemAccess,
            "operationoutcome" => FhirAuthorizationPolicies.RequireSystemAccess,
            "auditevent" => FhirAuthorizationPolicies.RequireSystemAccess,
            "provenance" => FhirAuthorizationPolicies.RequireSystemAccess,
            "task" => FhirAuthorizationPolicies.RequireUserAccess,
            "careteam" => FhirAuthorizationPolicies.RequirePatientAccess,
            "episodeofcare" => FhirAuthorizationPolicies.RequirePatientAccess,
            "flag" => FhirAuthorizationPolicies.RequirePatientAccess,
            "medicationknowledge" => FhirAuthorizationPolicies.RequireUserAccess,
            "medicinalproduct" => FhirAuthorizationPolicies.RequireUserAccess,
            "medicinalproductauthorization" => FhirAuthorizationPolicies.RequireUserAccess,
            "medicinalproductcontraindication" => FhirAuthorizationPolicies.RequireUserAccess,
            "medicinalproductindication" => FhirAuthorizationPolicies.RequireUserAccess,
            "medicinalproductingredient" => FhirAuthorizationPolicies.RequireUserAccess,
            "medicinalproductinteraction" => FhirAuthorizationPolicies.RequireUserAccess,
            "medicinalproductmanufactured" => FhirAuthorizationPolicies.RequireUserAccess,
            "medicinalproductpackaged" => FhirAuthorizationPolicies.RequireUserAccess,
            "medicinalproductpharmaceutical" => FhirAuthorizationPolicies.RequireUserAccess,
            "medicinalproductundesirableeffect" => FhirAuthorizationPolicies.RequireUserAccess,
            "molecularsequence" => FhirAuthorizationPolicies.RequirePatientAccess,
            "person" => FhirAuthorizationPolicies.RequireUserAccess,
            "practitionerrole" => FhirAuthorizationPolicies.RequireUserAccess,
            "relatedperson" => FhirAuthorizationPolicies.RequirePatientAccess,
            "researchdefinition" => FhirAuthorizationPolicies.RequireSystemAccess,
            "researchelementdefinition" => FhirAuthorizationPolicies.RequireSystemAccess,
            "researchvariable" => FhirAuthorizationPolicies.RequireSystemAccess,
            "searchparameter" => FhirAuthorizationPolicies.RequireSystemAccess,
            "servicerequest" => FhirAuthorizationPolicies.RequirePatientAccess,
            "specimendefinition" => FhirAuthorizationPolicies.RequireSystemAccess,
            "substance" => FhirAuthorizationPolicies.RequireUserAccess,
            "substancedefinition" => FhirAuthorizationPolicies.RequireSystemAccess,
            "substancenucleicacid" => FhirAuthorizationPolicies.RequireUserAccess,
            "substancepolymer" => FhirAuthorizationPolicies.RequireUserAccess,
            "substancereferenceinformation" => FhirAuthorizationPolicies.RequireUserAccess,
            "substancesourcematerial" => FhirAuthorizationPolicies.RequireUserAccess,
            "substancespecification" => FhirAuthorizationPolicies.RequireUserAccess,
            "topic" => FhirAuthorizationPolicies.RequireSystemAccess,
            "triggerdefinition" => FhirAuthorizationPolicies.RequireSystemAccess,
            "usagecontext" => FhirAuthorizationPolicies.RequireSystemAccess,
            _ => FhirAuthorizationPolicies.RequireUserAccess
        };
    }
}
