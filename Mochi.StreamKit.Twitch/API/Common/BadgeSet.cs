using System.Text.Json.Serialization;

namespace Mochi.StreamKit.Twitch.API;

public class BadgeSet
{
    [JsonPropertyName("set_id")]
    public string SetId { get; set; }
    
    [JsonPropertyName("versions")]
    public List<BadgeVersion> Versions { get; set; }
}