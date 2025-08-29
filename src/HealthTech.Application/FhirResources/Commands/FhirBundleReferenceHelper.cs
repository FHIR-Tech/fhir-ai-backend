using Hl7.Fhir.Model;
using System.Text.Json;

namespace HealthTech.Application.FhirResources.Commands;

/// <summary>
/// Helper class for handling FHIR resource references in bundles
/// </summary>
public static class FhirBundleReferenceHelper
{
    /// <summary>
    /// Extract all resource references from a FHIR resource
    /// </summary>
    /// <param name="resource">FHIR resource</param>
    /// <returns>List of resource references</returns>
    public static List<ResourceReference> ExtractReferences(Resource resource)
    {
        var references = new List<ResourceReference>();
        
        if (resource is DomainResource domainResource)
        {
            // Handle contained resources
            if (domainResource.Contained != null)
            {
                foreach (var contained in domainResource.Contained)
                {
                    references.AddRange(ExtractReferences(contained));
                }
            }

            // Handle extensions
            if (domainResource.Extension != null)
            {
                foreach (var extension in domainResource.Extension)
                {
                    if (extension.Value is ResourceReference extRef)
                    {
                        references.Add(extRef);
                    }
                }
            }
        }

        // Handle specific resource types
        if (resource is Patient patient)
        {
            ExtractPatientReferences(patient, references);
        }
        else if (resource is Encounter encounter)
        {
            ExtractEncounterReferences(encounter, references);
        }
        else if (resource is Observation observation)
        {
            ExtractObservationReferences(observation, references);
        }
        else if (resource is Procedure procedure)
        {
            ExtractProcedureReferences(procedure, references);
        }
        else if (resource is Condition condition)
        {
            ExtractConditionReferences(condition, references);
        }

        return references;
    }

    private static void ExtractPatientReferences(Patient patient, List<ResourceReference> references)
    {
        if (patient.ManagingOrganization != null)
            references.Add(patient.ManagingOrganization);
        
        if (patient.GeneralPractitioner != null)
        {
            foreach (var practitioner in patient.GeneralPractitioner)
            {
                references.Add(practitioner);
            }
        }

        if (patient.Contact != null)
        {
            foreach (var contact in patient.Contact)
            {
                if (contact.Organization != null)
                    references.Add(contact.Organization);
            }
        }
    }

    private static void ExtractEncounterReferences(Encounter encounter, List<ResourceReference> references)
    {
        if (encounter.Subject != null)
            references.Add(encounter.Subject);
        
        if (encounter.ServiceProvider != null)
            references.Add(encounter.ServiceProvider);
        
        if (encounter.Location != null)
        {
            foreach (var location in encounter.Location)
            {
                if (location.Location != null)
                    references.Add(location.Location);
            }
        }

        if (encounter.Participant != null)
        {
            foreach (var participant in encounter.Participant)
            {
                if (participant.Individual != null)
                    references.Add(participant.Individual);
            }
        }
    }

    private static void ExtractObservationReferences(Observation observation, List<ResourceReference> references)
    {
        if (observation.Subject != null)
            references.Add(observation.Subject);
        
        if (observation.Encounter != null)
            references.Add(observation.Encounter);
        
        if (observation.Performer != null)
        {
            foreach (var performer in observation.Performer)
            {
                references.Add(performer);
            }
        }

        if (observation.Specimen != null)
            references.Add(observation.Specimen);
    }

    private static void ExtractProcedureReferences(Procedure procedure, List<ResourceReference> references)
    {
        if (procedure.Subject != null)
            references.Add(procedure.Subject);
        
        if (procedure.Encounter != null)
            references.Add(procedure.Encounter);
        
        if (procedure.Performer != null)
        {
            foreach (var performer in procedure.Performer)
            {
                if (performer.Actor != null)
                    references.Add(performer.Actor);
            }
        }
    }

    private static void ExtractConditionReferences(Condition condition, List<ResourceReference> references)
    {
        if (condition.Subject != null)
            references.Add(condition.Subject);
        
        if (condition.Encounter != null)
            references.Add(condition.Encounter);
        
        if (condition.Asserter != null)
            references.Add(condition.Asserter);
    }

    /// <summary>
    /// Check if a resource has any references to other resources in the bundle
    /// </summary>
    /// <param name="resource">Resource to check</param>
    /// <param name="bundleResourceIds">Set of resource IDs in the bundle</param>
    /// <returns>True if resource has references to other bundle resources</returns>
    public static bool HasBundleReferences(Resource resource, HashSet<string> bundleResourceIds)
    {
        var references = ExtractReferences(resource);
        
        foreach (var reference in references)
        {
            if (reference.Reference != null && bundleResourceIds.Contains(reference.Reference))
            {
                return true;
            }
        }
        
        return false;
    }

    /// <summary>
    /// Get resource type from reference string
    /// </summary>
    /// <param name="reference">Reference string (e.g., "Patient/123")</param>
    /// <returns>Resource type or null</returns>
    public static string? GetResourceTypeFromReference(string reference)
    {
        if (string.IsNullOrEmpty(reference))
            return null;
            
        var parts = reference.Split('/', StringSplitOptions.RemoveEmptyEntries);
        return parts.Length > 0 ? parts[0] : null;
    }

    /// <summary>
    /// Get resource ID from reference string
    /// </summary>
    /// <param name="reference">Reference string (e.g., "Patient/123")</param>
    /// <returns>Resource ID or null</returns>
    public static string? GetResourceIdFromReference(string reference)
    {
        if (string.IsNullOrEmpty(reference))
            return null;
            
        var parts = reference.Split('/', StringSplitOptions.RemoveEmptyEntries);
        return parts.Length > 1 ? parts[1] : null;
    }
}
