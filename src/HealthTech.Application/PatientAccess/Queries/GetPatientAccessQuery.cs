using MediatR;
using FluentValidation;
using HealthTech.Application.Common.Interfaces;
using HealthTech.Domain.Enums;

namespace HealthTech.Application.PatientAccess.Queries;

public record GetPatientAccessQuery : IRequest<GetPatientAccessResponse>
{
    public string? PatientId { get; init; }
    public string? UserId { get; init; }
    public PatientAccessLevel? AccessLevel { get; init; }
    public bool? IsActive { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}

public record GetPatientAccessResponse
{
    public bool Success { get; init; }
    public List<PatientAccessInfo> AccessRecords { get; init; } = new();
    public int TotalCount { get; init; }
    public int Page { get; init; }
    public int PageSize { get; init; }
    public string? ErrorMessage { get; init; }
}

public record PatientAccessInfo
{
    public string Id { get; init; } = string.Empty;
    public string UserId { get; init; } = string.Empty;
    public string UserName { get; init; } = string.Empty;
    public string PatientId { get; init; } = string.Empty;
    public string PatientName { get; init; } = string.Empty;
    public PatientAccessLevel AccessLevel { get; init; }
    public string? Reason { get; init; }
    public DateTime GrantedAt { get; init; }
    public string GrantedBy { get; init; } = string.Empty;
    public DateTime? ExpiresAt { get; init; }
    public bool IsActive { get; init; }
}

public class GetPatientAccessQueryValidator : AbstractValidator<GetPatientAccessQuery>
{
    public GetPatientAccessQueryValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThan(0).WithMessage("Page must be greater than 0");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100).WithMessage("Page size must be between 1 and 100");

        RuleFor(x => x.AccessLevel)
            .IsInEnum().When(x => x.AccessLevel.HasValue)
            .WithMessage("Invalid access level");
    }
}

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
                    Success = false,
                    ErrorMessage = "User not authenticated"
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
                    Success = false,
                    ErrorMessage = "Insufficient permissions to view patient access records"
                };
            }

            // Get patient access records
            var (accessRecords, totalCount) = await _patientAccessService.GetPatientAccessAsync(
                request.PatientId,
                request.UserId,
                request.AccessLevel,
                request.IsActive,
                request.Page,
                request.PageSize);

            return new GetPatientAccessResponse
            {
                Success = true,
                AccessRecords = accessRecords,
                TotalCount = totalCount,
                Page = request.Page,
                PageSize = request.PageSize
            };
        }
        catch (Exception ex)
        {
            return new GetPatientAccessResponse
            {
                Success = false,
                ErrorMessage = "An error occurred while retrieving patient access records"
            };
        }
    }
}
