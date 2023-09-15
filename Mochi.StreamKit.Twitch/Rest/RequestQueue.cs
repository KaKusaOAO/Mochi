using System.Net;
using Microsoft.VisualBasic.CompilerServices;
using Mochi.Utils;

namespace Mochi.StreamKit.Twitch.Rest;

public class RequestQueue
{
    private readonly TwitchRestApiClient _client;
    private readonly SemaphoreSlim _lock = new(1, 1);
    private RateLimitInfo? _rateLimitInfo;
    private bool _isReLoggingIn;

    public RequestQueue(TwitchRestApiClient client)
    {
        _client = client;
    }
    
    public async Task<Stream> SendAsync(RestRequest request)
    {
        await _lock.WaitAsync();
        try
        {
            while (true)
            {
                if (_rateLimitInfo != null && _rateLimitInfo.Remaining - request.Options.ExpectedPointsCost < 0)
                {
                    var now = DateTimeOffset.Now;
                    var resetAt = _rateLimitInfo.ResetAt;
                    if (resetAt > now)
                    {
                        Logger.Verbose("Waiting for rate limit reset...");
                        await Task.Delay(resetAt - now);
                    }
                }

                // The rate limit should not reach here
                var response = await request.SendAsync();
                _rateLimitInfo = response.RateLimitInfo;

                if ((int) response.StatusCode is < 200 or >= 300)
                {
                    if (response.StatusCode == HttpStatusCode.TooManyRequests)
                    {
                        Logger.Warn("Being rate limited. Retrying after reset...");
                        continue;
                    }

                    if (response.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        Logger.Warn("Unauthorized! Re-logging in and retrying...");
                        await _client.ReLoginAsync();
                        continue;
                    }

                    throw new Exception();
                }

                return response.Stream!;
            }
        }
        finally
        {
            _lock.Release();
        }
    }
}