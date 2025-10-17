using BloggingApp.RazorPages.Models;
using BloggingApp.RazorPages.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BloggingApp.RazorPages.Pages.Admin;

public class AdminPostsModel : PageModel
{
    private readonly IPostService _postService;
    private readonly IAuthService _authService;

    public AdminPostsModel(IPostService postService, IAuthService authService)
    {
        _postService = postService;
        _authService = authService;
    }

    public List<PostAdminDto> FilteredPosts { get; set; } = new();
    public List<PostAdminDto> AllPosts { get; set; } = new();
    public bool Loading { get; set; } = true;
    
    [BindProperty(SupportsGet = true)]
    public string? StatusFilter { get; set; }

    public int DraftCount => AllPosts.Count(p => p.Status == "Draft");
    public int PublishedCount => AllPosts.Count(p => p.Status == "Published");

    public async Task<IActionResult> OnGetAsync()
    {
        if (!await _authService.IsAuthenticatedAsync())
        {
            return RedirectToPage("/admin/login");
        }

        ViewData["IsAuthenticated"] = true;
        await LoadPosts();
        return Page();
    }

    public async Task<IActionResult> OnPostDeleteAsync(Guid id)
    {
        if (!await _authService.IsAuthenticatedAsync())
        {
            return RedirectToPage("/admin/login");
        }

        await _postService.DeletePostAsync(id);
        return RedirectToPage();
    }

    private async Task LoadPosts()
    {
        Loading = true;
        AllPosts = await _postService.GetAdminPostsAsync(null);
        ApplyFilter();
        Loading = false;
    }

    private void ApplyFilter()
    {
        if (string.IsNullOrEmpty(StatusFilter))
        {
            FilteredPosts = AllPosts;
        }
        else
        {
            FilteredPosts = AllPosts.Where(p => p.Status == StatusFilter).ToList();
        }
    }
}
