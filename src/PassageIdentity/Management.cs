using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Web;
using Microsoft.Extensions.Logging;

namespace PassageIdentity;

public class PassageManagement
{
    private readonly IPassageConfig _config;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger _logger;

    public PassageManagement(ILogger logger, IHttpClientFactory httpClientFactory, IPassageConfig config)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
        _config = config;
    }

    public AuthResult? Auth { get; set; }

    /// <summary>
    /// Assign an app that is currently in test mode to the current authorized user.
    /// Test mode apps are created when an App ID is not specified in the Passage
    /// custom element.
    /// </summary>
    /// <param name="ct"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task ClaimTestAppAsync(string? name = null, CancellationToken ct = default)
    {
        // POST https://api.passage.id/v1/apps/{app_id}/claim/
        // Authorization: Bearer (key/jwt)
        // 200/400/401/404/500
        try
        {
            var uri = new Uri($"https://api.passage.id/v1/apps/{_config.AppId}/claim/");
            using var client = _httpClientFactory.CreateClient(PassageConsts.NamedClient);

            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _config.ApiKey);

            var content = new List<KeyValuePair<string, string>>();
            if (!string.IsNullOrEmpty(name))
            {
                content.Add(new KeyValuePair<string, string>("name", name!));
            }
            using var form = new FormUrlEncodedContent(content);

            using var response = await client.PostAsync(uri, form, ct).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                throw new PassageException(string.Format(CultureInfo.InvariantCulture, "Failed to assign Claim. StatusCode: {0}, ReasonPhrase: {1}", response.StatusCode, response.ReasonPhrase), response);
            }
        }
        catch (Exception)
        {
            throw;
        }
    }

    /// <summary>
    /// Create a new application. If not authenticated, the app is created in test
    /// mode and not assigned to a specific user.
    /// </summary>
    /// <param name="app"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<App?> CreateAppAsync(App app, CancellationToken ct = default)
    {
        // POST https://api.passage.id/v1/apps/
        // Authorization: Bearer (key/jwt)
        // 200/400/409/500

        try
        {
            var uri = new Uri($"https://api.passage.id/v1/apps/");
            using var client = _httpClientFactory.CreateClient(PassageConsts.NamedClient);

            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _config.ApiKey);

            if (app == null)
            {
                throw new PassageException(string.Format(CultureInfo.InvariantCulture, "Passage app must have content"));
            }

            var jsonContent = JsonSerializer.Serialize(app);
            using (StringContent stringContent = new StringContent(jsonContent, UnicodeEncoding.UTF8, "application/json"))
            {
                using var response = await client.PostAsync(uri, stringContent, ct).ConfigureAwait(false);

                if (!response.IsSuccessStatusCode)
                {
                    throw new PassageException(string.Format(CultureInfo.InvariantCulture, "Failed to Create Passage App. StatusCode: {0}, ReasonPhrase: {1}", response.StatusCode, response.ReasonPhrase), response);
                }

                var responseStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                var result = await JsonSerializer.DeserializeAsync<PassageApp>(responseStream, options, ct).ConfigureAwait(false) ?? new();
                return result.App;
            }
        }
        catch (Exception)
        {
            throw;
        }
    }

    /// <summary>
    /// Get app information
    /// </summary>
    /// <param name="ct"></param>
    /// <returns></returns>
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
        // GET https://api.passage.id/v1/apps/
        // Authorization: Bearer (api_key)
        // 200/401/500
        var uri = new Uri($"https://api.passage.id/v1/apps/");
        using var client = _httpClientFactory.CreateClient(PassageConsts.NamedClient);

        //var content = new List<KeyValuePair<string, string>>();
        //content.Add(new KeyValuePair<string, string>("refresh_token", ""));
        //using var refresh_token = new FormUrlEncodedContent(content);

        //using var token = await client.PostAsync(new Uri($"https://auth.passage.id/v1/apps/passage/login/webauthn/start/"), refresh_token, ct).ConfigureAwait(false);

        //client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _config.ApiKey);

        using var response = await client.GetAsync(uri, ct).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
        {
            throw new PassageException(string.Format(CultureInfo.InvariantCulture, "Failed to get Passge Apps. StatusCode: {0}, ReasonPhrase: {1}", response.StatusCode, response.ReasonPhrase), response);
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
    /// Update the settings or authentication configuration for an application.
    /// </summary>
    /// <param name="app"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<App?> UpdateAppAsync(App app, CancellationToken ct = default)
    {
        // PATCH https://api.passage.id/v1/apps/{app_id}/
        // Authorization: Bearer (api_key/auth_token)
        // 200/400/401/404/409/500

        var uri = new Uri($"https://api.passage.id/v1/apps/{_config.AppId}/");
        using var client = _httpClientFactory.CreateClient(PassageConsts.NamedClient);


        if (app == null)
        {
            throw new PassageException(string.Format(CultureInfo.InvariantCulture, "Passage app cannot be null"));
        }

        var payloadJson = JsonSerializer.Serialize(app);
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

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            throw new PassageException(string.Format(CultureInfo.InvariantCulture, "Passage app with ID \"{0}\" does not exist", app.Id), response);
        }

        if (!response.IsSuccessStatusCode)
        {
            throw new PassageException(string.Format(CultureInfo.InvariantCulture, "Failed to get Update Passage App. StatusCode: {0}, ReasonPhrase: {1}", response.StatusCode, response.ReasonPhrase), response);
        }

        var responseStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        var result = await JsonSerializer.DeserializeAsync<PassageApp>(responseStream, options, ct).ConfigureAwait(false) ?? new();
        return result.App;

    }

    /// <summary>
    /// Delete an application.
    /// </summary>
    /// <returns></returns>
    public async Task DeleteAppAsync(string appId, CancellationToken ct = default)
    {
        // GET https://api.passage.id/v1/apps/{app_id}/
        // Authorization: Bearer (api_key/auth_token)
        // 200/401/404/500

        try
        {
            var uri = new Uri($"https://api.passage.id/v1/apps/{appId}/");
            using var client = _httpClientFactory.CreateClient(PassageConsts.NamedClient);

            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _config.ApiKey);

            using var response = await client.DeleteAsync(uri, ct).ConfigureAwait(false);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                throw new PassageException(string.Format(CultureInfo.InvariantCulture, "Passage app with ID \"{0}\" does not exist", appId), response);
            }

            if (!response.IsSuccessStatusCode)
            {
                throw new PassageException(string.Format(CultureInfo.InvariantCulture, "Failed to Delete Passage App. StatusCode: {0}, ReasonPhrase: {1}", response.StatusCode, response.ReasonPhrase), response);
            }


        }
        catch (Exception)
        {
            throw;
        }
    }


    /************************/
    /*******   USERS  *******/
    /************************/


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
            throw new PassageException(string.Format(CultureInfo.InvariantCulture, "Passage User with ID \"{0}\" does not exist", userId), response);
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
    /// Create user for an application. Must provide an email of phone number identifier.
    /// </summary>
    /// <param name="user"></param>
    /// <param name="appId"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    public async Task<User?> CreateUserAsync(User user, string? appId = null, CancellationToken ct = default)
    {
        // POST https://api.passage.id/v1/apps/{app_id}/users/
        // Authorization: Bearer (api_key/auth_token)
        // 200/400/401/409/500

        try
        {
            var uri = new Uri($"https://api.passage.id/v1/apps/{appId}/users/");
            using var client = _httpClientFactory.CreateClient(PassageConsts.NamedClient);

            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _config.ApiKey);

            if (user == null)
            {
                throw new PassageException(string.Format(CultureInfo.InvariantCulture, "Passage app must have content"));
            }

            var jsonContent = JsonSerializer.Serialize(user);
            using (StringContent stringContent = new StringContent(jsonContent, UnicodeEncoding.UTF8, "application/json"))
            {
                using var response = await client.PostAsync(uri, stringContent, ct).ConfigureAwait(false);

                if (!response.IsSuccessStatusCode)
                {
                    throw new PassageException(string.Format(CultureInfo.InvariantCulture, "Failed to Create Passage User. StatusCode: {0}, ReasonPhrase: {1}", response.StatusCode, response.ReasonPhrase), response);
                }

                var responseStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                var result = await JsonSerializer.DeserializeAsync<PassageUser>(responseStream, options, ct).ConfigureAwait(false) ?? new();
                return result.User;
            }
        }
        catch (Exception)
        {
            throw;
        }
    }

    /// <summary>
    /// Bulk upload users for an application. Files must be in valid CSV format and no more than 30k users.
    /// </summary>
    /// <param name="csvData">key: csv_user_import, file type: text/csv</param>
    /// <param name="appId"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    public async Task<IEnumerable<User?>> CreateUsersAsync(byte[] csvData, string? appId = null, CancellationToken ct = default)
    {
        // POST https://api.passage.id/v1/apps/{app_id}/import/users/
        // Authorization: Bearer (api_key/auth_token)
        // 200/400/401/500
        try
        {
            var uri = new Uri($"https://api.passage.id/v1/apps/{_config.AppId}/import/users/");
            using var client = _httpClientFactory.CreateClient(PassageConsts.NamedClient);

            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _config.ApiKey);

            using (ByteArrayContent byteArrayContent = new ByteArrayContent(csvData))
            {
                byteArrayContent.Headers.ContentType = new MediaTypeHeaderValue("application/bson");
                using var response = await client.PostAsync(uri, byteArrayContent, ct).ConfigureAwait(false);

                if (!response.IsSuccessStatusCode)
                {
                    throw new PassageException(string.Format(CultureInfo.InvariantCulture, "Failed to Create Passage Users. StatusCode: {0}, ReasonPhrase: {1}", response.StatusCode, response.ReasonPhrase), response);
                }

                var responseStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                var result = await JsonSerializer.DeserializeAsync<PassageUserList>(responseStream, options, ct).ConfigureAwait(false) ?? new();
                return result.Users;
            }
        }
        catch (Exception)
        {
            throw;
        }
    }

    /// <summary>
    /// List users for an app.
    /// </summary>
    /// <param name="query"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    public async Task<IEnumerable<User?>> GetUsersAsync(ListPassageUsersQuery query, CancellationToken ct = default)
    {
        // GET https://api.passage.id/v1/apps/{app_id}/users/
        // Authorization: Bearer (api_key/auth_token)
        // 200/400/401/404/500

        //build query string
        string queryString = string.Empty;
        if (query != null)
        {
            var queryBuilder = HttpUtility.ParseQueryString(string.Empty);
            queryBuilder["page"] = $"{query.Page}";
            queryBuilder["limit"] = $"{query.Limit ?? 500}";
            queryBuilder["created_before"] = query.CreatedBefore.HasValue ? $"{((DateTimeOffset)query.CreatedBefore.Value).ToUnixTimeSeconds()}" : string.Empty;
            queryBuilder["order_by"] = query.OrderBy ?? string.Empty;
            queryBuilder["identifier"] = query.Identifier ?? string.Empty;
            queryBuilder["id"] = query.Id ?? string.Empty;
            queryBuilder["login_count"] = query.LoginCount ?? string.Empty;
            queryBuilder["status"] = query.Status ?? string.Empty;
            queryBuilder["email_verified"] = query.EmailVerified ?? string.Empty;
            queryBuilder["created_at"] = query.CreatedAt?.ToString("u") ?? string.Empty;
            queryBuilder["updated_at"] = query.UpdatedAt?.ToString("u") ?? string.Empty;
            queryBuilder["last_login_at"] = query.LastLoginAt?.ToString("u") ?? string.Empty;
            queryString = queryBuilder.ToString();
        }

        var uri = new UriBuilder($"https://api.passage.id/v1/apps/{_config.AppId}/users/");
        uri.Query = queryString;
        using var client = _httpClientFactory.CreateClient(PassageConsts.NamedClient);

        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _config.ApiKey);


        using var response = await client.GetAsync(uri.ToString(), ct).ConfigureAwait(false);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            throw new PassageException(string.Format(CultureInfo.InvariantCulture, "Passage app with ID \"{0}\" does not exist", _config.AppId), response);
        }

        if (!response.IsSuccessStatusCode)
        {
            throw new PassageException(string.Format(CultureInfo.InvariantCulture, "Failed to get Passage Users. StatusCode: {0}, ReasonPhrase: {1}", response.StatusCode, response.ReasonPhrase), response);
        }

        var responseStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        var result = await JsonSerializer.DeserializeAsync<PassageUserList>(responseStream, options, ct).ConfigureAwait(false) ?? new();
        return result.Users;
    }

    /// <summary>
    /// Update a user's information.
    /// </summary>
    /// <param name="user"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<User?> UpdateUserAsync(User user, CancellationToken ct = default)
    {
        // PATCH https://api.passage.id/v1/apps/{app_id}/users/{user_id}/
        // Authorization: Bearer (api_key/auth_token)
        // 200/400/401/404/409/500

        if (user == null)
        {
            throw new PassageException("User cannot be null.");
        }

        var uri = new Uri($"https://api.passage.id/v1/apps/{_config.AppId}/users/{user.Id}/");
        using var client = _httpClientFactory.CreateClient(PassageConsts.NamedClient);

        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _config.ApiKey);

        var payloadJson = JsonSerializer.Serialize(user);
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

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            throw new PassageException(string.Format(CultureInfo.InvariantCulture, "Passage user with ID \"{0}\" does not exist", user.Id), response);
        }

        if (!response.IsSuccessStatusCode)
        {
            throw new PassageException(string.Format(CultureInfo.InvariantCulture, "Failed to update Passage User. StatusCode: {0}, ReasonPhrase: {1}", response.StatusCode, response.ReasonPhrase), response);
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
    /// Activate a user. They will now be able to login.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<User?> ActivateUserAsync(string userId, CancellationToken ct = default)
    {
        // PATCH https://api.passage.id/v1/apps/{app_id}/users/{user_id}/activate
        // Authorization: Bearer (api_key/auth_token)
        // 200/401/404/409/500

        var uri = new Uri($"https://api.passage.id/v1/apps/{_config.AppId}/users/{userId}/activate");
        using var client = _httpClientFactory.CreateClient(PassageConsts.NamedClient);

        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _config.ApiKey);

        var payload = new { status = "active" };
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

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            throw new PassageException(string.Format(CultureInfo.InvariantCulture, "Passage user with ID \"{0}\" does not exist", userId), response);
        }

        if (!response.IsSuccessStatusCode)
        {
            throw new PassageException(string.Format(CultureInfo.InvariantCulture, "Failed to get Activate User. StatusCode: {0}, ReasonPhrase: {1}", response.StatusCode, response.ReasonPhrase), response);
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
    /// Deactivate a user. Their account will still exist, but they will not be able to login.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<User?> DeactivateUserAsync(string userId, CancellationToken ct = default)
    {
        // PATCH https://api.passage.id/v1/apps/{app_id}/users/{user_id}/deactivate
        // Authorization: Bearer (api_key/auth_token)
        // 200/401/404/409/500

        var uri = new Uri($"https://api.passage.id/v1/apps/{_config.AppId}/users/{userId}/deactivate");
        using var client = _httpClientFactory.CreateClient(PassageConsts.NamedClient);

        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _config.ApiKey);

        var payload = new { status = "inactive" };
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

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            throw new PassageException(string.Format(CultureInfo.InvariantCulture, "Passage user with ID \"{0}\" does not exist", userId), response);
        }

        if (!response.IsSuccessStatusCode)
        {
            throw new PassageException(string.Format(CultureInfo.InvariantCulture, "Failed to Deactivate user. StatusCode: {0}, ReasonPhrase: {1}", response.StatusCode, response.ReasonPhrase), response);
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
    /// Delete a user.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    public async Task DeleteUserAsync(string userId, CancellationToken ct = default)
    {
        // DELETE https://api.passage.id/v1/apps/{app_id}/users/{user_id}/
        // Authorization: Bearer (api_key/auth_token)
        // 200/401/404/500

        try
        {
            var uri = new Uri($"https://api.passage.id/v1/apps/{_config.AppId}/users/{userId}");
            using var client = _httpClientFactory.CreateClient(PassageConsts.NamedClient);

            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _config.ApiKey);

            using var response = await client.DeleteAsync(uri, ct).ConfigureAwait(false);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                throw new PassageException(string.Format(CultureInfo.InvariantCulture, "Passage User with ID \"{0}\" does not exist", userId), response);
            }

            if (!response.IsSuccessStatusCode)
            {
                throw new PassageException(string.Format(CultureInfo.InvariantCulture, "Failed to Delete User. StatusCode: {0}, ReasonPhrase: {1}", response.StatusCode, response.ReasonPhrase), response);
            }


        }
        catch (Exception)
        {
            throw;
        }
    }

    /// <summary>
    /// Revokes all refresh tokens for a user
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    public async Task RevokeTokensAsync(string userId, CancellationToken ct = default)
    {
        // DELETE https://api.passage.id/v1/apps/{app_id}/users/{user_id}/tokens/
        // Authorization: Bearer (api_key/auth_token)
        // 200/401/404/500

        try
        {
            var uri = new Uri($"https://api.passage.id/v1/apps/{_config.AppId}/users/{userId}/tokens/");
            using var client = _httpClientFactory.CreateClient(PassageConsts.NamedClient);

            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _config.ApiKey);

            using var response = await client.DeleteAsync(uri, ct).ConfigureAwait(false);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                throw new PassageException(string.Format(CultureInfo.InvariantCulture, "Passage user with ID \"{0}\" does not exist", userId), response);
            }

            if (!response.IsSuccessStatusCode)
            {
                throw new PassageException(string.Format(CultureInfo.InvariantCulture, "Failed to revoke Tokens. StatusCode: {0}, ReasonPhrase: {1}", response.StatusCode, response.ReasonPhrase), response);
            }


        }
        catch (Exception)
        {
            throw;
        }
    }


    /***************************/
    /*******   API Keys  *******/
    /***************************/

    /// <summary>
    /// List API keys for an application.
    /// </summary>
    /// <param name="ct"></param>
    /// <returns></returns>
    public async Task<List<ApiKey>> GetAPIKeysAsync(CancellationToken ct = default)
    {
        try
        {
            // GET https://api.passage.id/v1/apps/{app_id}/api-keys/
            // Authorization: Bearer (api_key)
            // 200/500
            var uri = new Uri($"https://api.passage.id/v1/apps/{_config.AppId}/api-keys/");
            using var client = _httpClientFactory.CreateClient(PassageConsts.NamedClient);

            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _config.ApiKey);

            using var response = await client.GetAsync(uri, ct).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                throw new PassageException(string.Format(CultureInfo.InvariantCulture, "Failed to get Passage API Key. StatusCode: {0}, ReasonPhrase: {1}", response.StatusCode, response.ReasonPhrase), response);
            }

            var responseStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            var result = await JsonSerializer.DeserializeAsync<PassageApiKeys>(responseStream, options, ct).ConfigureAwait(false) ?? new();
            return result.ApiKeys.ToList();
        }
        catch (Exception)
        {
            throw;
        }
    }

    /// <summary>
    /// List API keys for an application.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    public async Task<ApiKey?> CreateAPIKey(string name, CancellationToken ct = default)
    {
        try
        {
            // POST https://api.passage.id/v1/apps/{app_id}/api-keys/
            // Authorization: Bearer (api_key)
            // 200/400/401/409/500
            var uri = new Uri($"https://api.passage.id/v1/apps/{_config.AppId}/api-keys/");
            using var client = _httpClientFactory.CreateClient(PassageConsts.NamedClient);

            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _config.ApiKey);

            if (string.IsNullOrEmpty(name))
            {
                throw new PassageException(string.Format(CultureInfo.InvariantCulture, "Passage API key must have content"));
            }

            var content = new { name = name };
            var jsonContent = JsonSerializer.Serialize(content);
            using (StringContent stringContent = new StringContent(jsonContent, UnicodeEncoding.UTF8, "application/json"))
            {
                using var response = await client.PostAsync(uri, stringContent, ct).ConfigureAwait(false);

                if (!response.IsSuccessStatusCode)
                {
                    throw new PassageException(string.Format(CultureInfo.InvariantCulture, "Failed to Create Passage API Key. StatusCode: {0}, ReasonPhrase: {1}", response.StatusCode, response.ReasonPhrase), response);
                }

                var responseStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                var result = await JsonSerializer.DeserializeAsync<PassageApiKey>(responseStream, options, ct).ConfigureAwait(false) ?? new();
                return result.ApiKey;
            }
        }
        catch (Exception)
        {
            throw;
        }
    }

    /// <summary>
    /// Delete an API key.
    /// </summary>
    /// <param name="apiKeyId"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    public async Task DeleteAPIKeyAsync(string apiKeyId, CancellationToken ct = default)
    {
        // DELETE https://api.passage.id/v1/apps/{app_id}/api-keys/{api_key_id}
        // Authorization: Bearer (api_key/auth_token)
        // 200/401/404/500

        try
        {
            var uri = new Uri($"https://api.passage.id/v1/apps/{_config.AppId}/api-keys/{apiKeyId}");
            using var client = _httpClientFactory.CreateClient(PassageConsts.NamedClient);

            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _config.ApiKey);

            using var response = await client.DeleteAsync(uri, ct).ConfigureAwait(false);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                throw new PassageException(string.Format(CultureInfo.InvariantCulture, "Passage API Key with ID \"{0}\" does not exist", apiKeyId), response);
            }

            if (!response.IsSuccessStatusCode)
            {
                throw new PassageException(string.Format(CultureInfo.InvariantCulture, "Failed to get API Keys. StatusCode: {0}, ReasonPhrase: {1}", response.StatusCode, response.ReasonPhrase), response);
            }


        }
        catch (Exception)
        {
            throw;
        }
    }


    /*************************/
    /*******   Admins  *******/
    /*************************/
    /// <summary>
    /// List admins for an application.
    /// </summary>
    /// <param name="ct">Cancellation token</param>
    /// <returns>The app</returns>
    /// <exception cref="PassageException"></exception>
    public async Task<IEnumerable<AdminMember?>> GetAdminsAsync(CancellationToken ct = default)
    {
        // GET https://api.passage.id/v1/apps/{app_id}/admins/
        // Authorization: Bearer (api_key)
        // 200/400/401/500
        var uri = new Uri($"https://api.passage.id/v1/apps/{_config.AppId}/admins");
        using var client = _httpClientFactory.CreateClient(PassageConsts.NamedClient);

        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _config.ApiKey);

        using var response = await client.GetAsync(uri, ct).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
        {
            throw new PassageException(string.Format(CultureInfo.InvariantCulture, "Failed to get Passge Admins. StatusCode: {0}, ReasonPhrase: {1}", response.StatusCode, response.ReasonPhrase), response);
        }

        var responseStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        var appBody = await JsonSerializer.DeserializeAsync<PassageAdmins>(responseStream, options, ct).ConfigureAwait(false) ?? new PassageAdmins();
        return appBody.Admins.AsEnumerable();
    }

    /// <summary>
    /// List admins for an application.
    /// </summary>
    /// <param name="adminId">Cancellation token</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>The app</returns>
    /// <exception cref="PassageException"></exception>
    public async Task<PassageAdmin?> GetAdminAsync(string adminId, CancellationToken ct = default)
    {
        // GET https://api.passage.id/v1/apps/{app_id}/admins/{admin_id}/
        // Authorization: Bearer (api_key)
        // 200/400/401/500
        var uri = new Uri($"https://api.passage.id/v1/apps/{_config.AppId}/admins/{adminId}");
        using var client = _httpClientFactory.CreateClient(PassageConsts.NamedClient);

        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _config.ApiKey);

        using var response = await client.GetAsync(uri, ct).ConfigureAwait(false);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            throw new PassageException(string.Format(CultureInfo.InvariantCulture, "Passage Admin with ID \"{0}\" does not exist", adminId), response);
        }

        if (!response.IsSuccessStatusCode)
        {
            throw new PassageException(string.Format(CultureInfo.InvariantCulture, "Failed to get Admin. StatusCode: {0}, ReasonPhrase: {1}", response.StatusCode, response.ReasonPhrase), response);
        }

        var responseStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        var adminUser = await JsonSerializer.DeserializeAsync<PassageAdmin>(responseStream, options, ct).ConfigureAwait(false);
        return adminUser;
    }

    /// <summary>
    /// Create an admin for an application. Requires an email address.
    /// </summary>
    /// <param name="email"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<AdminMember?> CreateAdminAsync(string email, CancellationToken ct = default)
    {
        // POST https://api.passage.id/v1/apps/
        // Authorization: Bearer (key/jwt)
        // 200/400/409/500

        try
        {
            var uri = new Uri($"https://api.passage.id/v1/apps/");
            using var client = _httpClientFactory.CreateClient(PassageConsts.NamedClient);

            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _config.ApiKey);

            if (string.IsNullOrEmpty(email))
            {
                throw new PassageException(string.Format(CultureInfo.InvariantCulture, "Email required."));
            }

            var content = new { email = email };
            var jsonContent = JsonSerializer.Serialize(content);
            using (StringContent stringContent = new StringContent(jsonContent, UnicodeEncoding.UTF8, "application/json"))
            {
                using var response = await client.PostAsync(uri, stringContent, ct).ConfigureAwait(false);

                if (!response.IsSuccessStatusCode)
                {
                    throw new PassageException(string.Format(CultureInfo.InvariantCulture, "Failed to Create Admin. StatusCode: {0}, ReasonPhrase: {1}", response.StatusCode, response.ReasonPhrase), response);
                }

                var responseStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                var result = await JsonSerializer.DeserializeAsync<AdminMember>(responseStream, options, ct).ConfigureAwait(false) ?? new();
                return result;
            }
        }
        catch (Exception)
        {
            throw;
        }
    }

    /// <summary>
    /// Delete an admin.
    /// </summary>
    /// <returns></returns>
    public async Task DeleteAdminAsync(string adminId, CancellationToken ct = default)
    {
        // GET https://api.passage.id/v1/apps/{app_id}/
        // Authorization: Bearer (api_key/auth_token)
        // 200/401/404/500

        try
        {
            var uri = new Uri($"https://api.passage.id/v1/apps/{_config.AppId}/admins/{adminId}");
            using var client = _httpClientFactory.CreateClient(PassageConsts.NamedClient);

            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _config.ApiKey);

            using var response = await client.DeleteAsync(uri, ct).ConfigureAwait(false);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                throw new PassageException(string.Format(CultureInfo.InvariantCulture, "Passage app Admin with ID \"{0}\" does not exist", adminId), response);
            }

            if (!response.IsSuccessStatusCode)
            {
                throw new PassageException(string.Format(CultureInfo.InvariantCulture, "Failed to Delete Admin. StatusCode: {0}, ReasonPhrase: {1}", response.StatusCode, response.ReasonPhrase), response);
            }


        }
        catch (Exception)
        {
            throw;
        }
    }


    /*************************/
    /*******   Events  *******/
    /*************************/

    /// <summary>
    /// List admins for an application.
    /// </summary>
    /// <param name="query">Cancellation token</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>The app</returns>
    /// <exception cref="PassageException"></exception>
    public async Task<IEnumerable<PaginatedEvent?>> GetPaginatedEventsAsync(PassagePaginatedEventsQuery query, CancellationToken ct = default)
    {
        // GET https://api.passage.id/v1/apps/{app_id}/events/
        // Authorization: Bearer (api_key)
        // 200/400/401/500

        string queryString = string.Empty;
        if (query != null)
        {
            var queryBuilder = HttpUtility.ParseQueryString(string.Empty);
            queryBuilder["page"] = $"{query.Page}";
            queryBuilder["limit"] = $"{query.Limit ?? 500}";
            queryBuilder["created_before"] = query.CreatedBefore.HasValue ? $"{((DateTimeOffset)query.CreatedBefore.Value).ToUnixTimeSeconds()}" : string.Empty;
            queryBuilder["order_by"] = query.OrderBy ?? string.Empty;
            queryBuilder["identifier"] = query.Identifier ?? string.Empty;
            queryBuilder["id"] = query.Id ?? string.Empty;
            queryBuilder["user_id"] = query.UserId ?? string.Empty;
            queryBuilder["ip_addr"] = query.IpAddress ?? string.Empty;
            queryBuilder["user_agent"] = query.UserAgent ?? string.Empty;
            queryBuilder["created_at"] = query.CreatedAt?.ToString("u") ?? string.Empty;
            queryBuilder["type"] = query.PaginatedEventType?.ToString() ?? string.Empty;
            queryString = queryBuilder.ToString();
        }

        var uri = new UriBuilder($"https://api.passage.id/v1/apps/{_config.AppId}/events/");
        uri.Query = queryString;
        using var client = _httpClientFactory.CreateClient(PassageConsts.NamedClient);

        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _config.ApiKey);

        using var response = await client.GetAsync(uri.ToString(), ct).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
        {
            throw new PassageException(string.Format(CultureInfo.InvariantCulture, "Failed to get Passge Pagination Events. StatusCode: {0}, ReasonPhrase: {1}", response.StatusCode, response.ReasonPhrase), response);
        }

        var responseStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        var appBody = await JsonSerializer.DeserializeAsync<PassagePaginatedEvents>(responseStream, options, ct).ConfigureAwait(false) ?? new();
        return appBody.Events.AsEnumerable();
    }


    /*****************************/
    /*******   Magic Link  *******/
    /*****************************/

    /// <summary>
    /// Create a new application. If not authenticated, the app is created in test
    /// mode and not assigned to a specific user.
    /// </summary>
    /// <param name="magiclink"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<CreateMagicLink?> CreateMagicLinkAsync(CreateMagicLink magiclink, CancellationToken ct = default)
    {
        // POST https://api.passage.id/v1/apps/{app_id}/magic-links/
        // Authorization: Bearer (key/jwt)
        // 200/400/401/409/500

        try
        {
            var uri = new Uri($"https://api.passage.id/v1/apps/");
            using var client = _httpClientFactory.CreateClient(PassageConsts.NamedClient);

            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _config.ApiKey);

            if (magiclink == null)
            {
                throw new PassageException(string.Format(CultureInfo.InvariantCulture, "Passage Magic link must have content"));
            }

            var jsonContent = JsonSerializer.Serialize(magiclink);
            using (StringContent stringContent = new StringContent(jsonContent, UnicodeEncoding.UTF8, "application/json"))
            {
                using var response = await client.PostAsync(uri, stringContent, ct).ConfigureAwait(false);

                if (!response.IsSuccessStatusCode)
                {
                    throw new PassageException(string.Format(CultureInfo.InvariantCulture, "Failed to Create Passage App. StatusCode: {0}, ReasonPhrase: {1}", response.StatusCode, response.ReasonPhrase), response);
                }

                var responseStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                var result = await JsonSerializer.DeserializeAsync<CreateMagicLink>(responseStream, options, ct).ConfigureAwait(false) ?? new();
                return result;
            }
        }
        catch (Exception)
        {
            throw;
        }
    }
}
