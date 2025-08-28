using MediatR;
using FluentValidation;
using HealthTech.Application.Common.Interfaces;

namespace HealthTech.Application.PatientAccess.Commands;

public record RevokePatientAccessCommand : IRequest<RevokePatientAccessResponse>
{
    public string AccessId { get; init; } = string.Empty;
    public string? Reason { get; init; }
}

public record RevokePatientAccessResponse
{
    public bool Success { get; init; }
    public string? ErrorMessage { get; init; }
}

public class RevokePatientAccessCommandValidator : AbstractValidator<RevokePatientAccessCommand>
{
    public RevokePatientAccessCommandValidator()
    {
        RuleFor(x => x.AccessId)
            .NotEmpty().WithMessage("Access ID is required");

        RuleFor(x => x.Reason)
            .MaximumLength(500).WithMessage("Reason cannot exceed 500 characters");
    }
}

public class RevokePatientAccessCommandHandler : IRequestHandler<RevokePatientAccessCommand, RevokePatientAccessResponse>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IPatientAccessService _patientAccessService;

    public RevokePatientAccessCommandHandler(
        ICurrentUserService currentUserService,
        IPatientAccessService patientAccessService)
    {
        _currentUserService = currentUserService;
        _patientAccessService = patientAccessService;
    }

    public async Task<RevokePatientAccessResponse> Handle(RevokePatientAccessCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var currentUser = _currentUserService;
            if (currentUser.UserId == null)
            {
                return new RevokePatientAccessResponse
                {
                    Success = false,
                    ErrorMessage = "User not authenticated"
                };
            }

            // Check if current user can revoke this access
            var canRevoke = await _patientAccessService.CanRevokeAccessAsync(
                currentUser.UserId, 
                request.AccessId, 
                currentUser.UserRole);

            if (!canRevoke)
            {
                return new RevokePatientAccessResponse
                {
                    Success = false,
                    ErrorMessage = "Insufficient permissions to revoke patient access"
                };
            }

            // Revoke access
            var success = await _patientAccessService.RevokeAccessAsync(
                request.AccessId,
                currentUser.UserId,
                request.Reason);

            return new RevokePatientAccessResponse
            {
                Success = success,
                ErrorMessage = success ? null : "Access not found or already revoked"
            };
        }
        catch (Exception ex)
        {
            return new RevokePatientAccessResponse
            {
                Success = false,
                ErrorMessage = "An error occurred while revoking patient access"
            };
        }
    }
}
