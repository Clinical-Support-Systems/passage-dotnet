using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace AspNet.Security.Identity.Passage
{
    public class TicketAcceptedContext
    {
        private HttpContext _context;
        private AuthenticationScheme _scheme;
        private PassageAuthenticationOptions _options;
        private AuthenticationTicket _ticket;

        public TicketAcceptedContext(HttpContext context, AuthenticationScheme scheme, PassageAuthenticationOptions options, AuthenticationTicket ticket)
        {
            _context = context;
            _scheme = scheme;
            _options = options;
            _ticket = ticket;
        }
    }
}
