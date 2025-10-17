using BloggingApp.RazorPages.Models;
using BloggingApp.RazorPages.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BloggingApp.RazorPages.Pages.Admin;

public class AdminLoginModel : PageModel
{
    private readonly IAuthService _authService;

    public AdminLoginModel(IAuthService authService)
    {
        _authService = authService;
    }

    [BindProperty]
    public string Email { get; set; } = string.Empty;

    [BindProperty]
    public string Password { get; set; } = string.Empty;

    public string ErrorMessage { get; set; } = string.Empty;

    public async Task<IActionResult> OnGetAsync()
    {
        ViewData["IsAuthenticated"] = await _authService.IsAuthenticatedAsync();
        
        if (await _authService.IsAuthenticatedAsync())
        {
            return RedirectToPage("/admin/posts");
        }
        
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        ViewData["IsAuthenticated"] = false;
        
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var request = new LoginRequest(Email, Password);
        var response = await _authService.LoginAsync(request);

        if (response != null)
        {
            return RedirectToPage("/admin/posts");
        }

        ErrorMessage = "Invalid email or password. Please try again.";
        return Page();
    }
}
