using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using PassageIdentity;

namespace AspNet.Security.Identity.Passage
{
    public static class PassageAuthenticationExtensions
    {
        /// <summary>
        /// Adds <see cref="PassageAuthenticationHandler"/> to the specified
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

            return builder.AddPassage(PassageAuthenticationConstants.PassageAuthAuthScheme, options => { });
        }

        /// <summary>
        /// Adds <see cref="PassageAuthenticationHandler"/> to the specified
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

            return builder.AddPassage(PassageAuthenticationConstants.PassageAuthAuthScheme, configuration);
        }

        /// <summary>
        /// Adds <see cref="PassageAuthenticationHandler"/> to the specified
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

            return builder.AddPassage(scheme, PassageAuthenticationConstants.PassageDisplayName, configuration);
        }

        /// <summary>
        /// Adds <see cref="PassageAuthenticationHandler"/> to the specified
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

            builder.Services.AddHttpClient(PassageConsts.NamedClient);
            builder.Services.TryAddSingleton<JwtSecurityTokenHandler>();

            return builder.AddScheme<PassageAuthenticationOptions, PassageAuthenticationHandler>(scheme, caption, configuration);
        }
    }
}
