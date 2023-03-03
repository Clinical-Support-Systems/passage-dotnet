using System.Globalization;
using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace PassageIdentity;

public class PassageAuthentication
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger _logger;
    private readonly IPassageConfig _passageConfig;
    private string? _bearerToken = string.Empty;
    private string? _refreshToken = string.Empty;

    public PassageAuthentication(ILogger logger, IHttpClientFactory httpClientFactory, IPassageConfig passageConfig)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
        _passageConfig = passageConfig;
    }

    public Task<User?> ActivateUserAsync(string identifier, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<User?> CreateUserAsync(string? email, string? phone, Dictionary<string, object>? userMetadata = null, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<User?> DeactivateUserAsync(string identifier, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteUserAsync(string identifier, CancellationToken ct = default)
    {
        throw new NotImplementedException();
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
            throw new PassageException(string.Format(CultureInfo.InvariantCulture, "Failed to get Passage User. StatusCode: {0}, ReasonPhrase: {1}", response.StatusCode, response.ReasonPhrase), response);
        }

        var responseStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        var appBody = await JsonSerializer.DeserializeAsync<PassageApp>(responseStream, options, ct).ConfigureAwait(false);
        return appBody?.App;
    }

    public static bool IsValidIdentifier(string identifier)
    {
        if (identifier.IsValidEmail())
        {
            return true;
        }

        if (identifier.IsValidE164())
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Login with Magic Link
    /// <para>Send a login email or SMS to the user. The user will receive an email or text with a link to complete their login.</para>
    /// </summary>
    /// <param name="identifier"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    /// <exception cref="PassageException"></exception>
    public async Task<MagicLink> GetMagicLinkAsync(string identifier, CancellationToken ct = default)
    {
        if (!IsValidIdentifier(identifier))
        {
            throw new ArgumentException("Identifier is not a valid email address or E164 formatted phone number.", nameof(identifier));
        }

        var uri = new Uri($"https://auth.passage.id/v1/apps/{_passageConfig.AppId}/login/magic-link/");
        using var client = _httpClientFactory.CreateClient(PassageConsts.NamedClient);
        var payload = new { identifier };
        var payloadJson = JsonSerializer.Serialize(payload);
        using var request = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = uri,
            Content = new StringContent(payloadJson)
            {
                Headers =
                {
                    ContentType = new MediaTypeHeaderValue("application/json")
                }
            }
        };
        using var response = await client.SendAsync(request, ct).ConfigureAwait(false);
        if (!response.IsSuccessStatusCode)
        {
            throw new PassageException($"Failed to get user '{identifier}'. StatusCode: {response.StatusCode}, ReasonPhrase: {response.ReasonPhrase}", response);
        }

        var responseStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        var result = await JsonSerializer.DeserializeAsync<PassageMagicLink>(responseStream, options, ct).ConfigureAwait(false);
        return result?.MagicLink ?? new();
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
            throw new PassageException(string.Format(CultureInfo.InvariantCulture, $"Failed to get user by identifier '{identifier}'. StatusCode: {response.StatusCode}, ReasonPhrase: {response.ReasonPhrase}"), response);
        }

        var responseStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        var userBody = await JsonSerializer.DeserializeAsync<PassageUser>(responseStream, options, ct).ConfigureAwait(false);
        return userBody?.User;
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
            throw new PassageException(string.Format(CultureInfo.InvariantCulture, "Failed to get Passage User. StatusCode: {0}, ReasonPhrase: {1}", response.StatusCode, response.ReasonPhrase), response);
        }

        using var responseStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        var results = await JsonSerializer.DeserializeAsync<List<Device>>(responseStream, options, ct).ConfigureAwait(false);
        return results ?? new();
    }

    public Task<User?> UpdateUserAsync(string identifier, string? email, string? phone, Dictionary<string, object>? userMetadata = null, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Authenticate a magic link for a user. This endpoint checks that the magic link is valid, then returns an authentication token for the user.
    /// </summary>
    /// <param name="magicLink"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    /// <exception cref="PassageException"></exception>
    public async Task<AuthResult> CompleteMagicLinkLoginAsync(string? magicLink, CancellationToken ct = default)
    {
        var uri = new Uri($"https://auth.passage.id/v1/apps/{_passageConfig.AppId}/magic-link/activate/");
        using var client = _httpClientFactory.CreateClient(PassageConsts.NamedClient);
        var payload = new { magic_link = magicLink };
        var payloadJson = JsonSerializer.Serialize(payload);
        using var request = new HttpRequestMessage
        {
            Method = new HttpMethod("PATCH"),
            RequestUri = uri,
            Content = new StringContent(payloadJson)
            {
                Headers =
        {
            ContentType = new MediaTypeHeaderValue("application/json")
        }
            }
        };
        using var response = await client.SendAsync(request, ct).ConfigureAwait(false);
        if (!response.IsSuccessStatusCode)
        {
            throw new PassageException($"Failed to authenticate using magic link '{magicLink}'. StatusCode: {response.StatusCode}, ReasonPhrase: {response.ReasonPhrase}", response);
        }

        var responseStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        var result = await JsonSerializer.DeserializeAsync<PassageAuthResult>(responseStream, options, ct).ConfigureAwait(false);
        return result?.Result ?? new();
    }
}
