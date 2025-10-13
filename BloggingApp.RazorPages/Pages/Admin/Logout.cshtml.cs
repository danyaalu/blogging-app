using BloggingApp.RazorPages.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BloggingApp.RazorPages.Pages.Admin;

public class LogoutModel : PageModel
{
    private readonly IAuthService _authService;

    public LogoutModel(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        await _authService.LogoutAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        await _authService.LogoutAsync();
        return RedirectToPage("/Index");
    }
}
