using System.Globalization;
using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace PassageIdentity;

public class PassageManagement
{
    private readonly IPassageConfig _config;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger _logger;
    private AuthResult? _authResult;

    public PassageManagement(ILogger logger, IHttpClientFactory httpClientFactory, IPassageConfig config)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
        _config = config;
    }

    public AuthResult? Auth { get => _authResult; set => _authResult = value; }

    /// <summary>
    /// Assign an app that is currently in test mode to the current authorized user.
    /// Test mode apps are created when an App ID is not specified in the Passage
    /// custom element.
    /// </summary>
    /// <param name="ct"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public Task ClaimTestAppAsync(CancellationToken ct = default)
    {
        // POST https://api.passage.id/v1/apps/{app_id}/claim/
        // Authorization: Bearer (key/jwt)
        // 200/400/401/404/500
        throw new NotImplementedException();
    }

    /// <summary>
    /// Create a new application. If not authenticated, the app is created in test
    /// mode and not assigned to a specific user.
    /// </summary>
    /// <param name="app"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public Task<App?> CreateAppAsync(App app, CancellationToken ct = default)
    {
        // POST https://api.passage.id/v1/apps/
        // Authorization: Bearer (key/jwt)
        // 200/400/409/500
        throw new NotImplementedException();
    }

    /// <summary>
    /// Create user for an application. Must provide an email of phone number identifier.
    /// </summary>
    /// <param name="user"></param>
    /// <param name="appId"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    public Task<User?> CreateUserAsync(User user, string? appId = null, CancellationToken ct = default)
    {
        // POST https://api.passage.id/v1/apps/{app_id}/users/
        // Authorization: Bearer (api_key/auth_token)
        // 200/400/401/409/500
        throw new NotImplementedException();
    }

    /// <summary>
    /// Bulk upload users for an application. Files must be in valid CSV format and no more than 30k users.
    /// </summary>
    /// <param name="csvData">key: csv_user_import, file type: text/csv</param>
    /// <param name="appId"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    public Task<IEnumerable<User?>> CreateUsersAsync(byte[] csvData, string? appId = null, CancellationToken ct = default)
    {
        // POST https://api.passage.id/v1/apps/{app_id}/import/users/
        // Authorization: Bearer (api_key/auth_token)
        // 200/400/401/500
        throw new NotImplementedException();
    }

    /// <summary>
    /// Delete an application.
    /// </summary>
    /// <param name="appId"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public Task<bool> DeleteAppAsync(string appId, CancellationToken ct = default)
    {
        // DELETE https://api.passage.id/v1/apps/{app_id}/
        // Authorization: Bearer (api_key/auth_token)
        // 200/401/404/500
        throw new NotImplementedException();
    }

    public async Task<App?> GetAppAsync(CancellationToken ct = default)
    {
        try
        {
            // GET https://api.passage.id/v1/apps/{app_id}/
            // Authorization: Bearer (api_key)
            // 200/400/401/404/500
            var uri = new Uri($"https://api.passage.id/v1/apps/{_config.AppId}/");
            using var client = _httpClientFactory.CreateClient(PassageConsts.NamedClient);

            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _config.ApiKey);

            using var response = await client.GetAsync(uri, ct).ConfigureAwait(false);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                throw new PassageException(string.Format(CultureInfo.InvariantCulture, "Passage app with ID \"{0}\" does not exist", _config.AppId), response);
            }

            if (!response.IsSuccessStatusCode)
            {
                throw new PassageException(string.Format(CultureInfo.InvariantCulture, "Failed to get Passage User. StatusCode: {0}, ReasonPhrase: {1}", response.StatusCode, response.ReasonPhrase), response);
            }

            var responseStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            var result = await JsonSerializer.DeserializeAsync<PassageApp>(responseStream, options, ct).ConfigureAwait(false) ?? new();
            return result.App;
        }
        catch (Exception)
        {
            throw;
        }
    }

    /// <summary>
    /// List apps for current authorized user
    /// </summary>
    /// <param name="ct">Cancellation token</param>
    /// <returns>The app</returns>
    /// <exception cref="PassageException"></exception>
    public async Task<IEnumerable<App?>> GetAppsAsync(CancellationToken ct = default)
    {
        var uri = new Uri($"https://api.passage.id/v1/apps/");
        using var client = _httpClientFactory.CreateClient(PassageConsts.NamedClient);

        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _config.ApiKey);

        using var response = await client.GetAsync(uri, ct).ConfigureAwait(false);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            throw new PassageException(string.Format(CultureInfo.InvariantCulture, "Passage app with ID \"{0}\" does not exist", _config.AppId), response);
        }

        if (!response.IsSuccessStatusCode)
        {
            throw new PassageException(string.Format(CultureInfo.InvariantCulture, "Failed to get Passage User. StatusCode: {0}, ReasonPhrase: {1}", response.StatusCode, response.ReasonPhrase), response);
        }

        var responseStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        var appBody = await JsonSerializer.DeserializeAsync<List<PassageApp>>(responseStream, options, ct).ConfigureAwait(false) ?? new List<PassageApp>();
        return appBody.ConvertAll(x => x.App).AsEnumerable();
    }

    /// <summary>
    /// Get information about a user.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    public async Task<User?> GetUserAsync(string? userId = null, CancellationToken ct = default)
    {
        // GET https://api.passage.id/v1/apps/{app_id}/users/{user_id}/
        // Authorization: Bearer (api_key/auth_token)
        // 200/401/404/500
        // check token for validity, for expiry, get refresh if required
        var uri = new Uri($"https://api.passage.id/v1/apps/{_config.AppId}/users/{userId}/");
        using var client = _httpClientFactory.CreateClient(PassageConsts.NamedClient);

        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _config.ApiKey);

        using var response = await client.GetAsync(uri, ct).ConfigureAwait(false);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            throw new PassageException(string.Format(CultureInfo.InvariantCulture, "Passage app with ID \"{0}\" does not exist", _config.AppId), response);
        }

        if (!response.IsSuccessStatusCode)
        {
            throw new PassageException(string.Format(CultureInfo.InvariantCulture, "Failed to get Passage User. StatusCode: {0}, ReasonPhrase: {1}", response.StatusCode, response.ReasonPhrase), response);
        }

        var responseStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        var result = await JsonSerializer.DeserializeAsync<PassageUser>(responseStream, options, ct).ConfigureAwait(false) ?? new();
        return result.User;
    }

    /// <summary>
    /// List users for an app.
    /// </summary>
    /// <param name="appId"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    public Task<IEnumerable<User?>> GetUsersAsync(string? appId = null, CancellationToken ct = default)
    {
        // GET https://api.passage.id/v1/apps/{app_id}/users/
        // Authorization: Bearer (api_key/auth_token)
        // 200/400/401/404/500
        throw new NotImplementedException();
    }

    /// <summary>
    /// Update the settings or authentication configuration for an application.
    /// </summary>
    /// <param name="app"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public Task<App?> UpdateAppAsync(App app, CancellationToken ct = default)
    {
        // PATCH https://api.passage.id/v1/apps/{app_id}/
        // Authorization: Bearer (api_key/auth_token)
        // 200/400/401/404/409/500
        throw new NotImplementedException();
    }
}
