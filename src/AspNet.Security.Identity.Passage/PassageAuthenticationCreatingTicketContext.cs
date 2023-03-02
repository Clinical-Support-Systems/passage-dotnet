using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace AspNet.Security.Identity.Passage
{
    public class PassageAuthenticationCreatingTicketContext : ResultContext<PassageAuthenticationOptions>
    {
        public PassageAuthenticationCreatingTicketContext(HttpContext context,
                                                          AuthenticationScheme scheme,
                                                          PassageAuthenticationOptions options,
                                                          ClaimsPrincipal principal,
                                                          AuthenticationProperties properties,
                                                          string userName)
            : base(context, scheme, options)
        {
            Username = userName;
            Principal = principal;
            Properties = properties;
        }

        /// <summary>
        /// Gets the user identifier
        /// </summary>
        public string Username { get; }
    }
}
