using BloggingApp.RazorPages.Models;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace BloggingApp.RazorPages.Services;

public interface IPostService
{
    Task<List<PostListDto>> GetPublishedPostsAsync(int skip = 0, int take = 10);
    Task<PostDetailDto?> GetPostBySlugAsync(string slug);
    Task<List<PostAdminDto>> GetAdminPostsAsync(string? status = null);
    Task<PostAdminDto?> CreatePostAsync(CreatePostRequest request);
    Task<PostAdminDto?> UpdatePostAsync(Guid id, UpdatePostRequest request);
    Task<bool> DeletePostAsync(Guid id);
}

public class PostService : IPostService
{
    private readonly HttpClient _httpClient;
    private readonly IAuthService _authService;

    public PostService(HttpClient httpClient, IAuthService authService)
    {
        _httpClient = httpClient;
        _authService = authService;
    }

    public async Task<List<PostListDto>> GetPublishedPostsAsync(int skip = 0, int take = 10)
    {
        try
        {
            var posts = await _httpClient.GetFromJsonAsync<List<PostListDto>>($"api/posts?skip={skip}&take={take}");
            return posts ?? new List<PostListDto>();
        }
        catch
        {
            return new List<PostListDto>();
        }
    }

    public async Task<PostDetailDto?> GetPostBySlugAsync(string slug)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<PostDetailDto>($"api/posts/{slug}");
        }
        catch
        {
            return null;
        }
    }

    public async Task<List<PostAdminDto>> GetAdminPostsAsync(string? status = null)
    {
        await SetAuthHeaderAsync();
        try
        {
            var url = string.IsNullOrEmpty(status) ? "api/admin/posts" : $"api/admin/posts?status={status}";
            var posts = await _httpClient.GetFromJsonAsync<List<PostAdminDto>>(url);
            return posts ?? new List<PostAdminDto>();
        }
        catch
        {
            return new List<PostAdminDto>();
        }
    }

    public async Task<PostAdminDto?> CreatePostAsync(CreatePostRequest request)
    {
        await SetAuthHeaderAsync();
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/admin/posts", request);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<PostAdminDto>();
            }
            return null;
        }
        catch
        {
            return null;
        }
    }

    public async Task<PostAdminDto?> UpdatePostAsync(Guid id, UpdatePostRequest request)
    {
        await SetAuthHeaderAsync();
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"api/admin/posts/{id}", request);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<PostAdminDto>();
            }
            return null;
        }
        catch
        {
            return null;
        }
    }

    public async Task<bool> DeletePostAsync(Guid id)
    {
        await SetAuthHeaderAsync();
        try
        {
            var response = await _httpClient.DeleteAsync($"api/admin/posts/{id}");
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    private async Task SetAuthHeaderAsync()
    {
        var token = await _authService.GetTokenAsync();
        if (!string.IsNullOrEmpty(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
    }
}
