using MediatR;
using HealthTech.Application.Common.Interfaces;

namespace HealthTech.Application.PatientAccess.Queries.GetPatientAccess;

public class GetPatientAccessQueryHandler : IRequestHandler<GetPatientAccessQuery, GetPatientAccessResponse>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IPatientAccessService _patientAccessService;

    public GetPatientAccessQueryHandler(
        ICurrentUserService currentUserService,
        IPatientAccessService patientAccessService)
    {
        _currentUserService = currentUserService;
        _patientAccessService = patientAccessService;
    }

    public async Task<GetPatientAccessResponse> Handle(GetPatientAccessQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var currentUser = _currentUserService;
            if (currentUser.UserId == null)
            {
                return new GetPatientAccessResponse
                {
                    IsSuccess = false,
                    Message = "User not authenticated",
                    RequestId = request.RequestId
                };
            }

            // Check if user has permission to view patient access records
            var canView = await _patientAccessService.CanViewAccessRecordsAsync(
                currentUser.UserId, 
                request.PatientId, 
                currentUser.UserRole);

            if (!canView)
            {
                return new GetPatientAccessResponse
                {
                    IsSuccess = false,
                    Message = "Insufficient permissions to view patient access records",
                    RequestId = request.RequestId
                };
            }

            // Get patient access records
            var (accessRecords, totalCount) = await _patientAccessService.GetPatientAccessAsync(
                request.PatientId,
                request.UserId,
                request.AccessLevel,
                request.IsActive,
                request.PageNumber,
                request.PageSize);

            // Convert to the correct type
            var convertedRecords = accessRecords.Select(record => new PatientAccessInfo
            {
                Id = record.Id,
                UserId = record.UserId,
                UserName = record.UserName,
                PatientId = record.PatientId,
                PatientName = record.PatientName,
                AccessLevel = record.AccessLevel,
                Reason = record.Reason,
                GrantedAt = record.GrantedAt,
                GrantedBy = record.GrantedBy,
                ExpiresAt = record.ExpiresAt,
                IsActive = record.IsActive
            }).ToList();

            var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);

            return new GetPatientAccessResponse
            {
                IsSuccess = true,
                Message = "Patient access records retrieved successfully",
                RequestId = request.RequestId,
                Items = convertedRecords,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalPages = totalPages,
                HasPreviousPage = request.PageNumber > 1,
                HasNextPage = request.PageNumber < totalPages
            };
        }
        catch (Exception ex)
        {
            return new GetPatientAccessResponse
            {
                IsSuccess = false,
                Message = "An error occurred while retrieving patient access records",
                RequestId = request.RequestId
            };
        }
    }
}
