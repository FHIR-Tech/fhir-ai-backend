using FluentValidation;
using Hl7.Fhir.Serialization;

namespace HealthTech.Application.FhirResources.Commands.CreateFhirResource;

/// <summary>
/// Validator for CreateFhirResourceCommand
/// </summary>
public class CreateFhirResourceCommandValidator : AbstractValidator<CreateFhirResourceCommand>
{
    /// <summary>
    /// Constructor
    /// </summary>
    public CreateFhirResourceCommandValidator()
    {
        RuleFor(x => x.ResourceType)
            .NotEmpty()
            .WithMessage("Resource type is required")
            .MaximumLength(100)
            .WithMessage("Resource type cannot exceed 100 characters");

        RuleFor(x => x.ResourceJson)
            .NotEmpty()
            .WithMessage("Resource JSON is required")
            .Must(BeValidJson)
            .WithMessage("Resource JSON must be valid JSON format");

        RuleFor(x => x.FhirId)
            .MaximumLength(255)
            .WithMessage("FHIR ID cannot exceed 255 characters")
            .When(x => !string.IsNullOrEmpty(x.FhirId));
    }

    private static bool BeValidJson(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
            return false;

        try
        {
            // Try to parse as JSON
            var parser = new FhirJsonParser();
            parser.Parse<Hl7.Fhir.Model.Resource>(json);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
