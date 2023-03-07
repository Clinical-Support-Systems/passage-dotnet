using Microsoft.AspNetCore.Authentication;
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
        private readonly PassageClient _client;

        public PrivacyModel(ILogger<PrivacyModel> logger, IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _logger = logger;
            _client = new PassageClient(logger, httpClientFactory, new PassageConfig(configuration["Passage:AppId"] ?? string.Empty));
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

        public async Task OnPostSubmit(CancellationToken ct = default)
        {
            var authResponse = await _client.Authentication.GetToken(RefreshToken, ct);

            if (authResponse == null || authResponse.Result == null) return;
            else
            {
                AccessToken = authResponse.Result?.AccessToken;
                RefreshToken = authResponse.Result?.RefreshToken;
            }
        }
    }
}
