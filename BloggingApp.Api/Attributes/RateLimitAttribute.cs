using BloggingApp.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;

namespace BloggingApp.Api.Attributes;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class RateLimitAttribute : ActionFilterAttribute
{
    private readonly int _maxAttempts;
    private readonly int _windowMinutes;
    private readonly string _keyPrefix;

    public RateLimitAttribute(int maxAttempts = 5, int windowMinutes = 15, string keyPrefix = "login")
    {
        _maxAttempts = maxAttempts;
        _windowMinutes = windowMinutes;
        _keyPrefix = keyPrefix;
    }

    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var rateLimitingService = context.HttpContext.RequestServices.GetRequiredService<IRateLimitingService>();
        
        // Get client IP address
        var clientIp = GetClientIpAddress(context.HttpContext);
        var key = $"{_keyPrefix}:{clientIp}";
        
        var window = TimeSpan.FromMinutes(_windowMinutes);
        var rateLimitInfo = await rateLimitingService.GetRateLimitInfoAsync(key, _maxAttempts, window);
        
        if (!rateLimitInfo.IsAllowed)
        {
            var retryAfterSeconds = (int?)rateLimitInfo.RetryAfter?.TotalSeconds;
            var retryAfterMinutes = retryAfterSeconds.HasValue ? Math.Ceiling(retryAfterSeconds.Value / 60.0) : 0;
            
            var message = $"Too many failed login attempts detected. For security reasons, login attempts from your location have been temporarily blocked.";
            
            if (retryAfterSeconds.HasValue && retryAfterSeconds > 0)
            {
                if (retryAfterSeconds < 60)
                {
                    message += $" Please try again in {retryAfterSeconds} seconds.";
                }
                else
                {
                    message += $" Please try again in approximately {retryAfterMinutes} minutes.";
                }
            }
            else
            {
                message += " Please try again later.";
            }

            context.Result = new ObjectResult(new
            {
                error = "rate_limit_exceeded",
                message = message,
                details = new
                {
                    maxAttempts = _maxAttempts,
                    windowMinutes = _windowMinutes,
                    attemptsRemaining = 0,
                    retryAfterSeconds = retryAfterSeconds,
                    retryAfterMinutes = retryAfterMinutes
                }
            })
            {
                StatusCode = (int)HttpStatusCode.TooManyRequests
            };
            
            // Add rate limit headers
            context.HttpContext.Response.Headers["X-RateLimit-Limit"] = _maxAttempts.ToString();
            context.HttpContext.Response.Headers["X-RateLimit-Remaining"] = "0";
            if (rateLimitInfo.RetryAfter.HasValue)
            {
                context.HttpContext.Response.Headers["Retry-After"] = ((int)rateLimitInfo.RetryAfter.Value.TotalSeconds).ToString();
            }
            
            return;
        }
        
        // Add rate limit headers for successful requests
        context.HttpContext.Response.Headers["X-RateLimit-Limit"] = _maxAttempts.ToString();
        context.HttpContext.Response.Headers["X-RateLimit-Remaining"] = rateLimitInfo.AttemptsRemaining.ToString();
        
        await next();
    }
    
    private static string GetClientIpAddress(HttpContext context)
    {
        // Check for forwarded IP first (for reverse proxy scenarios)
        var forwardedFor = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrEmpty(forwardedFor))
        {
            var ips = forwardedFor.Split(',', StringSplitOptions.RemoveEmptyEntries);
            if (ips.Length > 0)
            {
                return ips[0].Trim();
            }
        }
        
        // Check X-Real-IP header
        var realIp = context.Request.Headers["X-Real-IP"].FirstOrDefault();
        if (!string.IsNullOrEmpty(realIp))
        {
            return realIp;
        }
        
        // Fall back to connection remote IP
        return context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
    }
}
