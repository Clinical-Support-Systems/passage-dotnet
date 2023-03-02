using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PassageIdentity;

namespace AspNet.Security.Identity.Passage
{
    public class PassageAuthenticationHandler : RemoteAuthenticationHandler<PassageAuthenticationOptions>
    {
        private readonly PassageClient _client;
        private readonly ILogger<PassageAuthenticationHandler> _logger;

        public PassageAuthenticationHandler(IOptionsMonitor<PassageAuthenticationOptions> options,
                                  ILoggerFactory loggerFactory,
                                  IHttpClientFactory httpClientFactory,
                                  UrlEncoder encoder,
                                  ISystemClock clock,
                                  IConfiguration configuration)
            : base(options, loggerFactory, encoder, clock)
        {
            if (options is null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            if (configuration is null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            _client = new PassageClient(loggerFactory.CreateLogger<PassageAuthenticationHandler>(),
                                        httpClientFactory,
                                        new PassageConfig(options.CurrentValue.AppId ?? configuration["Passage:AppId"]) { ApiKey = options.CurrentValue.ApiKey ?? configuration["Passage:ApiKey"] });

            _logger = loggerFactory.CreateLogger<PassageAuthenticationHandler>();
        }
        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            return base.HandleAuthenticateAsync();
        }

        public override Task<bool> HandleRequestAsync()
        {
            if (Request.Cookies.TryGetValue("psg_auth_token", out var authTokenValue))
            {
                // there is a cookie value that might be important
                //throw new NotImplementedException();
            }

            return base.HandleRequestAsync();
        }

        protected override Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            return base.HandleChallengeAsync(properties);
        }

        /// <summary>
        /// The handler calls methods on the events which give the application control at certain points where processing is occurring.
        /// If it is not provided a default instance is supplied which does nothing when the methods are called.
        /// </summary>
        protected new PassageAuthenticationEvents Events
        {
            get => (PassageAuthenticationEvents)base.Events;
            set => base.Events = value;
        }

        public static string? GetSubFromJwt(string? jwtToken)
        {
            var jwtHandler = new JwtSecurityTokenHandler();
            var token = jwtHandler.ReadJwtToken(jwtToken);

            var subClaim = token.Claims.FirstOrDefault(claim => claim.Type == "sub");
            if (subClaim != null)
            {
                return subClaim.Value;
            }

            return null;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1848:Use the LoggerMessage delegates", Justification = "<Pending>")]
        protected override async Task<HandleRequestResult> HandleRemoteAuthenticateAsync()
        {
            using var cts = new CancellationTokenSource();
            try
            {
                AuthenticationProperties? properties = null;
                var query = Request.Query;
                string? userId = null;
                if (query.TryGetValue(PassageAuthenticationConstants.MagicLinkValue, out var magicLinkValue))
                {
                    var authResult = await _client.Authentication.CompleteMagicLinkLoginAsync(magicLinkValue[0]).ConfigureAwait(false);

                    // We have a JWT, todo validate
                    _client.Management.Auth = authResult;

                    userId = GetSubFromJwt(authResult.AccessToken);
                }
                else
                {
                    // not magic link?
                }

                //if (query.TryGetValue("state", out var state))
                //{
                //    properties = Options.StateDataFormat?.Unprotect(state);
                //    if (properties == null)
                //    {
                //        return HandleRequestResult.Fail("The state was missing or invalid.");
                //    }
                //}

                var user = await _client.Management.GetUserAsync(userId, ct: cts.Token).ConfigureAwait(false);

                if (user == null)
                {
                    return HandleRequestResult.Fail("Could not find user.");
                }

                var claims = new[]
                    {
                        new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
                        //new Claim(ClaimTypes.Role, role),
                        //new Claim(InternalClaimTypes.UserId, authorizationInfo.UserId.ToString("N", CultureInfo.InvariantCulture)),
                        //new Claim(InternalClaimTypes.DeviceId, authorizationInfo.DeviceId),
                        //new Claim(InternalClaimTypes.Device, authorizationInfo.Device),
                        //new Claim(InternalClaimTypes.Client, authorizationInfo.Client),
                        //new Claim(InternalClaimTypes.Version, authorizationInfo.Version),
                        //new Claim(InternalClaimTypes.Token, authorizationInfo.Token),
                        //new Claim(InternalClaimTypes.IsApiKey, authorizationInfo.IsApiKey.ToString(CultureInfo.InvariantCulture))
                    };

                var identity = new ClaimsIdentity(claims, Scheme.Name);
                var principal = new ClaimsPrincipal(identity);

                var ticketContext = new PassageAuthenticationCreatingTicketContext(Context, Scheme, Options, principal, properties!, user.Email ?? string.Empty);

                Context.User = ticketContext.Principal;

                var ticket = new AuthenticationTicket(ticketContext.Principal, ticketContext.Properties, Scheme.Name);

                //await Options.Events.CreatingTicket(ticketContext);

                return HandleRequestResult.Success(ticket);
            }
            catch (PassageException ex)
            {
                _logger.LogDebug(ex, "Error authenticating with {Handler}", nameof(PassageAuthenticationHandler));
                return HandleRequestResult.Fail(ex);
            }
            finally
            {
                cts.Cancel();
            }
        }
    }
}
