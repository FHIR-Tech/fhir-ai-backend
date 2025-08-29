using MediatR;
using HealthTech.Application.Common.Interfaces;

namespace HealthTech.Application.PatientAccess.Commands.GrantPatientAccess;

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
                    IsSuccess = false,
                    Message = "User not authenticated",
                    RequestId = request.RequestId
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
                    IsSuccess = false,
                    Message = "Insufficient permissions to grant patient access",
                    RequestId = request.RequestId
                };
            }

            // Verify target user exists and is in same tenant
            var targetUser = await _userService.GetUserByIdAsync(Guid.Parse(request.TargetUserId), currentUser.TenantId ?? "default");
            if (targetUser == null)
            {
                return new GrantPatientAccessResponse
                {
                    IsSuccess = false,
                    Message = "Target user not found",
                    RequestId = request.RequestId
                };
            }

            if (targetUser.TenantId != currentUser.TenantId)
            {
                return new GrantPatientAccessResponse
                {
                    IsSuccess = false,
                    Message = "Cannot grant access to user from different tenant",
                    RequestId = request.RequestId
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
                IsSuccess = true,
                Message = "Patient access granted successfully",
                RequestId = request.RequestId,
                AccessId = accessId
            };
        }
        catch (Exception ex)
        {
            return new GrantPatientAccessResponse
            {
                IsSuccess = false,
                Message = "An error occurred while granting patient access",
                RequestId = request.RequestId
            };
        }
    }
}
