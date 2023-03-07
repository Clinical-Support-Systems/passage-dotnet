using System.Globalization;
using System.Net;
using System.Net.Http.Headers;
using System.Numerics;
using System.Text;
using System.Text.Json;
using System.Web;
using System.Xml.Linq;
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

    /*******************************/
    /************ Users ************/
    /*******************************/

    /// <summary>
    /// Get information about an application.
    /// </summary>
    /// <example>https://docs.passage.id/api-docs/authentication-api/apps</example>
    /// <param name="appId">An App ID if different than the current one.</param>
    /// <param name="ct"></param>
    /// <returns></returns>
    public async Task<App?> GetApp(string? appId = null, CancellationToken ct = default)
    {
        var uri = new Uri($"https://auth.passage.id/v1/apps/{appId ?? _passageConfig.AppId}/");
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

    /*******************************/
    /********* Magic Links *********/
    /*******************************/

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

    /*******************************/
    /************ Users ************/
    /*******************************/

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
        var queryBuilder = HttpUtility.ParseQueryString(string.Empty);
        queryBuilder["identifier"] = identifier;
        var queryString = queryBuilder.ToString();


        var uri = new UriBuilder($"https://auth.passage.id/v1/apps/{_passageConfig.AppId}/users/");
        uri.Query = queryString;

        using var client = _httpClientFactory.CreateClient(PassageConsts.NamedClient);

        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _bearerToken);

        using var response = await client.GetAsync(uri.ToString(), ct).ConfigureAwait(false);
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
        var results = await JsonSerializer.DeserializeAsync<PassageDevies>(responseStream, options, ct).ConfigureAwait(false) ?? new();
        return results.Devices.AsEnumerable();
    }

    /// <summary>
    /// Change Email
    /// Initiate an email change for the authenticated user. An email change requires verification,
    /// so an email will be sent to the user which they must verify before the email change takes effect.
    /// </summary>
    /// <param name="email"></param>
    /// <param name="magicLinkPath"></param>
    /// <param name="redirectPath"></param>
    /// <param name="language"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    /// <exception cref="PassageException"></exception>
    public async Task<User?> ChangeEmail(string email, string magicLinkPath, string redirectPath, string? language = null, CancellationToken ct = default)
    {

        //PATCH https://auth.passage.id/v1/apps/{app_id}/currentuser/email/


        var uri = new Uri($"https://auth.passage.id/v1/apps/{_passageConfig.AppId}/currentuser/email/");
        using var client = _httpClientFactory.CreateClient(PassageConsts.NamedClient);

        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _bearerToken);

        var payload = new { new_email = email, magic_link_path = magicLinkPath, redirect_url = redirectPath, language = language ?? string.Empty };
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
            throw new PassageException($"Failed to change email to '{email}'. StatusCode: {response.StatusCode}, ReasonPhrase: {response.ReasonPhrase}", response);
        }

        var responseStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        var result = await JsonSerializer.DeserializeAsync<PassageUser>(responseStream, options, ct).ConfigureAwait(false) ?? new();
        return result.User ?? new();
    }

    /// <summary>
    /// Change Phone
    /// Initiate a phone number change for the authenticated user.An phone number change requires verification,
    /// so an SMS with a link will be sent to the user which they must verify before the phone number change takes effect.
    /// </summary>
    /// <param name="phone"></param>
    /// <param name="magicLinkPath"></param>
    /// <param name="redirectPath"></param>
    /// <param name="language"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    /// <exception cref="PassageException"></exception>
    public async Task<User?> ChangePhone(string phone, string magicLinkPath, string redirectPath, string? language = null, CancellationToken ct = default)
    {

        //PATCH https://auth.passage.id/v1/apps/{app_id}/currentuser/phone/


        var uri = new Uri($"https://auth.passage.id/v1/apps/{_passageConfig.AppId}/currentuser/phone/");
        using var client = _httpClientFactory.CreateClient(PassageConsts.NamedClient);

        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _bearerToken);

        var payload = new { new_phone = phone, magic_link_path = magicLinkPath, redirect_url = redirectPath, language = language ?? string.Empty };
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
            throw new PassageException($"Failed to change phone to '{phone}'. StatusCode: {response.StatusCode}, ReasonPhrase: {response.ReasonPhrase}", response);
        }

        var responseStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        var result = await JsonSerializer.DeserializeAsync<PassageUser>(responseStream, options, ct).ConfigureAwait(false) ?? new();
        return result.User ?? new();
    }

    /// <summary>
    ///Update Device
    /// Update a device by ID for the current user.
    /// Currently the only field that can be updated is the friendly name.User must be authenticated via a bearer token.
    /// </summary>
    /// <param name="deviceId"></param>
    /// <param name="friendlyName"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    /// <exception cref="PassageException"></exception>
    public async Task<User?> UpdateDevice(string deviceId, string friendlyName, CancellationToken ct = default)
    {

        //PATCH https://auth.passage.id/v1/apps/{app_id}/currentuser/devices/{device_id}/


        var uri = new Uri($"https://auth.passage.id/v1/apps/{_passageConfig.AppId}/currentuser/devices/{deviceId}");
        using var client = _httpClientFactory.CreateClient(PassageConsts.NamedClient);

        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _bearerToken);

        var payload = new { friendly_name = friendlyName };
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
            throw new PassageException($"Failed to change device to '{deviceId}'. StatusCode: {response.StatusCode}, ReasonPhrase: {response.ReasonPhrase}", response);
        }

        var responseStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        var result = await JsonSerializer.DeserializeAsync<PassageUser>(responseStream, options, ct).ConfigureAwait(false) ?? new();
        return result.User ?? new();
    }

    /// <summary>
    /// Revoke a device by ID for the current user. User must be authenticated via a bearer token.
    /// </summary>
    /// <param name="deviceId"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    /// <exception cref="PassageException"></exception>
    public async Task RevokeDevice(string deviceId, CancellationToken ct = default)
    {
        //PATCH https://auth.passage.id/v1/apps/{app_id}/currentuser/devices/{device_id}/

        var uri = new Uri($"https://auth.passage.id/v1/apps/{_passageConfig.AppId}/currentuser/devices/{deviceId}");
        using var client = _httpClientFactory.CreateClient(PassageConsts.NamedClient);

        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _bearerToken);

        using var response = await client.DeleteAsync(uri, ct).ConfigureAwait(false);
        if (!response.IsSuccessStatusCode)
        {
            throw new PassageException($"Failed to revoke device to '{deviceId}'. StatusCode: {response.StatusCode}, ReasonPhrase: {response.ReasonPhrase}", response);
        }

        var responseStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }


    /// <summary>
    /// Start WebAuthn Add Device
    /// Initiate a WebAuthn add device operation for the current user.
    /// This endpoint creates a WebAuthn credential creation challenge that is used to perform the registration ceremony from the browser.
    /// User must be authenticated via a bearer token.
    /// </summary>
    /// <param name="ct"></param>
    /// <returns></returns>
    /// <exception cref="PassageException"></exception>
    public async Task AddDeviceStart(CancellationToken ct = default)
    {
        //POST https://auth.passage.id/v1/apps/{app_id}/currentuser/devices/start/

        var uri = new Uri($"https://auth.passage.id/v1/apps/{_passageConfig.AppId}/currentuser/devices/start/");
        using var client = _httpClientFactory.CreateClient(PassageConsts.NamedClient);

        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _bearerToken);
        using (StringContent stringContent = new StringContent(string.Empty, UnicodeEncoding.UTF8, "application/json"))
        {

            using var response = await client.PostAsync(uri, stringContent, ct).ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
            {
                throw new PassageException($"Failed to start Add Device for App '{_passageConfig.AppId}'. StatusCode: {response.StatusCode}, ReasonPhrase: {response.ReasonPhrase}", response);
            }

            var responseStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            var result = await JsonSerializer.DeserializeAsync<PassageWebAuthLogin>(responseStream, options, ct).ConfigureAwait(false);
            //return result ?? new();
        }

    }

    /// <summary>
    /// Finish WebAuthn Add Device
    /// Complete a WebAuthn add device operation for the current user.
    /// This endpoint accepts and verifies the response from navigator.credential.create() and returns the created device for the user.
    /// User must be authenticated via a bearer token.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="credType"></param>
    /// <param name="handShakeId"></param>
    /// <param name="handshakeResponse"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    /// <exception cref="PassageException"></exception>
    public async Task AddDeviceFinish(string userId, string credType, string handShakeId, string handshakeResponse, CancellationToken ct = default)
    {
        //POST https://auth.passage.id/v1/apps/{app_id}/currentuser/devices/finish/

        var uri = new Uri($"https://auth.passage.id/v1/apps/{_passageConfig.AppId}/currentuser/devices/start/");
        using var client = _httpClientFactory.CreateClient(PassageConsts.NamedClient);

        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _bearerToken);

        var payload = new { cred_type = credType, handshake_id = handShakeId, handshake_response = handshakeResponse };
        var jsonContent = JsonSerializer.Serialize(payload);
        using (StringContent stringContent = new StringContent(jsonContent, UnicodeEncoding.UTF8, "application/json"))
        {

            using var response = await client.PostAsync(uri, stringContent, ct).ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
            {
                throw new PassageException($"Failed to finish Add Device for App '{_passageConfig.AppId}'. StatusCode: {response.StatusCode}, ReasonPhrase: {response.ReasonPhrase}", response);
            }

            var responseStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            var result = await JsonSerializer.DeserializeAsync<PassageWebAuthLogin>(responseStream, options, ct).ConfigureAwait(false);
            //return result ?? new();
        }

    }

    /*******************************/
    /********** Web Authn **********/
    /*******************************/

    /// <summary>
    /// Initiate a WebAuthn login for a user. This endpoint creates a WebAuthn credential assertion challenge that is used to perform the login ceremony from the browser.
    /// </summary>
    /// <param name="ct"></param>
    /// <returns></returns>
    /// <exception cref="PassageException"></exception>
    public async Task<PassageWebAuthLogin> WebAuthLoginStart(CancellationToken ct = default)
    {
        var uri = new Uri($"https://auth.passage.id/v1/apps/{_passageConfig.AppId}/login/webauthn/start/");
        using var client = _httpClientFactory.CreateClient(PassageConsts.NamedClient);

        var payload = new { refresh_token = _refreshToken ?? string.Empty };
        var jsonContent = JsonSerializer.Serialize(payload);
        using (StringContent stringContent = new StringContent(jsonContent, UnicodeEncoding.UTF8, "application/json"))
        {

            using var response = await client.PostAsync(uri, stringContent, ct).ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
            {
                throw new PassageException($"Failed to start WebAuth for App '{_passageConfig.AppId}'. StatusCode: {response.StatusCode}, ReasonPhrase: {response.ReasonPhrase}", response);
            }

            var responseStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            var result = await JsonSerializer.DeserializeAsync<PassageWebAuthLogin>(responseStream, options, ct).ConfigureAwait(false);
            return result ?? new();
        }
    }



    /// <summary>
    /// Initiate a WebAuthn login for a user. This endpoint creates a WebAuthn credential assertion challenge that is used to perform the login ceremony from the browser.
    /// </summary>
    /// <param name="ct"></param>
    /// <returns></returns>
    /// <exception cref="PassageException"></exception>
    public async Task WebAuthLoginFinish(CancellationToken ct = default)
    {
        var uri = new Uri($"https://auth.passage.id/v1/apps/{_passageConfig.AppId}/login/webauthn/finish/");
        using var client = _httpClientFactory.CreateClient(PassageConsts.NamedClient);

        var payload = new { refresh_token = _refreshToken ?? string.Empty };
        var jsonContent = JsonSerializer.Serialize(payload);
        using (StringContent stringContent = new StringContent(jsonContent, UnicodeEncoding.UTF8, "application/json"))
        {

            using var response = await client.PostAsync(uri, stringContent, ct).ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
            {
                throw new PassageException($"Failed to finish WebAuth for App '{_passageConfig.AppId}'. StatusCode: {response.StatusCode}, ReasonPhrase: {response.ReasonPhrase}", response);
            }

            var responseStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            var result = await JsonSerializer.DeserializeAsync<PassageWebAuthLogin>(responseStream, options, ct).ConfigureAwait(false);
            //return result ?? new();
        }
    }

    /// <summary>
    /// Initiate a WebAuthn registration and create the user. This endpoint creates a WebAuthn credential creation challenge that is used to perform the registration ceremony from the browser.
    /// </summary>
    /// <param name="identifier"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    /// <exception cref="PassageException"></exception>
    public async Task<PassageWebAuthLogin> WebAuthRegisterStart(string identifier, CancellationToken ct = default)
    {
        var uri = new Uri($"https://auth.passage.id/v1/apps/{_passageConfig.AppId}/register/webauthn/start/");
        using var client = _httpClientFactory.CreateClient(PassageConsts.NamedClient);

        if (string.IsNullOrWhiteSpace(identifier)) throw new PassageException("Identifier required (valid email or phone number)");

        var payload = new { identifier = identifier };
        var jsonContent = JsonSerializer.Serialize(payload);
        using (StringContent stringContent = new StringContent(jsonContent, UnicodeEncoding.UTF8, "application/json"))
        {

            using var response = await client.PostAsync(uri, stringContent, ct).ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
            {
                throw new PassageException($"Failed to start WebAuth for App '{_passageConfig.AppId}'. StatusCode: {response.StatusCode}, ReasonPhrase: {response.ReasonPhrase}", response);
            }

            var responseStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            var result = await JsonSerializer.DeserializeAsync<PassageWebAuthLogin>(responseStream, options, ct).ConfigureAwait(false);
            return result ?? new();
        }
    }

    /// <summary>
    /// Finish WebAuthn Registration
    /// Complete a WebAuthn registration and authenticate the user.This endpoint accepts and verifies the response from navigator.credential.create() and returns an authentication token for the user.
    /// </summary>
    /// <param name="identifier"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    /// <exception cref="PassageException"></exception>
    public async Task<PassageWebAuthLogin> WebAuthRegisterFinish(string identifier, CancellationToken ct = default)
    {
        var uri = new Uri($"https://auth.passage.id/v1/apps/{_passageConfig.AppId}/register/webauthn/finish/");
        using var client = _httpClientFactory.CreateClient(PassageConsts.NamedClient);

        if (string.IsNullOrWhiteSpace(identifier)) throw new PassageException("Identifier required (valid email or phone number)");

        var payload = new { user_id = identifier };
        var jsonContent = JsonSerializer.Serialize(payload);
        using (StringContent stringContent = new StringContent(jsonContent, UnicodeEncoding.UTF8, "application/json"))
        {

            using var response = await client.PostAsync(uri, stringContent, ct).ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
            {
                throw new PassageException($"Failed to finish WebAuth for App '{_passageConfig.AppId}'. StatusCode: {response.StatusCode}, ReasonPhrase: {response.ReasonPhrase}", response);
            }

            var responseStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            var result = await JsonSerializer.DeserializeAsync<PassageWebAuthLogin>(responseStream, options, ct).ConfigureAwait(false);
            return result ?? new();
        }
    }

    /*******************************/
    /***** Session Management ******/
    /*******************************/

    /// <summary>
    /// Creates new auth and refresh token
    /// Creates and returns a new auth token and a new refresh token
    /// </summary>
    /// <param name="refreshToken"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
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
    /// Revokes the refresh token
    /// </summary>
    /// <param name="ct"></param>
    /// <returns></returns>
    /// <exception cref="PassageException"></exception>
    public async Task<User?> DeleteRefreshToken(CancellationToken ct = default)
    {
        var queryBuilder = HttpUtility.ParseQueryString(string.Empty);
        queryBuilder["refresh_token"] = _refreshToken;
        var queryString = queryBuilder.ToString();


        var uri = new UriBuilder($"https://api.passage.id/v1/apps/{_passageConfig.AppId}/tokens/");
        uri.Query = queryString;

        using var client = _httpClientFactory.CreateClient(PassageConsts.NamedClient);

        //client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", ConfigAPIKey);

        using var response = await client.DeleteAsync(uri.ToString(), ct).ConfigureAwait(false);
        if (!response.IsSuccessStatusCode)
        {
            throw new PassageException(string.Format(CultureInfo.InvariantCulture, $"Failed to delete the refresh token. StatusCode: {response.StatusCode}, ReasonPhrase: {response.ReasonPhrase}"), response);
        }

        var responseStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        var userBody = await JsonSerializer.DeserializeAsync<PassageUser>(responseStream, options, ct).ConfigureAwait(false);
        return userBody?.User;
    }
}
