using Microsoft.AspNetCore.Authentication;

namespace AspNet.Security.Identity.Passage
{
    public class PassageAuthenticationOptions : AuthenticationSchemeOptions
    {
        public string? AppId { get; set; }
        public string? ApiKey { get; set; }
    }
}
