using System.Net;
using Mochi.Utils;

namespace Mochi.StreamKit.Twitch.Rest;

public class RequestQueue
{
    private readonly SemaphoreSlim _lock = new(1, 1);
    private RateLimitInfo? _rateLimitInfo;

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