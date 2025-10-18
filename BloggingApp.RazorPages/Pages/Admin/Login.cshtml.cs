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
    public bool IsRateLimited { get; set; } = false;
    public int? RetryAfterSeconds { get; set; }
    public int? RetryAfterMinutes { get; set; }

    // Computed properties for the view
    public bool HasRetryInfo => RetryAfterSeconds.HasValue || RetryAfterMinutes.HasValue;
    public bool ShowSeconds => RetryAfterSeconds.HasValue && RetryAfterSeconds < 60;
    public bool ShowMinutes => RetryAfterMinutes.HasValue && (!RetryAfterSeconds.HasValue || RetryAfterSeconds >= 60);
    public string RetryTimeDisplay
    {
        get
        {
            if (ShowSeconds)
                return $"You can try again in {RetryAfterSeconds} seconds.";
            else if (ShowMinutes)
                return $"You can try again in approximately {RetryAfterMinutes} minutes.";
            return "";
        }
    }

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
        var result = await _authService.LoginAsync(request);

        if (result.Success && result.LoginResponse != null)
        {
            return RedirectToPage("/admin/posts");
        }

        // Handle rate limiting errors
        if (result.IsRateLimited)
        {
            IsRateLimited = true;
            RetryAfterSeconds = result.RetryAfterSeconds;
            RetryAfterMinutes = result.RetryAfterMinutes;
        }

        ErrorMessage = result.ErrorMessage ?? "An error occurred while trying to log in.";
        return Page();
    }
}
