using Microsoft.Extensions.Caching.Memory;
using System.Net;

namespace BloggingApp.Api.Services;

public interface IRateLimitingService
{
    Task<bool> IsRequestAllowedAsync(string key, int maxAttempts, TimeSpan window);
    Task<RateLimitInfo> GetRateLimitInfoAsync(string key, int maxAttempts, TimeSpan window);
}

public class RateLimitInfo
{
    public bool IsAllowed { get; set; }
    public int AttemptsRemaining { get; set; }
    public TimeSpan? RetryAfter { get; set; }
    public int TotalAttempts { get; set; }
}

public class RateLimitingService : IRateLimitingService
{
    private readonly IMemoryCache _cache;
    private readonly ILogger<RateLimitingService> _logger;

    public RateLimitingService(IMemoryCache cache, ILogger<RateLimitingService> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public Task<bool> IsRequestAllowedAsync(string key, int maxAttempts, TimeSpan window)
    {
        var info = GetRateLimitInfoAsync(key, maxAttempts, window);
        return Task.FromResult(info.Result.IsAllowed);
    }

    public Task<RateLimitInfo> GetRateLimitInfoAsync(string key, int maxAttempts, TimeSpan window)
    {
        var cacheKey = $"rate_limit:{key}";
        
        if (_cache.TryGetValue(cacheKey, out RateLimitEntry? entry) && entry != null)
        {
            var now = DateTime.UtcNow;
            
            // Check if window has expired
            if (now >= entry.WindowStart.Add(window))
            {
                // Reset the window
                entry = new RateLimitEntry
                {
                    WindowStart = now,
                    Attempts = 1
                };
                
                _cache.Set(cacheKey, entry, window);
                
                return Task.FromResult(new RateLimitInfo
                {
                    IsAllowed = true,
                    AttemptsRemaining = maxAttempts - 1,
                    TotalAttempts = 1
                });
            }
            
            // Within the current window
            entry.Attempts++;
            _cache.Set(cacheKey, entry, entry.WindowStart.Add(window) - now);
            
            var isAllowed = entry.Attempts <= maxAttempts;
            var attemptsRemaining = Math.Max(0, maxAttempts - entry.Attempts);
            TimeSpan? retryAfter = isAllowed ? null : entry.WindowStart.Add(window) - now;
            
            if (!isAllowed)
            {
                _logger.LogWarning("Rate limit exceeded for key: {Key}. Attempts: {Attempts}/{MaxAttempts}", 
                    key, entry.Attempts, maxAttempts);
            }
            
            return Task.FromResult(new RateLimitInfo
            {
                IsAllowed = isAllowed,
                AttemptsRemaining = attemptsRemaining,
                RetryAfter = retryAfter,
                TotalAttempts = entry.Attempts
            });
        }
        
        // First request in window
        var newEntry = new RateLimitEntry
        {
            WindowStart = DateTime.UtcNow,
            Attempts = 1
        };
        
        _cache.Set(cacheKey, newEntry, window);
        
        return Task.FromResult(new RateLimitInfo
        {
            IsAllowed = true,
            AttemptsRemaining = maxAttempts - 1,
            TotalAttempts = 1
        });
    }

    private class RateLimitEntry
    {
        public DateTime WindowStart { get; set; }
        public int Attempts { get; set; }
    }
}
