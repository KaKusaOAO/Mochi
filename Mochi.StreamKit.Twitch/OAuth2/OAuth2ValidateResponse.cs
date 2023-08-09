using System.Text.Json.Serialization;

namespace Mochi.StreamKit.Twitch.OAuth2;

public class OAuth2ValidateResponse
{
    [JsonPropertyName("client_id")]
    public string ClientId { get; set; }
    
    [JsonPropertyName("user_id")]
    public string UserId { get; set; }
    
    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; }
    
    [JsonPropertyName("login")]
    public string Login { get; set; }
}