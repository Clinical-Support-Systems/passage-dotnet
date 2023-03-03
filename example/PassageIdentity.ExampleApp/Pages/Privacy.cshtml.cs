using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PassageIdentity.ExampleApp.Pages
{
    public class PrivacyModel : PageModel 
    {
        [BindProperty(SupportsGet = true)]
        public string? IdToken { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? AccessToken { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? RefreshToken { get; set; }

        private readonly ILogger<PrivacyModel> _logger;

        public PrivacyModel(ILogger<PrivacyModel> logger)
        {
            _logger = logger;
        }

        public async Task OnGet()
        {
            if (User == null || User.Identity == null) return;

            if (User.Identity.IsAuthenticated!)
            {
                IdToken = await HttpContext.GetTokenAsync("id_token");
                AccessToken = await HttpContext.GetTokenAsync("access_token");
                RefreshToken = await HttpContext.GetTokenAsync("refresh_token");
            }
        }

        public Task OnPostSubmit(CancellationToken ct = default)
        {
            return Task.CompletedTask;
        }
    }
}
