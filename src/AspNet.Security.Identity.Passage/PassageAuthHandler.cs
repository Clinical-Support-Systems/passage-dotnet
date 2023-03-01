using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.Features.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AspNet.Security.Identity.Passage
{
    public static class PassageAuthenticationExtensions
    {
        /// <summary>
        /// Adds <see cref="PassageAuthHandler"/> to the specified
        /// <see cref="AuthenticationBuilder"/>, which enables Passage authentication capabilities.
        /// </summary>
        /// <param name="builder">The authentication builder.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static AuthenticationBuilder AddPassage(this AuthenticationBuilder builder)
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            return builder.AddPassage(PassageAuthSchemeConstants.PassageAuthAuthScheme, options => { });
        }

        /// <summary>
        /// Adds <see cref="PassageAuthHandler"/> to the specified
        /// <see cref="AuthenticationBuilder"/>, which enables Passage authentication capabilities.
        /// </summary>
        /// <param name="builder">The authentication builder.</param>
        /// <param name="configuration">The delegate used to configure the Passage options.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static AuthenticationBuilder AddPassage(
            this AuthenticationBuilder builder,
            Action<PassageAuthenticationOptions> configuration)
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (configuration is null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            return builder.AddPassage(PassageAuthSchemeConstants.PassageAuthAuthScheme, configuration);
        }

        /// <summary>
        /// Adds <see cref="PassageAuthHandler"/> to the specified
        /// <see cref="AuthenticationBuilder"/>, which enables Passage authentication capabilities.
        /// </summary>
        /// <param name="builder">The authentication builder.</param>
        /// <param name="scheme">The authentication scheme associated with this instance.</param>
        /// <param name="configuration">The delegate used to configure the Passage options.</param>
        /// <returns>The <see cref="AuthenticationBuilder"/>.</returns>
        public static AuthenticationBuilder AddPassage(
            this AuthenticationBuilder builder,
            string scheme,
            Action<PassageAuthenticationOptions> configuration)
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (string.IsNullOrEmpty(scheme))
            {
                throw new ArgumentException($"'{nameof(scheme)}' cannot be null or empty.", nameof(scheme));
            }

            if (configuration is null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            return builder.AddPassage(scheme, PassageAuthSchemeConstants.PassageDisplayName, configuration);
        }

        /// <summary>
        /// Adds <see cref="PassageAuthHandler"/> to the specified
        /// <see cref="AuthenticationBuilder"/>, which enables OneId authentication capabilities.
        /// </summary>
        /// <param name="builder">The authentication builder.</param>
        /// <param name="scheme">The authentication scheme associated with this instance.</param>
        /// <param name="caption">The optional display name associated with this instance.</param>
        /// <param name="configuration">The delegate used to configure the Passage options.</param>
        /// <returns>The <see cref="AuthenticationBuilder"/>.</returns>
        public static AuthenticationBuilder AddPassage(
            this AuthenticationBuilder builder,
            string scheme,
            string caption,
            Action<PassageAuthenticationOptions> configuration)
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (scheme is null)
            {
                throw new ArgumentNullException(nameof(scheme));
            }

            if (configuration is null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            builder.Services.AddHttpClient();
            builder.Services.TryAddSingleton<JwtSecurityTokenHandler>();

            return builder.AddScheme<PassageAuthenticationOptions, PassageAuthHandler>(scheme, caption, configuration);
        }
    }
    public class PassageAuthHandler : AuthenticationHandler<PassageAuthenticationOptions>
    {
        public PassageAuthHandler(IOptionsMonitor<PassageAuthenticationOptions> options,
                                  ILoggerFactory logger,
                                  UrlEncoder encoder,
                                  ISystemClock clock) : base(options, logger, encoder, clock)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            throw new NotImplementedException();
        }
    }
}
