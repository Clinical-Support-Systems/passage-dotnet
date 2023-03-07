using System.Net.Sockets;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace AspNet.Security.Identity.Passage
{
    public class PassageAuthenticatedContext : BaseContext<PassageAuthenticationOptions>
    {
        public PassageAuthenticatedContext(HttpContext context, AuthenticationScheme scheme, PassageAuthenticationOptions options, AuthenticationTicket ticket) : base(context, scheme, options)
        {
            Ticket = ticket;
        }

        /// <summary>
        /// Gets or sets the authentication ticket.
        /// </summary>
        public AuthenticationTicket Ticket { get; set; }

        /// <summary>
        /// Gets the identity containing the claims associated with the current user.
        /// </summary>
        public ClaimsIdentity? Identity => Ticket?.Principal?.Identity as ClaimsIdentity;

        /// <summary>
        /// Gets the identifier returned by the identity provider.
        /// </summary>
        public string? Identifier => Ticket?.Principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        /// <summary>
        /// Gets the authentication properties associated with the ticket.
        /// </summary>
        public AuthenticationProperties? Properties => Ticket?.Properties;

        /// <summary>
        /// Gets or sets the attributes associated with the current user.
        /// </summary>
        public IDictionary<string, string> Attributes { get; } = new Dictionary<string, string>();

        /// <summary>
        /// Gets or sets the optional JSON payload extracted from the current request.
        /// This property is not set by the generic middleware but can be used by specialized middleware.
        /// </summary>
        public JsonDocument? UserPayload { get; set; }
    }
}
