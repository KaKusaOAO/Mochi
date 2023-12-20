using System.Text.Json.Serialization;
using Mochi.StreamKit.Twitch.API.Converters;
using Mochi.StreamKit.Twitch.Entities;

namespace Mochi.StreamKit.Twitch.API;

public class User
{
    [JsonPropertyName("id")]
    public string Id { get; set; }
    
    [JsonPropertyName("login")]
    public string Login { get; set; }
    
    [JsonPropertyName("display_name")]
    public string DisplayName { get; set; }
    
    [JsonPropertyName("type")]
    [JsonConverter(typeof(UserTypeConverter))]
    public UserType Type { get; set; }
    
    [JsonPropertyName("broadcaster_type")]
    [JsonConverter(typeof(BroadcasterTypeConverter))]
    public BroadcasterType BroadcasterType { get; set; }
    
    [JsonPropertyName("description")]
    public string Description { get; set; }
    
    [JsonPropertyName("profile_image_url")]
    public string ProfileImageUrl { get; set; }
    
    [JsonPropertyName("offline_image_url")]
    public string OfflineImageUrl { get; set; }
    
    [JsonPropertyName("email")]
    public string? Email { get; set; }
    
    [JsonPropertyName("created_at")]
    [JsonConverter(typeof(DateTimeOffsetConverter))]
    public DateTimeOffset CreatedAt { get; set; }
}