using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PassageIdentity;

namespace AspNet.Security.Identity.Passage
{
    public class PassageAuthHandler : AuthenticationHandler<PassageAuthenticationOptions>
    {
        private PassageClient _client;
        private readonly ILogger<PassageAuthHandler> _logger;

        public PassageAuthHandler(IOptionsMonitor<PassageAuthenticationOptions> options,
                                  ILoggerFactory loggerFactory,
                                  IHttpClientFactory httpClientFactory,
                                  UrlEncoder encoder,
                                  ISystemClock clock)
            : base(options, loggerFactory, encoder, clock)
        {
            if (options is null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            // Setup the handler to remake the client should the options change
            options.OnChange((opt) =>
            {
                _client = new PassageClient(loggerFactory.CreateLogger<PassageAuthHandler>(),
                                        httpClientFactory,
                                        new PassageConfig(opt.AppId ?? string.Empty) { ApiKey = opt.ApiKey });
            });

            _client = new PassageClient(loggerFactory.CreateLogger<PassageAuthHandler>(),
                                        httpClientFactory,
                                        new PassageConfig(options.CurrentValue.AppId ?? string.Empty) { ApiKey = options.CurrentValue.ApiKey });
            _logger = loggerFactory.CreateLogger<PassageAuthHandler>();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1848:Use the LoggerMessage delegates", Justification = "<Pending>")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            using var cts = new CancellationTokenSource();
            try
            {
                var auth = await _client.Authentication.GetToken(ct: cts.Token).ConfigureAwait(false);
                if (auth == null)
                {
                    return AuthenticateResult.Fail("Could not authenticate.");
                }

                // set access and refresh token
                _client.Management.Auth = auth.Result;

                var user = await _client.Management.GetUserAsync(ct: cts.Token).ConfigureAwait(false);

                if (user == null)
                {
                    return AuthenticateResult.Fail("Could not find user.");
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
                var ticket = new AuthenticationTicket(principal, Scheme.Name);
                return AuthenticateResult.Success(ticket);
            }
            catch (PassageException passEx)
            {
                _logger.LogDebug(passEx, "Error authenticating with {Handler}", nameof(PassageAuthHandler));
                return AuthenticateResult.Fail(passEx);
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex, "Error authenticating with {Handler}", nameof(PassageAuthHandler));
                return AuthenticateResult.NoResult();
            }
            finally
            {
                cts.Cancel();
            }
        }
    }
}
