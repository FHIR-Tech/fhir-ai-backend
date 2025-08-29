using MediatR;

namespace HealthTech.Application.Common.Base;

public abstract record BaseRequest<TResponse> : IRequest<TResponse>
{
    public Guid RequestId { get; init; } = Guid.NewGuid();
    public DateTime RequestedAt { get; init; } = DateTime.UtcNow;
    public string? CorrelationId { get; init; }
    public string? UserId { get; init; }
    public string? TenantId { get; init; }
}
