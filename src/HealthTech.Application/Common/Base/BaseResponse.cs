namespace HealthTech.Application.Common.Base;

public abstract record BaseResponse
{
    public bool IsSuccess { get; init; }
    public string? Message { get; init; }
    public List<string> Errors { get; init; } = new();
    public Guid RequestId { get; init; }
    public DateTime RespondedAt { get; init; } = DateTime.UtcNow;
    public int StatusCode { get; init; } = 200;
}
