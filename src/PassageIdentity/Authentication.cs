using System.Globalization;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace PassageIdentity
{
    public class PassageAuthentication
    {
        private readonly ILogger _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IPassageConfig _passageConfig;
        private string? _bearerToken = string.Empty;
        private string? _refreshToken = string.Empty;

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

        public Task<User?> ActivateUserAsync(string identifier, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<User?> DeactivateUserAsync(string identifier, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<User?> UpdateUserAsync(string identifier, string? email, string? phone, Dictionary<string, object>? userMetadata = null, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteUserAsync(string identifier, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<User?> CreateUserAsync(string? email, string? phone, Dictionary<string, object>? userMetadata = null, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// List all WebAuthn devices for the authenticated user. User must be authenticated via bearer token.
        /// </summary>
        /// <param name="identifier"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Device>> GetUserDevicesListAsync(string identifier, CancellationToken ct = default)
        {
            // 401/404/500/200
            // GET https://auth.passage.id/v1/apps/{app_id}/currentuser/devices/
            var uri = new Uri($"https://api.passage.id/v1/apps/{_passageConfig.AppId}/users/{identifier}");
            using var client = _httpClientFactory.CreateClient(PassageConsts.NamedClient);

            _bearerToken = "";
            _refreshToken = "";
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _bearerToken);

            using var response = await client.GetAsync(uri, ct).ConfigureAwait(false);

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                throw new PassageException(string.Format(CultureInfo.InvariantCulture, "Passage User with ID \"{0}\" does not exist", identifier), response);
            }

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(string.Format(CultureInfo.InvariantCulture, "Failed to get Passage User. StatusCode: {0}, ReasonPhrase: {1}", response.StatusCode, response.ReasonPhrase));
            }

            using var responseStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            var results = await JsonSerializer.DeserializeAsync<List<Device>>(responseStream, options, ct).ConfigureAwait(false);
            return results ?? new();
        }

        public async Task<PassageAuthResult?> GetToken(string? refreshToken = null, CancellationToken ct = default)
        {
            var uri = new Uri($"https://auth.passage.id/v1/apps/{_passageConfig.AppId}/tokens/");
            using var client = _httpClientFactory.CreateClient(PassageConsts.NamedClient);
            refreshToken ??= _refreshToken;
            var content = new List<KeyValuePair<string, string>>();
            if (!string.IsNullOrEmpty(refreshToken))
            {
                content.Add(new KeyValuePair<string, string>("refresh_token", refreshToken!));
            }
            using var form = new FormUrlEncodedContent(content);

            using var response = await client.PostAsync(uri, form, ct).ConfigureAwait(false);
            using var responseStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            var results = await JsonSerializer.DeserializeAsync<PassageAuthResult>(responseStream, options, ct).ConfigureAwait(false);
            return results ?? new();
        }
    }
}
