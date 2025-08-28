using MediatR;
using FluentValidation;
using HealthTech.Application.Common.Interfaces;

namespace HealthTech.Application.FhirResources.Commands.PatientAccess;

/// <summary>
/// Command for granting patient access
/// </summary>
public record GrantPatientAccessCommand : IRequest<GrantPatientAccessResponse>
{
    public Guid PatientId { get; init; }
    public Guid UserId { get; init; }
    public string AccessLevel { get; init; } = string.Empty;
    public DateTime? ExpiresAt { get; init; }
    public bool IsEmergencyAccess { get; init; }
    public string? EmergencyJustification { get; init; }
    public string? Purpose { get; init; }
}

/// <summary>
/// Response for grant patient access command
/// </summary>
public record GrantPatientAccessResponse
{
    public bool Success { get; init; }
    public Guid? AccessId { get; init; }
    public string? ErrorMessage { get; init; }
}

/// <summary>
/// Validator for grant patient access command
/// </summary>
public class GrantPatientAccessCommandValidator : AbstractValidator<GrantPatientAccessCommand>
{
    public GrantPatientAccessCommandValidator()
    {
        RuleFor(x => x.PatientId)
            .NotEmpty().WithMessage("Patient ID is required");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required");

        RuleFor(x => x.AccessLevel)
            .NotEmpty().WithMessage("Access level is required")
            .Must(BeValidAccessLevel).WithMessage("Invalid access level");

        RuleFor(x => x.EmergencyJustification)
            .NotEmpty().When(x => x.IsEmergencyAccess)
            .WithMessage("Emergency justification is required for emergency access");

        RuleFor(x => x.Purpose)
            .NotEmpty().When(x => !x.IsEmergencyAccess)
            .WithMessage("Purpose is required for non-emergency access");
    }

    private static bool BeValidAccessLevel(string accessLevel)
    {
        return accessLevel switch
        {
            "ReadOnly" => true,
            "ReadWrite" => true,
            "FullAccess" => true,
            "EmergencyAccess" => true,
            "ResearchAccess" => true,
            _ => false
        };
    }
}

/// <summary>
/// Handler for grant patient access command
/// </summary>
public class GrantPatientAccessCommandHandler : IRequestHandler<GrantPatientAccessCommand, GrantPatientAccessResponse>
{
    private readonly IPatientAccessService _patientAccessService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUserService _userService;

    public GrantPatientAccessCommandHandler(
        IPatientAccessService patientAccessService,
        ICurrentUserService currentUserService,
        IUserService userService)
    {
        _patientAccessService = patientAccessService;
        _currentUserService = currentUserService;
        _userService = userService;
    }

    public async Task<GrantPatientAccessResponse> Handle(GrantPatientAccessCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Check if current user has permission to grant access
            if (!_currentUserService.IsSystemAdministrator() && !_currentUserService.IsHealthcareProvider())
            {
                return new GrantPatientAccessResponse
                {
                    Success = false,
                    ErrorMessage = "Insufficient permissions to grant patient access"
                };
            }

            // Check if current user can access the patient
            var canAccessPatient = await _currentUserService.CanAccessPatientAsync(request.PatientId.ToString());
            if (!canAccessPatient && !_currentUserService.IsSystemAdministrator())
            {
                return new GrantPatientAccessResponse
                {
                    Success = false,
                    ErrorMessage = "You do not have access to this patient"
                };
            }

            // Validate that the target user exists
            var targetUser = await _userService.GetByIdAsync(request.UserId);
            if (targetUser == null)
            {
                return new GrantPatientAccessResponse
                {
                    Success = false,
                    ErrorMessage = "Target user not found"
                };
            }

            // Grant access
            var accessId = await _patientAccessService.GrantAccessAsync(
                request.PatientId,
                request.UserId,
                request.AccessLevel,
                request.ExpiresAt,
                request.IsEmergencyAccess,
                request.EmergencyJustification,
                request.Purpose,
                _currentUserService.UserId!.Value);

            return new GrantPatientAccessResponse
            {
                Success = true,
                AccessId = accessId
            };
        }
        catch (Exception ex)
        {
            return new GrantPatientAccessResponse
            {
                Success = false,
                ErrorMessage = "An error occurred while granting patient access. Please try again."
            };
        }
    }
}
