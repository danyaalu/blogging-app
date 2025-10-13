using BloggingApp.RazorPages.Models;
using BloggingApp.RazorPages.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BloggingApp.RazorPages.Pages;

public class PostDetailModel : PageModel
{
    private readonly IPostService _postService;
    private readonly IAuthService _authService;

    public PostDetailModel(IPostService postService, IAuthService authService)
    {
        _postService = postService;
        _authService = authService;
    }

    [BindProperty(SupportsGet = true)]
    public string Slug { get; set; } = string.Empty;

    public PostDetailDto? Post { get; set; }
    public bool Loading { get; set; } = true;

    public async Task OnGetAsync()
    {
        ViewData["IsAuthenticated"] = await _authService.IsAuthenticatedAsync();
        
        if (string.IsNullOrEmpty(Slug))
            return;

        Post = await _postService.GetPostBySlugAsync(Slug);
        Loading = false;
    }
}
