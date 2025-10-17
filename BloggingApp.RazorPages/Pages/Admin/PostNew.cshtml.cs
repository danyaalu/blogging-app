using BloggingApp.RazorPages.Models;
using BloggingApp.RazorPages.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BloggingApp.RazorPages.Pages.Admin;

public class AdminPostNewModel : PageModel
{
    private readonly IPostService _postService;
    private readonly IAuthService _authService;
    private readonly ISlugService _slugService;

    public AdminPostNewModel(IPostService postService, IAuthService authService, ISlugService slugService)
    {
        _postService = postService;
        _authService = authService;
        _slugService = slugService;
    }

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

    public async Task<IActionResult> OnGetAsync()
    {
        if (!await _authService.IsAuthenticatedAsync())
        {
            return RedirectToPage("/admin/login");
        }

        ViewData["IsAuthenticated"] = true;
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
            return Page();
        }

        // Generate slug if not provided
        if (string.IsNullOrWhiteSpace(Slug))
        {
            Slug = _slugService.GenerateSlug(Title);
        }

        var request = new CreatePostRequest(Title, Slug, Summary, BodyMarkdown, Status);
        var result = await _postService.CreatePostAsync(request);

        if (result != null)
        {
            return RedirectToPage("/admin/posts");
        }

        ErrorMessage = "Failed to create post. Please check your inputs and try again.";
        return Page();
    }
}
