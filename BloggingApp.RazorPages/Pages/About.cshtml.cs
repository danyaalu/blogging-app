using BloggingApp.RazorPages.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BloggingApp.RazorPages.Pages;

public class AboutModel : PageModel
{
    private readonly IAuthService _authService;

    public AboutModel(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task OnGetAsync()
    {
        ViewData["IsAuthenticated"] = await _authService.IsAuthenticatedAsync();
    }
}

