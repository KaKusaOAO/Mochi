using System.Collections;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Security.Authentication;
using System.Text;
using System.Text.Json;
using System.Web;
using Mochi.StreamKit.Twitch.API;
using Mochi.StreamKit.Twitch.OAuth2;
using Mochi.StreamKit.Twitch.Rest;
using Mochi.Utils;

namespace Mochi.StreamKit.Twitch;

public class TwitchRestApiClient
{
    private readonly TwitchClient _client;
    private readonly HttpClient _httpClient;
    private readonly RequestQueue _requestQueue = new();
    public string? AccessToken { get; private set; }
    public CredentialType CredentialType { get; private set; }
    public User CurrentUser { get; private set; } = null!;
    public BadgeStore Badges { get; }

    public TwitchRestApiClient(TwitchClient client)
    {
        _client = client;
        Badges = new BadgeStore(this);
        _httpClient = new HttpClient(new HttpClientHandler
        {
            UseCookies = false
        });
    }

    public async Task LoginAsync(Credential credential)
    {
        var loggedIn = false;
        
        if (credential.AccessToken != null)
        {
            // Validate the token then login.
            try
            {
                Logger.Info("Validating the existing OAuth token...");
                await ValidateTokenAsync(credential);
                loggedIn = true;
            }
            catch (AuthenticationException)
            {
                Logger.Warn("Invalid OAuth2 token!");
                if (credential.RefreshToken != null)
                {
                    try
                    {
                        Logger.Info("Refresh token found! Attempting to get a new access token...");
                        await RefreshTokenAndLoginAsync(credential);
                        loggedIn = true;
                    }
                    catch (AuthenticationException)
                    {
                        // Fallback to login procedure
                    }
                }
            }
        }

        if (!loggedIn)
        {
            Logger.Info("Starting login sequence...");
            await StartLoginSequenceAsync(credential);
        }
        else
        {
            Logger.Info("Logged in!");
        }
    }

    private async Task StartLoginSequenceAsync(Credential credential)
    {
        var type = credential.Type;
        switch (type)
        {
            case CredentialType.App:
                await LoginAppAsync(credential);
                break;
            case CredentialType.Unspecified:
            case CredentialType.User:
            default:
                await LoginUserAsync(credential);
                break;
        }
    }

    private async Task LoginAppAsync(Credential credential)
    {
        await InternalLoginAsync(credential);
    }
    
    private async Task RefreshTokenAndLoginAsync(Credential credential)
    {
        var clientId = credential.ClientId;
        var secret = credential.ClientSecret;
        var refreshToken = credential.RefreshToken!;
        
        var request = new HttpRequestMessage(HttpMethod.Post, TwitchConfig.OAuth2TokenUrl);
        var form = new Dictionary<string, string>
        {
            ["client_id"] = clientId,
            ["client_secret"] = secret,
            ["grant_type"] = "refresh_token",
            ["refresh_token"] = refreshToken
        };
        
        request.Content = new FormUrlEncodedContent(form);
        
        var response = await _httpClient.SendAsync(request);
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception("Failed to login.");
        }

        var payload = await JsonSerializer.DeserializeAsync<OAuth2Response>(await response.Content.ReadAsStreamAsync());
        var token = payload!.AccessToken;
        credential.AccessToken = token;
        credential.RefreshToken = payload.RefreshToken;
        credential.ExpiresAt = DateTimeOffset.Now + TimeSpan.FromSeconds(payload.ExpiresIn);

        await ValidateTokenAsync(credential);
    }

    private async Task InternalLoginAsync(Credential credential, string? code = null)
    {
        var clientId = credential.ClientId;
        var secret = credential.ClientSecret;
        if (code != null && credential.Type != CredentialType.User)
        {
            throw new ArgumentException("OAuth Code should not be defined if the credential type is not User.");
        }
        
        var request = new HttpRequestMessage(HttpMethod.Post, TwitchConfig.OAuth2TokenUrl);
        var form = new Dictionary<string, string>
        {
            ["client_id"] = clientId,
            ["client_secret"] = secret,
            ["grant_type"] = code == null ? "client_credentials" : "authorization_code"
        };
        if (code != null)
        {
            form.Add("code", code);
            form.Add("redirect_uri", "http://localhost:35918");
        }
        
        request.Content = new FormUrlEncodedContent(form);
        
        var response = await _httpClient.SendAsync(request);
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception("Failed to login.");
        }

        var payload = await JsonSerializer.DeserializeAsync<OAuth2Response>(await response.Content.ReadAsStreamAsync());
        var token = payload!.AccessToken;
        credential.AccessToken = token;
        credential.RefreshToken = payload.RefreshToken;
        credential.ExpiresAt = DateTimeOffset.Now + TimeSpan.FromSeconds(payload.ExpiresIn);
        await ValidateTokenAsync(credential);
    }

    public async Task ValidateTokenAsync(Credential credential)
    {
        var type = credential.Type;
        var clientId = credential.ClientId;
        var token = credential.AccessToken!;
        
        // Validate the token
        var request = new HttpRequestMessage(HttpMethod.Get, TwitchConfig.OAuth2ValidateUrl);
        request.Headers.Authorization = new AuthenticationHeaderValue("OAuth", token);
        request.Content = new StringContent("");
        
        var response = await _httpClient.SendAsync(request);
        if (!response.IsSuccessStatusCode)
        {
            throw new AuthenticationException("The given OAuth token is invalidated.");
        }

        var json = await response.Content.ReadAsStringAsync();
        var validated = JsonSerializer.Deserialize<OAuth2ValidateResponse>(json)!;
        credential.ExpiresAt = DateTimeOffset.Now + TimeSpan.FromSeconds(validated.ExpiresIn);
        
        var userId = validated!.UserId;
        Login(type, clientId, token);

        var userList = await GetUsersByIdAsync(userId);
        CurrentUser = userList.Data.FirstOrDefault();
    }

    public List<Scope> GetScopes()
    {
        return new List<Scope>
        {
            Scope.ChatEdit,
            Scope.ChatRead
        };
    }
    
    private async Task LoginUserAsync(Credential credential)
    {
        if (credential.Type != CredentialType.User)
            throw new ArgumentException("Wrong credential type");
        
        void OpenUrl(string url)
        {
            try
            {
                Process.Start(url);
            }
            catch
            {
                // hack because of this: https://github.com/dotnet/corefx/issues/10361
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    // url = url.Replace("&", "^&");
                    Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    Process.Start("xdg-open", url);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    Process.Start("open", url);
                }
                else
                {
                    throw;
                }
            }
        }
        OpenUrl($"https://id.twitch.tv/oauth2/authorize?response_type=code&client_id={credential.ClientId}" +
                $"&redirect_uri=http://localhost:35918&scope=" +
                string.Join('+', GetScopes().Select(x => x.FullPath).Select(Uri.EscapeDataString)));

        var server = new HttpListener();
        server.Prefixes.Add("http://localhost:35918/");
        server.Start();

        var context = await server.GetContextAsync();
        context.Response.Close();
        
        var query = HttpUtility.ParseQueryString(context.Request.Url!.Query);
        var code = query["code"]!;
        await InternalLoginAsync(credential, code);
    }

    private void Login(CredentialType type, string clientId, string token)
    {
        CredentialType = type;
        AccessToken = token;
        SetHeader("Authorization", $"Bearer {token}");
        SetHeader("Client-Id", clientId);
    }
    
    public void SetHeader(string key, string? value)
    {
        _httpClient.DefaultRequestHeaders.Remove(key);
        if (value == null) return;
        _httpClient.DefaultRequestHeaders.Add(key, value);
    }

    public void SetHeaderValues(string key, IEnumerable<string>? value)
    {
        _httpClient.DefaultRequestHeaders.Remove(key);
        if (value == null) return;
        _httpClient.DefaultRequestHeaders.Add(key, value);
    }

    public async Task<T> SendAsync<T>(HttpMethod method, Expression<Func<string>> endpointExpr, 
        string? payload = null, IDictionary<string, string>? form = null,
        RequestOptions? options = null)
    {
        options ??= new RequestOptions();

        var request = new RestRequest(this, method, endpointExpr.Compile()(), payload, options);
        if (form != null)
        {
            foreach (var (key, value) in form)
            {
                request.Form.Add(key, value);
            }
        }

        var stream = await SendInternalAsync(request);
        return (await JsonSerializer.DeserializeAsync<T>(stream))!;
    }

    public async Task<Stream> SendInternalAsync(RestRequest request)
    {
        return await _requestQueue.SendAsync(request);
    }
    
    public async Task<RestResponse> SendAsync(HttpMethod method, string endpoint, RequestOptions options, 
        string? payload = null, IDictionary<string, string>? form = null)
    {
        var uri = Path.Combine(TwitchConfig.ApiUrl, endpoint);
        var request = new HttpRequestMessage(method, uri);
        if (payload != null)
        {
            request.Content = new StringContent(payload, Encoding.UTF8);
        } else if (form != null)
        {
            request.Content = new FormUrlEncodedContent(form);
        }

        return await SendInternalAsync(request, options);
    }

    private async Task<RestResponse> SendInternalAsync(HttpRequestMessage request, RequestOptions options)
    {
        var headers = options.Headers;
        foreach (var (key, value) in headers)
        {
            request.Headers.Add(key, value);
        }    
        
        var response = await _httpClient.SendAsync(request);
        return new RestResponse(response, options);
    }
    
    public async Task<ListPayload<User>> GetUsersAsync(IEnumerable<string> ids, IEnumerable<string> logins)
    {
        var idsList = ids.ToList();
        var loginsList = logins.ToList();
        if (idsList.Count + loginsList.Count > 100)
        {
            throw new Exception("Total lookup amount cannot exceed 100.");
        }

        var endpointBuilder = new StringBuilder();
        var idsQuery = string.Join('&', idsList.Select(x => $"id={x}")).Trim();
        var loginsQuery = string.Join('&', loginsList.Select(x => $"login={x}")).Trim();
        
        var hasIds = !string.IsNullOrEmpty(idsQuery);
        var hasLogins = !string.IsNullOrEmpty(loginsQuery);

        endpointBuilder.Append("users?");
        if (hasIds)
        {
            endpointBuilder.Append(idsQuery);
            if (hasLogins) endpointBuilder.Append('&');
        }

        if (hasLogins)
        {
            endpointBuilder.Append(loginsQuery);
        }

        var endpoint = endpointBuilder.ToString();
        return await SendAsync<ListPayload<User>>(HttpMethod.Get, () => endpoint);
    }

    public Task<ListPayload<User>> GetUsersByIdAsync(params string[] ids) =>
        GetUsersAsync(ids, Array.Empty<string>());
    
    public Task<ListPayload<User>> GetUsersByLoginNameAsync(params string[] ids) =>
        GetUsersAsync(Array.Empty<string>(), ids);
    
    public async Task<ListPayload<BadgeSet>> GetGlobalChatBadgesAsync() => 
        await SendAsync<ListPayload<BadgeSet>>(HttpMethod.Get, () => "chat/badges/global");

    public async Task<ListPayload<BadgeSet>> GetChannelChatBadgesAsync(string broadcasterId) => 
        await SendAsync<ListPayload<BadgeSet>>(HttpMethod.Get, () => $"chat/badges?broadcaster_id={broadcasterId}");
}