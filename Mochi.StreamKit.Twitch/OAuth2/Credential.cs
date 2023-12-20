namespace Mochi.StreamKit.Twitch.OAuth2;

public class Credential
{
    public CredentialType Type { get; set; }
    public string ClientId { get; set; } = "";
    public string ClientSecret { get; set; } = "";

    public Credential()
    {
        
    }

    public Credential(string clientId, string clientSecret, CredentialType type = CredentialType.User)
    {
        Type = type;
        ClientId = clientId;
        ClientSecret = clientSecret;
    }
    
    public string? RefreshToken { get; set; }
    public string? AccessToken { get; set; }
    public DateTimeOffset? ExpiresAt { get; set; }
}