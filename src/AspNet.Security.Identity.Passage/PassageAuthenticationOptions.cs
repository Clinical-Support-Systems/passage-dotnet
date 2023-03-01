using Microsoft.AspNetCore.Authentication;

namespace AspNet.Security.Identity.Passage
{
    public class PassageAuthenticationOptions : AuthenticationSchemeOptions
    {
        public string? ApiKey { get; set; }
        public string? AppId { get; set; }
    }
}
