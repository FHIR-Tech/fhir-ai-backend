using MediatR;
using HealthTech.Application.Common.Interfaces;

namespace HealthTech.Application.PatientAccess.Commands.RevokePatientAccess;

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
                    IsSuccess = false,
                    Message = "User not authenticated",
                    RequestId = request.RequestId
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
                    IsSuccess = false,
                    Message = "Insufficient permissions to revoke patient access",
                    RequestId = request.RequestId
                };
            }

            // Revoke access
            var success = await _patientAccessService.RevokeAccessAsync(
                request.AccessId,
                currentUser.UserId,
                request.Reason);

            return new RevokePatientAccessResponse
            {
                IsSuccess = success,
                Message = success ? "Patient access revoked successfully" : "Access not found or already revoked",
                RequestId = request.RequestId
            };
        }
        catch (Exception ex)
        {
            return new RevokePatientAccessResponse
            {
                IsSuccess = false,
                Message = "An error occurred while revoking patient access",
                RequestId = request.RequestId
            };
        }
    }
}
