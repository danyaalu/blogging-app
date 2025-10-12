using BloggingApp.Web.Models;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace BloggingApp.Web.Services;

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
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching posts: {ex.Message}");
            return new List<PostListDto>();
        }
    }

    public async Task<PostDetailDto?> GetPostBySlugAsync(string slug)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<PostDetailDto>($"api/posts/{slug}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching post: {ex.Message}");
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
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching admin posts: {ex.Message}");
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
            
            var error = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Create post failed: {response.StatusCode} - {error}");
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating post: {ex.Message}");
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
            
            var error = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Update post failed: {response.StatusCode} - {error}");
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating post: {ex.Message}");
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
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting post: {ex.Message}");
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
