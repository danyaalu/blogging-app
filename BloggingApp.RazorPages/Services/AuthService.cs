using BloggingApp.RazorPages.Models;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Json;
using System.Net;
using System.Text.Json;

namespace BloggingApp.RazorPages.Services;

public interface IAuthService
{
    Task<AuthResult> LoginAsync(LoginRequest request);
    Task LogoutAsync();
    Task<string?> GetTokenAsync();
    Task<bool> IsAuthenticatedAsync();
    Task SetTokenAsync(string token);
}

public class AuthResult
{
    public bool Success { get; set; }
    public LoginResponse? LoginResponse { get; set; }
    public string? ErrorMessage { get; set; }
    public bool IsRateLimited { get; set; }
    public int? RetryAfterSeconds { get; set; }
    public int? RetryAfterMinutes { get; set; }
}

public class AuthService : IAuthService
{
    private readonly HttpClient _httpClient;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private const string TokenKey = "authToken";

    public AuthService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
    {
        _httpClient = httpClient;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<AuthResult> LoginAsync(LoginRequest request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/login", request);
            
            if (response.IsSuccessStatusCode)
            {
                var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();
                if (loginResponse != null)
                {
                    await SetTokenAsync(loginResponse.AccessToken);
                    return new AuthResult 
                    { 
                        Success = true, 
                        LoginResponse = loginResponse 
                    };
                }
            }
            
            // Handle rate limiting (HTTP 429)
            if (response.StatusCode == HttpStatusCode.TooManyRequests)
            {
                try
                {
                    var errorContent = await response.Content.ReadFromJsonAsync<JsonElement>();
                    var errorMessage = "Too many login attempts. Please try again later.";
                    
                    if (errorContent.TryGetProperty("message", out var messageProperty))
                    {
                        errorMessage = messageProperty.GetString() ?? errorMessage;
                    }
                    
                    int? retryAfterSeconds = null;
                    int? retryAfterMinutes = null;
                    
                    if (errorContent.TryGetProperty("details", out var details))
                    {
                        if (details.TryGetProperty("retryAfterSeconds", out var retrySecondsProperty))
                        {
                            retryAfterSeconds = retrySecondsProperty.GetInt32();
                        }
                        if (details.TryGetProperty("retryAfterMinutes", out var retryMinutesProperty))
                        {
                            retryAfterMinutes = (int)retryMinutesProperty.GetDouble();
                        }
                    }
                    
                    return new AuthResult
                    {
                        Success = false,
                        IsRateLimited = true,
                        ErrorMessage = errorMessage,
                        RetryAfterSeconds = retryAfterSeconds,
                        RetryAfterMinutes = retryAfterMinutes
                    };
                }
                catch
                {
                    return new AuthResult
                    {
                        Success = false,
                        IsRateLimited = true,
                        ErrorMessage = "Too many login attempts. Please try again later."
                    };
                }
            }
            
            // Handle other authentication errors
            return new AuthResult
            {
                Success = false,
                ErrorMessage = "Invalid email or password. Please try again."
            };
        }
        catch
        {
            return new AuthResult
            {
                Success = false,
                ErrorMessage = "An error occurred while trying to log in. Please try again."
            };
        }
    }

    public Task LogoutAsync()
    {
        _httpContextAccessor.HttpContext?.Session.Remove(TokenKey);
        return Task.CompletedTask;
    }

    public Task<string?> GetTokenAsync()
    {
        var token = _httpContextAccessor.HttpContext?.Session.GetString(TokenKey);
        return Task.FromResult(token);
    }

    public async Task<bool> IsAuthenticatedAsync()
    {
        var token = await GetTokenAsync();
        return !string.IsNullOrEmpty(token);
    }

    public Task SetTokenAsync(string token)
    {
        _httpContextAccessor.HttpContext?.Session.SetString(TokenKey, token);
        return Task.CompletedTask;
    }
}
