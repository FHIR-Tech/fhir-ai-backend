using MediatR;
using FluentValidation;
using HealthTech.Application.Common.Interfaces;

namespace HealthTech.Application.FhirResources.Commands.PatientAccess;

/// <summary>
/// Command for revoking patient access
/// </summary>
public record RevokePatientAccessCommand : IRequest<RevokePatientAccessResponse>
{
    public Guid AccessId { get; init; }
    public string? Reason { get; init; }
}

/// <summary>
/// Response for revoke patient access command
/// </summary>
public record RevokePatientAccessResponse
{
    public bool Success { get; init; }
    public string? ErrorMessage { get; init; }
}

/// <summary>
/// Validator for revoke patient access command
/// </summary>
public class RevokePatientAccessCommandValidator : AbstractValidator<RevokePatientAccessCommand>
{
    public RevokePatientAccessCommandValidator()
    {
        RuleFor(x => x.AccessId)
            .NotEmpty().WithMessage("Access ID is required");
    }
}

/// <summary>
/// Handler for revoke patient access command
/// </summary>
public class RevokePatientAccessCommandHandler : IRequestHandler<RevokePatientAccessCommand, RevokePatientAccessResponse>
{
    private readonly IPatientAccessService _patientAccessService;
    private readonly ICurrentUserService _currentUserService;

    public RevokePatientAccessCommandHandler(
        IPatientAccessService patientAccessService,
        ICurrentUserService currentUserService)
    {
        _patientAccessService = patientAccessService;
        _currentUserService = currentUserService;
    }

    public async Task<RevokePatientAccessResponse> Handle(RevokePatientAccessCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Check if current user has permission to revoke access
            if (!_currentUserService.IsSystemAdministrator() && !_currentUserService.IsHealthcareProvider())
            {
                return new RevokePatientAccessResponse
                {
                    Success = false,
                    ErrorMessage = "Insufficient permissions to revoke patient access"
                };
            }

            // Get the access record to check permissions
            var access = await _patientAccessService.GetAccessByIdAsync(request.AccessId);
            if (access == null)
            {
                return new RevokePatientAccessResponse
                {
                    Success = false,
                    ErrorMessage = "Patient access record not found"
                };
            }

            // Check if current user can access the patient or is the grantor
            var canAccessPatient = await _currentUserService.CanAccessPatientAsync(access.PatientId.ToString());
            var isGrantor = access.GrantedBy == _currentUserService.UserId;
            
            if (!canAccessPatient && !isGrantor && !_currentUserService.IsSystemAdministrator())
            {
                return new RevokePatientAccessResponse
                {
                    Success = false,
                    ErrorMessage = "You do not have permission to revoke this access"
                };
            }

            // Revoke access
            var success = await _patientAccessService.RevokeAccessAsync(
                request.AccessId,
                request.Reason,
                _currentUserService.UserId!.Value);

            if (!success)
            {
                return new RevokePatientAccessResponse
                {
                    Success = false,
                    ErrorMessage = "Failed to revoke patient access"
                };
            }

            return new RevokePatientAccessResponse
            {
                Success = true
            };
        }
        catch (Exception ex)
        {
            return new RevokePatientAccessResponse
            {
                Success = false,
                ErrorMessage = "An error occurred while revoking patient access. Please try again."
            };
        }
    }
}
