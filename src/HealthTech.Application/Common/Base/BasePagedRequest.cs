namespace HealthTech.Application.Common.Base;

public abstract record BasePagedRequest<TResponse> : BaseRequest<TResponse>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public string? SortBy { get; init; }
    public string SortOrder { get; init; } = "asc";
    public string? SearchTerm { get; init; }
    public Dictionary<string, object>? Filters { get; init; }
}
