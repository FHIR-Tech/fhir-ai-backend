namespace HealthTech.Application.Common.Interfaces;

/// <summary>
/// DateTime service interface
/// </summary>
public interface IDateTime
{
    /// <summary>
    /// Current date and time in UTC
    /// </summary>
    DateTime Now { get; }

    /// <summary>
    /// Current date in UTC
    /// </summary>
    DateTime Today { get; }
}
