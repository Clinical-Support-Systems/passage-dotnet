using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace AspNet.Security.Identity.Passage
{
    public class PassageAuthenticationOptions : RemoteAuthenticationOptions
    {
        private sealed class PassageCookieBuilder : CookieBuilder
        {
            private readonly PassageAuthenticationOptions _options;

            public PassageCookieBuilder(PassageAuthenticationOptions options)
            {
                _options = options;
            }

            public override CookieOptions Build(HttpContext context, DateTimeOffset expiresFrom)
            {
                var options = base.Build(context, expiresFrom);
                if (!Expiration.HasValue)
                {
                    options.Expires = expiresFrom.Add(_options.RemoteAuthenticationTimeout);
                }
                return options;
            }
        }

        public PassageAuthenticationOptions()
        {
            CallbackPath = new PathString("/signin-passage");

            _stateCookieBuilder = new PassageCookieBuilder(this)
            {
                Name = DefaultStateCookieName,
                SecurePolicy = CookieSecurePolicy.SameAsRequest,
                HttpOnly = true,
                SameSite = SameSiteMode.Lax,
            };
        }

        public string? ApiKey { get; set; }
        public string? AppId { get; set; }

        private const string DefaultStateCookieName = "__PassageState";

        private CookieBuilder _stateCookieBuilder;

        /// <summary>
        /// Determines the settings used to create the state cookie before the
        /// cookie gets added to the response.
        /// </summary>
        public CookieBuilder StateCookie
        {
            get => _stateCookieBuilder;
            set => _stateCookieBuilder = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// Gets or sets the type used to secure data handled by the middleware.
        /// </summary>
        public ISecureDataFormat<AuthenticationProperties>? StateDataFormat { get; set; }
    }
}
