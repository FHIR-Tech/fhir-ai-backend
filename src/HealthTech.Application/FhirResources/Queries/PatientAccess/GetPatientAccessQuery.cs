using MediatR;
using FluentValidation;
using HealthTech.Application.Common.Interfaces;

namespace HealthTech.Application.FhirResources.Queries.PatientAccess;

/// <summary>
/// Query for getting patient access information
/// </summary>
public record GetPatientAccessQuery : IRequest<GetPatientAccessResponse>
{
    public Guid PatientId { get; init; }
    public Guid? UserId { get; init; }
    public string? AccessLevel { get; init; }
    public bool? IsActive { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}

/// <summary>
/// Response for get patient access query
/// </summary>
public record GetPatientAccessResponse
{
    public bool Success { get; init; }
    public List<PatientAccessInfo> AccessList { get; init; } = new();
    public int TotalCount { get; init; }
    public int Page { get; init; }
    public int PageSize { get; init; }
    public string? ErrorMessage { get; init; }
}

/// <summary>
/// Patient access information
/// </summary>
public record PatientAccessInfo
{
    public Guid Id { get; init; }
    public Guid PatientId { get; init; }
    public Guid UserId { get; init; }
    public string UserName { get; init; } = string.Empty;
    public string UserEmail { get; init; } = string.Empty;
    public string AccessLevel { get; init; } = string.Empty;
    public DateTime GrantedAt { get; init; }
    public DateTime? ExpiresAt { get; init; }
    public bool IsEmergencyAccess { get; init; }
    public string? EmergencyJustification { get; init; }
    public string? RevocationReason { get; init; }
    public DateTime? RevokedAt { get; init; }
    public bool IsActive { get; init; }
}

/// <summary>
/// Validator for get patient access query
/// </summary>
public class GetPatientAccessQueryValidator : AbstractValidator<GetPatientAccessQuery>
{
    public GetPatientAccessQueryValidator()
    {
        RuleFor(x => x.PatientId)
            .NotEmpty().WithMessage("Patient ID is required");

        RuleFor(x => x.Page)
            .GreaterThan(0).WithMessage("Page must be greater than 0");

        RuleFor(x => x.PageSize)
            .GreaterThan(0).WithMessage("Page size must be greater than 0")
            .LessThanOrEqualTo(100).WithMessage("Page size cannot exceed 100");

        RuleFor(x => x.AccessLevel)
            .Must(BeValidAccessLevel).When(x => !string.IsNullOrEmpty(x.AccessLevel))
            .WithMessage("Invalid access level");
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
/// Handler for get patient access query
/// </summary>
public class GetPatientAccessQueryHandler : IRequestHandler<GetPatientAccessQuery, GetPatientAccessResponse>
{
    private readonly IPatientAccessService _patientAccessService;
    private readonly ICurrentUserService _currentUserService;

    public GetPatientAccessQueryHandler(
        IPatientAccessService patientAccessService,
        ICurrentUserService currentUserService)
    {
        _patientAccessService = patientAccessService;
        _currentUserService = currentUserService;
    }

    public async Task<GetPatientAccessResponse> Handle(GetPatientAccessQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // Check if current user has permission to view patient access
            if (!_currentUserService.IsSystemAdministrator() && !_currentUserService.IsHealthcareProvider())
            {
                return new GetPatientAccessResponse
                {
                    Success = false,
                    ErrorMessage = "Insufficient permissions to view patient access"
                };
            }

            // Check if current user can access the patient
            var canAccessPatient = await _currentUserService.CanAccessPatientAsync(request.PatientId.ToString());
            if (!canAccessPatient && !_currentUserService.IsSystemAdministrator())
            {
                return new GetPatientAccessResponse
                {
                    Success = false,
                    ErrorMessage = "You do not have access to this patient"
                };
            }

            // Get patient access list
            var accessList = await _patientAccessService.GetPatientAccessAsync(
                request.PatientId,
                request.UserId,
                request.AccessLevel,
                request.IsActive,
                request.Page,
                request.PageSize);

            // Convert to response format
            var accessInfoList = accessList.Select(access => new PatientAccessInfo
            {
                Id = access.Id,
                PatientId = access.PatientId,
                UserId = access.UserId,
                UserName = access.User?.Username ?? "Unknown",
                UserEmail = access.User?.Email ?? "Unknown",
                AccessLevel = access.AccessLevel,
                GrantedAt = access.GrantedAt,
                ExpiresAt = access.ExpiresAt,
                IsEmergencyAccess = access.IsEmergencyAccess,
                EmergencyJustification = access.EmergencyJustification,
                RevocationReason = access.RevocationReason,
                RevokedAt = access.RevokedAt,
                IsActive = access.RevokedAt == null && (access.ExpiresAt == null || access.ExpiresAt > DateTime.UtcNow)
            }).ToList();

            // Get total count
            var totalCount = await _patientAccessService.GetPatientAccessCountAsync(
                request.PatientId,
                request.UserId,
                request.AccessLevel,
                request.IsActive);

            return new GetPatientAccessResponse
            {
                Success = true,
                AccessList = accessInfoList,
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
                ErrorMessage = "An error occurred while retrieving patient access. Please try again."
            };
        }
    }
}
