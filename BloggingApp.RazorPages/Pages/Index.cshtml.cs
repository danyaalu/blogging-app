using BloggingApp.RazorPages.Models;
using BloggingApp.RazorPages.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BloggingApp.RazorPages.Pages;

public class IndexModel : PageModel
{
    private readonly IPostService _postService;
    private readonly IAuthService _authService;

    public IndexModel(IPostService postService, IAuthService authService)
    {
        _postService = postService;
        _authService = authService;
    }

    public List<PostListDto> Posts { get; set; } = new();
    public bool Loading { get; set; } = true;

    public async Task OnGetAsync()
    {
        ViewData["IsAuthenticated"] = await _authService.IsAuthenticatedAsync();
        Posts = await _postService.GetPublishedPostsAsync(0, 20);
        Loading = false;
    }
}

