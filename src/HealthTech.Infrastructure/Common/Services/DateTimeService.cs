using HealthTech.Application.Common.Interfaces;

namespace HealthTech.Infrastructure.Common.Services;

/// <summary>
/// DateTime service implementation
/// </summary>
public class DateTimeService : IDateTime
{
    /// <summary>
    /// Current date and time in UTC
    /// </summary>
    public DateTime Now => DateTime.UtcNow;

    /// <summary>
    /// Current date in UTC
    /// </summary>
    public DateTime Today => DateTime.UtcNow.Date;
}
