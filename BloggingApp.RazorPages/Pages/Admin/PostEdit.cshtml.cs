using BloggingApp.RazorPages.Models;
using BloggingApp.RazorPages.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BloggingApp.RazorPages.Pages.Admin;

public class AdminPostEditModel : PageModel
{
    private readonly IPostService _postService;
    private readonly IAuthService _authService;

    public AdminPostEditModel(IPostService postService, IAuthService authService)
    {
        _postService = postService;
        _authService = authService;
    }

    [BindProperty(SupportsGet = true)]
    public Guid Id { get; set; }

    public PostAdminDto? Post { get; set; }

    [BindProperty]
    public string Title { get; set; } = string.Empty;

    [BindProperty]
    public string Slug { get; set; } = string.Empty;

    [BindProperty]
    public string? Summary { get; set; }

    [BindProperty]
    public string BodyMarkdown { get; set; } = string.Empty;

    [BindProperty]
    public string Status { get; set; } = "Draft";

    public string ErrorMessage { get; set; } = string.Empty;
    public string SuccessMessage { get; set; } = string.Empty;
    public bool Loading { get; set; } = true;

    public async Task<IActionResult> OnGetAsync()
    {
        if (!await _authService.IsAuthenticatedAsync())
        {
            return RedirectToPage("/admin/login");
        }

        ViewData["IsAuthenticated"] = true;
        await LoadPost();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!await _authService.IsAuthenticatedAsync())
        {
            return RedirectToPage("/admin/login");
        }

        ViewData["IsAuthenticated"] = true;

        if (!ModelState.IsValid)
        {
            await LoadPost();
            return Page();
        }

        var request = new UpdatePostRequest(Title, Slug, Summary, BodyMarkdown, Status);
        var result = await _postService.UpdatePostAsync(Id, request);

        if (result != null)
        {
            return RedirectToPage("/admin/posts");
        }

        ErrorMessage = "Failed to update post. Please check your inputs.";
        await LoadPost();
        return Page();
    }

    private async Task LoadPost()
    {
        Loading = true;
        
        var posts = await _postService.GetAdminPostsAsync();
        Post = posts.FirstOrDefault(p => p.Id == Id);

        if (Post != null)
        {
            Title = Post.Title;
            Slug = Post.Slug;
            Summary = Post.Summary;
            BodyMarkdown = Post.BodyMarkdown;
            Status = Post.Status;
        }

        Loading = false;
    }
}
