using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Security.Authentication;
using System.Text;
using System.Text.Json;
using System.Web;
using Mochi.Irc;
using Mochi.StreamKit.Twitch.API;
using Mochi.StreamKit.Twitch.Chat;
using Mochi.StreamKit.Twitch.Entities;
using Mochi.StreamKit.Twitch.OAuth2;
using Mochi.Texts;
using Mochi.Utils;
using Emote = Mochi.StreamKit.Twitch.Entities.Emote;
using User = Mochi.StreamKit.Twitch.API.User;

namespace Mochi.StreamKit.Twitch.Rest;

public class TwitchRestApiClient : IDisposable
{
    private readonly TwitchClient _client;
    private readonly HttpClient _httpClient;
    private readonly RequestQueue _requestQueue;
    private readonly BadgeStore _badges;
    public string? AccessToken { get; private set; }
    private Credential? _credential;
    public CredentialType CredentialType { get; private set; }
    public Entities.User CurrentUser { get; private set; } = null!;
    private bool _disposed;

    public TwitchRestApiClient(TwitchClient client)
    {
        _client = client;
        _requestQueue = new RequestQueue(this);
        _badges = new BadgeStore(this);
        _httpClient = new HttpClient(new HttpClientHandler
        {
            UseCookies = false
        });

        var userAgent = _httpClient.DefaultRequestHeaders.UserAgent;
        userAgent.Clear();
        userAgent.Add(new ProductInfoHeaderValue("KaKusaBot", "1.0"));

        _ = RunUserQueryLoopAsync();
    }

    public async Task ReLoginAsync()
    {
        if (_credential == null)
        {
            throw new Exception("The client must have successfully logged in before re-login.");
        }
        
        await LoginAsync(_credential);
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
            _credential = credential;
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
        
        StoreLoginInfo(type, clientId, token);
        _credential = credential;

        _ = Task.Run(async () =>
        {
            var userId = validated.UserId;
            CurrentUser = (await GetUserByIdAsync(userId))!;
        });
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

    private void StoreLoginInfo(CredentialType type, string clientId, string token)
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

        var endpoint = endpointExpr.Compile()();
        Logger.Verbose(TranslateText.Of("Sending %s request to %s...")
            .AddWith(Component.Literal(method.Method).SetColor(TextColor.Aqua))
            .AddWith(Component.Literal($"/{endpoint}").SetColor(TextColor.Green))
        );
        var request = new RestRequest(this, method, endpoint, payload, options);
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

    public async Task<Badge?> GetBadgeAsync(PartialBadge badge) =>
        await _badges.GetBadgeAsync(badge);
    
    private readonly SemaphoreSlim _usersLock = new(1, 1);
    private readonly ConcurrentQueue<string> _userNameQueue = new();
    private readonly ConcurrentQueue<string> _userIdQueue = new();
    private readonly ConcurrentQueue<string> _userNameDoneQueue = new();
    private readonly ConcurrentQueue<string> _userIdDoneQueue = new();

    private async Task RunUserQueryLoopAsync()
    {
        await Task.Yield();
        while (!_disposed)
        {
            try
            {
                SpinWait.SpinUntil(() => !_userNameQueue.IsEmpty || !_userIdQueue.IsEmpty);

                var ids = new List<string>();
                var names = new List<string>();
                {
                    while (_userIdQueue.TryDequeue(out var id)) 
                        ids.Add(id);
                    while (_userNameQueue.TryDequeue(out var name))
                        names.Add(name);
                }

                var toFetch = ids.ToHashSet().Where(x => _client.GetCachedUser(x)?.IsValidCache() != true).Chunk(100);
                var results = new List<DataList<User>>();
                foreach (var arr in toFetch)
                {
                    var models = await InternalGetUsersByIdAsync(arr);
                    results.Add(models);
                }

                toFetch = names.ToHashSet()
                    .Where(x => _client.GetCachedUserByName(x)?.IsValidCache() != true)
                    .Where(x => results.SelectMany(r => r.Data).All(m => m.Login != x))
                    .Chunk(100);
                foreach (var arr in toFetch)
                {
                    var models = await InternalGetUsersByLoginNameAsync(arr);
                    results.Add(models);
                }

                foreach (var model in results.SelectMany(x => x.Data))
                {
                    AddOrUpdateUser(model);
                }

                foreach (var id in ids)
                    _userIdDoneQueue.Enqueue(id);
                foreach (var name in names)
                    _userNameDoneQueue.Enqueue(name);
            }
            catch (Exception ex)
            {
                Logger.Warn(ex);
            }
        }
    }

    public async Task<GlobalUser?> GetUserByNameAsync(string name)
    {
        var cached = _client.GetCachedUserByName(name);
        if (cached != null && cached.IsValidCache())
        {
            return cached;
        }

        _userNameQueue.Enqueue(name);
        await _usersLock.WaitAsync();
        try
        {
            await Task.Yield();
            SpinWait.SpinUntil(() => _userNameDoneQueue.TryPeek(out var p) && p == name);
            _userNameDoneQueue.TryDequeue(out _);
            return _client.GetCachedUserByName(name);
        }
        finally
        {
            _usersLock.Release();
        }
    }
    
    public async Task<GlobalUser?> GetUserByIdAsync(string id)
    {
        var cached = _client.GetCachedUser(id);
        if (cached != null && cached.IsValidCache())
        {
            return cached;
        }
        
        _userIdQueue.Enqueue(id);
        await _usersLock.WaitAsync();
        try
        {
            await Task.Yield();
            SpinWait.SpinUntil(() => _userIdDoneQueue.TryPeek(out var p) && p == id);
            _userIdDoneQueue.TryDequeue(out _);
            return _client.GetCachedUser(id);
        }
        finally
        {
            _usersLock.Release();
        }
    }
    
    public async Task<List<GlobalUser>> GetUsersByNameAsync(params string[] names)
    {
        var cached = names
            .Select(x => _client.GetCachedUserByName(x)).OfType<GlobalUser>()
            .Where(x => x.IsValidCache())
            .ToList();
        if (cached.Count == names.Length) return cached;

        var newNames = names.Where(x => cached.All(u => u.Name != x)).ToList();
        foreach (var name in newNames)
        {
            _userNameQueue.Enqueue(name);
        }
        
        await _usersLock.WaitAsync();
        try
        {
            var results = new List<GlobalUser>();
            foreach (var name in newNames)
            {
                SpinWait.SpinUntil(() => _userNameDoneQueue.TryPeek(out var p) && p == name);
                _userNameDoneQueue.TryDequeue(out _);
                var user = _client.GetCachedUserByName(name);
                if (user != null) results.Add(user);
            }

            return cached.Concat(results).ToList();
        }
        finally
        {
            _usersLock.Release();
        }
    }
    
    public async Task<List<GlobalUser>> GetUsersByIdAsync(params string[] ids)
    {
        var cached = ids
            .Select(x => _client.GetCachedUser(x)).OfType<GlobalUser>()
            .Where(x => x.IsValidCache())
            .ToList();
        if (cached.Count == ids.Length) return cached;

        var newIds = ids.Where(x => cached.All(u => u.Id != x)).ToList();
        foreach (var id in newIds)
        {
            _userIdQueue.Enqueue(id);
        }
        
        await _usersLock.WaitAsync();
        try
        {
            var results = new List<GlobalUser>();
            foreach (var id in newIds)
            {
                SpinWait.SpinUntil(() => _userIdDoneQueue.TryPeek(out var p) && p == id);
                _userIdDoneQueue.TryDequeue(out _);
                var user = _client.GetCachedUser(id);
                if (user != null) results.Add(user);
            }

            return cached.Concat(results).ToList();
        }
        finally
        {
            _usersLock.Release();
        }
    }
    
    public async Task<List<EmoteSet>> GetEmoteSetsAsync(params string[] ids)
    {
        var cached = ids
            .Select(x => _client.GetEmoteSetFromCache(x)).OfType<EmoteSet>()
            .Where(x => x.IsValidCache())
            .ToList();

        var toFetch = ids.Where(x => cached.All(u => u.Id != x)).Chunk(25).ToList();
        var results = new List<DataListWithTemplate<EmoteSetEntry>>();
        foreach (var arr in toFetch)
        {
            var models = await InternalGetEmoteSetsAsync(arr);
            results.Add(models);
        }

        var map = new ConcurrentDictionary<string, EmoteSet>();
        foreach (var result in results)
        {
            foreach (var entry in result.Data)
            {
                var emoteSet = map.GetOrAdd(entry.EmoteSetId, x =>
                {
                    var val = _client.GetOrCreateEmoteSet(x);
                    val.Emotes.Clear();
                    return val;
                });
                
                var emote = Emote.Create(_client, entry.Id, emoteSet, entry);
                emote.Template = result.Template;
                emoteSet.Emotes.Add(emote);
            }
        }

        foreach (var removedId in toFetch.SelectMany(x => x).Where(x => !map.ContainsKey(x)))
        {
            _client.RemoveEmoteSetFromCache(removedId);   
        }

        foreach (var emoteSet in map.Values)
        {
            emoteSet.Update();
        }
        
        return cached.Concat(map.Values).ToList();
    }

    public async Task<EmoteSet?> GetEmoteSetAsync(string id)
    {
        var cached = _client.GetEmoteSetFromCache(id);
        if (cached != null && cached.IsValidCache())
        {
            return cached;
        }
        
        var result = await InternalGetEmoteSetsAsync(id);
        if (!result.Data.Any())
        {
            _client.RemoveEmoteSetFromCache(id);
            return null;
        }
        
        var emoteSet = _client.GetOrCreateEmoteSet(id);
        emoteSet.Emotes.Clear();
        
        foreach (var model in result.Data)
        {
            var emote = Emote.Create(_client, model.Id, emoteSet, model);
            emote.Template = result.Template;
            emoteSet.Emotes.Add(emote);
        }
        
        emoteSet.Update();
        return emoteSet;
    }
    
    private GlobalUser AddOrUpdateUser(User user)
    {
        var id = user.Id;
        var result = _client.State.GetOrAddUser(id, x => GlobalUser.Create(_client, x, user));
        result.Update(user);
        return result;
    }
    
    
    internal async Task<DataList<User>> GetUsersAsync(IEnumerable<string> ids, IEnumerable<string> logins)
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
        return await SendAsync<DataList<User>>(HttpMethod.Get, () => endpoint);
    }
    
    internal GlobalUser AddOrUpdateUserByIrc(IrcMessage model)
    {
        var id = model.Tags["user-id"]!.RawValue;
        var result = _client.State.GetOrAddUser(id, x => GlobalUser.Create(_client, x, model));
        result.Update(model);
        return result;
    }

    internal Task<DataList<User>> InternalGetUsersByIdAsync(params string[] ids) =>
        GetUsersAsync(ids, Array.Empty<string>());
    
    internal Task<DataList<User>> InternalGetUsersByLoginNameAsync(params string[] ids) =>
        GetUsersAsync(Array.Empty<string>(), ids);
    
    internal async Task<DataList<BadgeSet>> InternalGetGlobalChatBadgesAsync() => 
        await SendAsync<DataList<BadgeSet>>(HttpMethod.Get, () => "chat/badges/global");

    internal async Task<DataList<BadgeSet>> InternalGetChannelChatBadgesAsync(string broadcasterId) => 
        await SendAsync<DataList<BadgeSet>>(HttpMethod.Get, () => $"chat/badges?broadcaster_id={broadcasterId}");
    
    internal async Task<DataListWithTemplate<EmoteSetEntry>> InternalGetEmoteSetsAsync(params string[] ids)
    {
        var idsList = ids.ToList();
        if (!idsList.Any())
            throw new Exception("Lookup cannot be empty.");
        if (idsList.Count > 25)
            throw new Exception("Total lookup amount cannot exceed 25.");

        var endpointBuilder = new StringBuilder();
        var idsQuery = string.Join('&', idsList.Select(x => $"emote_set_id={x}")).Trim();
        endpointBuilder.Append("chat/emotes/set?");
        endpointBuilder.Append(idsQuery);

        var endpoint = endpointBuilder.ToString();
        return await SendAsync<DataListWithTemplate<EmoteSetEntry>>(HttpMethod.Get, () => endpoint);
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        _httpClient.Dispose();
    }
}