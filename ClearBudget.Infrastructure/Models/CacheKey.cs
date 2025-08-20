using ClearBudget.Infrastructure.Enums;

namespace ClearBudget.Infrastructure.Models;

public class CacheKey(string title, TimeSpan? duration = null, CacheExpiration expiration = CacheExpiration.Absolute)
{
    public string Title { get; set; } = title;
    public TimeSpan Duration { get; set; } = duration ?? TimeSpan.FromSeconds(60);
    public CacheExpiration ExpirationType { get; set; } = expiration;
}