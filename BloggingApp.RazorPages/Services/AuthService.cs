using BloggingApp.RazorPages.Models;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Json;

namespace BloggingApp.RazorPages.Services;

public interface IAuthService
{
    Task<LoginResponse?> LoginAsync(LoginRequest request);
    Task LogoutAsync();
    Task<string?> GetTokenAsync();
    Task<bool> IsAuthenticatedAsync();
    Task SetTokenAsync(string token);
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

    public async Task<LoginResponse?> LoginAsync(LoginRequest request)
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
                    return loginResponse;
                }
            }
            
            return null;
        }
        catch
        {
            return null;
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

