using System.Net;

namespace Mochi.StreamKit.Twitch.Rest;

public class RestResponse
{
    public HttpStatusCode StatusCode { get; }
    public Dictionary<string, string> Headers { get; }
    public Stream? Stream { get; }
    
    public RateLimitInfo RateLimitInfo { get; }

    public RestResponse(HttpResponseMessage response, RequestOptions options)
    {
        StatusCode = response.StatusCode;
        Headers = response.Headers.ToDictionary(x => x.Key, x => x.Value.FirstOrDefault()!);
        Stream = !options.HeaderOnly && response.IsSuccessStatusCode ? response.Content.ReadAsStream() : null;
        RateLimitInfo = RateLimitInfo.CreateFromHeaders(Headers);
    }

    public RestResponse(HttpStatusCode statusCode, Dictionary<string, string> headers, Stream? stream)
    {
        StatusCode = statusCode;
        Headers = headers;
        Stream = stream;
        RateLimitInfo = RateLimitInfo.CreateFromHeaders(Headers);
    }

    public static async Task<RestResponse> CreateAsync(HttpResponseMessage response, RequestOptions options)
    {
        var statusCode = response.StatusCode;
        var headers = response.Headers.ToDictionary(x => x.Key, x => x.Value.FirstOrDefault()!);
        var stream = !options.HeaderOnly && response.IsSuccessStatusCode ? await response.Content.ReadAsStreamAsync() : null;
        return new RestResponse(statusCode, headers, stream);
    }
}

public class RateLimitInfo
{
    public int Limit { get; }
    public int Remaining { get; }
    public DateTimeOffset ResetAt { get; }

    public RateLimitInfo(int limit, int remaining, DateTimeOffset resetAt)
    {
        Limit = limit;
        Remaining = remaining;
        ResetAt = resetAt;
    }

    public static RateLimitInfo CreateFromHeaders(Dictionary<string, string> headers)
    {
        var limit = int.Parse(headers["Ratelimit-Limit"]);
        var remaining = int.Parse(headers["Ratelimit-Remaining"]);
        var resetAt = DateTimeOffset.FromUnixTimeSeconds(long.Parse(headers["Ratelimit-Reset"]));
        return new RateLimitInfo(limit, remaining, resetAt);
    }
}