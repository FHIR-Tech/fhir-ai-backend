using MediatR;
using FluentValidation;
using HealthTech.Application.Common.Interfaces;
using HealthTech.Domain.Entities;
using HealthTech.Domain.Enums;

namespace HealthTech.Application.PatientAccess.Commands;

public record GrantPatientAccessCommand : IRequest<GrantPatientAccessResponse>
{
    public string TargetUserId { get; init; } = string.Empty;
    public string PatientId { get; init; } = string.Empty;
    public PatientAccessLevel AccessLevel { get; init; }
    public string? Reason { get; init; }
    public DateTime? ExpiresAt { get; init; }
}

public record GrantPatientAccessResponse
{
    public bool Success { get; init; }
    public string? AccessId { get; init; }
    public string? ErrorMessage { get; init; }
}

public class GrantPatientAccessCommandValidator : AbstractValidator<GrantPatientAccessCommand>
{
    public GrantPatientAccessCommandValidator()
    {
        RuleFor(x => x.TargetUserId)
            .NotEmpty().WithMessage("Target user ID is required");

        RuleFor(x => x.PatientId)
            .NotEmpty().WithMessage("Patient ID is required");

        RuleFor(x => x.AccessLevel)
            .IsInEnum().WithMessage("Invalid access level");

        RuleFor(x => x.Reason)
            .MaximumLength(500).WithMessage("Reason cannot exceed 500 characters");

        RuleFor(x => x.ExpiresAt)
            .GreaterThan(DateTime.UtcNow).When(x => x.ExpiresAt.HasValue)
            .WithMessage("Expiration date must be in the future");
    }
}

public class GrantPatientAccessCommandHandler : IRequestHandler<GrantPatientAccessCommand, GrantPatientAccessResponse>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IPatientAccessService _patientAccessService;
    private readonly IUserService _userService;

    public GrantPatientAccessCommandHandler(
        ICurrentUserService currentUserService,
        IPatientAccessService patientAccessService,
        IUserService userService)
    {
        _currentUserService = currentUserService;
        _patientAccessService = patientAccessService;
        _userService = userService;
    }

    public async Task<GrantPatientAccessResponse> Handle(GrantPatientAccessCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var currentUser = _currentUserService;
            if (currentUser.UserId == null)
            {
                return new GrantPatientAccessResponse
                {
                    Success = false,
                    ErrorMessage = "User not authenticated"
                };
            }

            // Check if current user can grant access
            var canGrant = await _patientAccessService.CanGrantAccessAsync(
                currentUser.UserId, 
                request.PatientId, 
                currentUser.UserRole);

            if (!canGrant)
            {
                return new GrantPatientAccessResponse
                {
                    Success = false,
                    ErrorMessage = "Insufficient permissions to grant patient access"
                };
            }

            // Verify target user exists and is in same tenant
            var targetUser = await _userService.GetUserByIdAsync(Guid.Parse(request.TargetUserId), currentUser.TenantId ?? "default");
            if (targetUser == null)
            {
                return new GrantPatientAccessResponse
                {
                    Success = false,
                    ErrorMessage = "Target user not found"
                };
            }

            if (targetUser.TenantId != currentUser.TenantId)
            {
                return new GrantPatientAccessResponse
                {
                    Success = false,
                    ErrorMessage = "Cannot grant access to user from different tenant"
                };
            }

            // Grant access
            var accessId = await _patientAccessService.GrantAccessAsync(
                request.TargetUserId,
                request.PatientId,
                request.AccessLevel,
                currentUser.UserId,
                request.Reason,
                request.ExpiresAt);

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
                ErrorMessage = "An error occurred while granting patient access"
            };
        }
    }
}
