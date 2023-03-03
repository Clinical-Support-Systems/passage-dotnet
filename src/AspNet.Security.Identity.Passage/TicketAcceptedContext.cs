using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace AspNet.Security.Identity.Passage
{
    public class TicketAcceptedContext
    {
        private readonly HttpContext _context;
        private readonly AuthenticationScheme _scheme;
        private readonly PassageAuthenticationOptions _options;
        private readonly AuthenticationTicket _ticket;

        public TicketAcceptedContext(HttpContext context, AuthenticationScheme scheme, PassageAuthenticationOptions options, AuthenticationTicket ticket)
        {
            _context = context;
            _scheme = scheme;
            _options = options;
            _ticket = ticket;
        }
    }
}
