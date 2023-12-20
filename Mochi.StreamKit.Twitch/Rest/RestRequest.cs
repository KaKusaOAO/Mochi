namespace Mochi.StreamKit.Twitch.Rest;

public class RestRequest
{
    public TwitchRestApiClient Client { get; }
    public HttpMethod Method { get; }
    public string Endpoint { get; }
    public string? Payload { get; }
    public Dictionary<string, string> Form { get; } = new();
    public RequestOptions Options { get; }

    public RestRequest(TwitchRestApiClient client, HttpMethod method, string endpoint, string? payload = null, RequestOptions? options = null)
    {
        Client = client;
        Method = method;
        Endpoint = endpoint;
        Payload = payload;
        Options = options ?? new RequestOptions();
    }

    public async Task<RestResponse> SendAsync()
    {
        return await Client.SendAsync(Method, Endpoint, Options, Payload, Form);
    }
}