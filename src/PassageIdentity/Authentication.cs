using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace PassageIdentity
{
    public class PassageAuthentication
    {
        private readonly ILogger _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IPassageConfig _passageConfig;

        public PassageAuthentication(ILogger logger, IHttpClientFactory httpClientFactory, IPassageConfig passageConfig)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _passageConfig = passageConfig;
        }

        /// <summary>
        /// Get information about an application.
        /// </summary>
        /// <example>https://docs.passage.id/api-docs/authentication-api/apps</example>
        /// <param name="appId">An App ID if different than the current one.</param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public async Task<App?> GetApp(string? appId = null, CancellationToken ct = default)
        {
            var uri = new Uri($"https://api.passage.id/v1/apps/{appId ?? _passageConfig.AppId}/");
            using var client = _httpClientFactory.CreateClient(PassageConsts.NamedClient);

            //client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", ConfigAPIKey);

            using var response = await client.GetAsync(uri, ct).ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(string.Format(CultureInfo.InvariantCulture, "Failed to get Passage User. StatusCode: {0}, ReasonPhrase: {1}", response.StatusCode, response.ReasonPhrase));
            }

            var responseStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            var appBody = await JsonSerializer.DeserializeAsync<PassageApp>(responseStream, options, ct).ConfigureAwait(false);
            return appBody?.App;
        }

        /// <summary>
        /// Get user information, if the user exists. This endpoint can be used to determine whether a
        /// user has an existing account and if they should login or register.
        /// </summary>
        /// <example>https://docs.passage.id/api-docs/authentication-api/users</example>
        /// <param name="identifier">Email or phone number</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>User, if found. Null otherwise.l</returns>
        /// <exception cref="Exception"></exception>
        public async Task<User?> GetUserAsync(string identifier, CancellationToken ct = default)
        {
            var uri = new Uri($"https://api.passage.id/v1/apps/{_passageConfig.AppId}/users/{identifier}");
            using var client = _httpClientFactory.CreateClient(PassageConsts.NamedClient);

            //client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", ConfigAPIKey);

            using var response = await client.GetAsync(uri, ct).ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(string.Format(CultureInfo.InvariantCulture, "Failed to get Passage User. StatusCode: {0}, ReasonPhrase: {1}", response.StatusCode, response.ReasonPhrase));
            }

            var responseStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            var userBody = await JsonSerializer.DeserializeAsync<PassageUser>(responseStream, options, ct).ConfigureAwait(false);
            return userBody?.User;
        }


        public void StartWebAuthnLogin(string appId)
        {
            // https://auth.passage.id/v1/apps/{app_id}/login/webauthn/start/
            // https://auth.passage.id/v1/apps/{app_id}/login/webauthn/finish/
            throw new NotImplementedException();
        }
    }
}
