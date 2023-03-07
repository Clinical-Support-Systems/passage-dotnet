using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text.Encodings.Web;
using JetBrains.Annotations;
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

        protected override Task<object> CreateEventsAsync() => Task.FromResult<object>(new PassageAuthenticationEvents());

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

        //public override Task<bool> HandleRequestAsync()
        //{
        //    if (Request.Cookies.TryGetValue("psg_auth_token", out var authTokenValue))
        //    {
        //        // there is a cookie value that might be important
        //        //throw new NotImplementedException();
        //    }

        //    return base.HandleRequestAsync();
        //}

        //protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        //{
        //    return base.HandleAuthenticateAsync();
        //}

        //protected override Task HandleChallengeAsync(AuthenticationProperties properties)
        //{
        //    return base.HandleChallengeAsync(properties);
        //}

        //public override Task<bool> ShouldHandleRequestAsync()
        //{
        //    //var query = Context.Request.Query;
        //    //return Task.FromResult(query.TryGetValue(PassageAuthenticationConstants.MagicLinkValue, out _));
        //}

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1848:Use the LoggerMessage delegates", Justification = "<Pending>")]
        protected override async Task<HandleRequestResult> HandleRemoteAuthenticateAsync()
        {
            using var cts = new CancellationTokenSource();
            try
            {
                AuthenticationProperties? properties = null;
                var query = Request.Query;
                var protectedRequestToken = Request.Cookies[Options.StateCookie.Name];

                var requestToken = Options.StateDataFormat?.Unprotect(protectedRequestToken);

                properties = requestToken ?? new AuthenticationProperties
                {
                    IssuedUtc = DateTime.UtcNow,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30),
                };

                string? userId = null;
                if (query.TryGetValue(PassageAuthenticationConstants.MagicLinkValue, out var magicLinkValue))
                {
                    var authResult = await _client.Authentication.CompleteMagicLinkLoginAsync(magicLinkValue[0]).ConfigureAwait(false);

                    if (Options.SaveTokens)
                    {
                        properties.StoreTokens(new[] {
                            new AuthenticationToken { Name = "access_token", Value = authResult.AccessToken },
                            new AuthenticationToken { Name = "refresh_token", Value = authResult.RefreshToken }
                        });
                    }

                    // We have a JWT, todo validate
                    _client.Management.Auth = authResult;

                    userId = GetSubFromJwt(authResult.AccessToken);
                }
                else if (query.TryGetValue(PassageAuthenticationConstants.VerifyLinkValue, out var verifyLinkValue))
                {
                    var authResult = await _client.Authentication.CompleteMagicLinkLoginAsync(verifyLinkValue[0]);

                    if (Options.SaveTokens)
                    {
                        properties.StoreTokens(new[] {
                            new AuthenticationToken { Name = "access_token", Value = authResult.AccessToken },
                            new AuthenticationToken { Name = "refresh_token", Value = authResult.RefreshToken }
                        });
                    }

                    // We have a JWT, todo validate
                    _client.Management.Auth = authResult;

                    userId = GetSubFromJwt(authResult.AccessToken);
                }
                else
                {
                    // not magic link? check psg_auth_token
                    var accessToken = Request.Cookies[PassageAuthenticationConstants.CookieAuthTokenValue];

                    userId = GetSubFromJwt(accessToken);

                    if (Options.SaveTokens)
                    {
                        properties.StoreTokens(new[] {
                             new AuthenticationToken { Name = "access_token", Value = accessToken },
                             //new AuthenticationToken { Name = "refresh_token", Value = authResult.RefreshToken }
                        });
                    }
                }

                //if (query.TryGetValue("state", out var state))
                //{
                //    properties = Options.StateDataFormat?.Unprotect(state);
                //    if (properties == null)
                //    {
                //        return HandleRequestResult.Fail("The state was missing or invalid.");
                //    }
                //}

                // Fetch the user using the Management API
                var user = await _client.Management.GetUserAsync(userId, ct: cts.Token);

                if (user == null)
                {
                    return HandleRequestResult.Fail("Could not find user.");
                }

                var identity = new ClaimsIdentity(Scheme.Name);
                identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, userId));

                if (!string.IsNullOrEmpty(user.Email))
                {
                    identity.AddClaim(new Claim(ClaimTypes.Email, user.Email));
                }
                if (!string.IsNullOrEmpty(user.Phone))
                {
                    identity.AddClaim(new Claim(ClaimTypes.HomePhone, user.Phone));
                }
                if (user.CreatedAt != null)
                {
                    identity.AddClaim(new Claim(InternalClaimTypes.CreatedAt, user.CreatedAt.ToString()));
                }
                if (user.UpdatedAt != null)
                {
                    identity.AddClaim(new Claim(InternalClaimTypes.UpdatedAt, user.UpdatedAt.ToString()));
                }
                if (user.LastLoginAt != null)
                {
                    identity.AddClaim(new Claim(InternalClaimTypes.LastLoginAt, user.LastLoginAt.ToString()));
                }

                var ticket = await CreateTicketAsync(identity, properties, user);
                if (ticket == null)
                {
                    Log.SkippedDueToNullTicket(Logger);

                    return HandleRequestResult.SkipHandler();
                }

                //var ticketContext = new PassageAuthenticationCreatingTicketContext(Context, Scheme, Options, principal, properties!, user.Email ?? string.Empty);

                //var ticket = new AuthenticationTicket(new ClaimsPrincipal(identity), Options.SignInScheme);

                // Raise TicketReceived event to give subscribers a chance to modify the ticket
                await Events.TicketReceived(new TicketReceivedContext(Context, Scheme, Options, ticket)).ConfigureAwait(false);

                // Accept the ticket to update user's ClaimsPrincipal and mark user as authenticated
                await Context.SignInAsync(Options.SignInScheme, ticket.Principal, ticket.Properties).ConfigureAwait(false);

                await Events.TicketAccepted(new TicketAcceptedContext(Context, Scheme, Options, ticket)).ConfigureAwait(false);

                return HandleRequestResult.Success(ticket);
            }
            catch (PassageException ex)
            {
                _logger.LogDebug(ex, "Error authenticating with {Handler}", nameof(PassageAuthenticationHandler));
                return HandleRequestResult.Fail(ex);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error");
                throw;
            }
            finally
            {
                cts.Cancel();
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task", Justification = "<Pending>")]
        private async Task<AuthenticationTicket> CreateTicketAsync(
            [NotNull] ClaimsIdentity identity,
            [NotNull] AuthenticationProperties properties,
            [NotNull] User user)
        {
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, properties, Scheme.Name);

            var context = new PassageAuthenticatedContext(Context, Scheme, Options, ticket);

            // Copy the attributes to the context object.
            //foreach (var attribute in attributes)
            //{
            //    context.Attributes.Add(attribute);
            //}

            await Events.Authenticated(context);

            // Note: return the authentication ticket associated
            // with the notification to allow replacing the ticket.
            return context.Ticket;
        }
    }

    internal static partial class Log
    {
        [LoggerMessage(1, LogLevel.Information, "The authentication process was skipped because returned a null ticket was returned.")]
        internal static partial void SkippedDueToNullTicket(ILogger logger);

        [LoggerMessage(2, LogLevel.Warning, "The authentication failed because an invalid check_authentication response was received: " +
                                            "the identity provider returned a {Status} response with the following payload: {Headers} {Body}.")]
        internal static partial void InvalidCheckAuthenticationHttpError(
            ILogger logger,
            HttpStatusCode status,
            string headers,
            string body);

        [LoggerMessage(3, LogLevel.Warning, "The authentication response was rejected because the identity provider " +
                                            "returned an invalid check_authentication response.")]
        internal static partial void InvalidCheckAuthenticationResponse(ILogger logger);

        [LoggerMessage(4, LogLevel.Warning, "The authentication response was rejected because the identity provider " +
                                            "declared the security assertion as invalid.")]
        internal static partial void InvalidSecurityAssertion(ILogger logger);
    }
}
