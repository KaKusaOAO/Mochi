using System.Text.Json.Serialization;
using Mochi.StreamKit.Twitch.API.Converters;

namespace Mochi.StreamKit.Twitch.API;

public class EmoteSetEntry : Emote
{
    [JsonPropertyName("emote_type")]
    [JsonConverter(typeof(EmoteTypeConverter))]
    public EmoteType EmoteType { get; set; }
    
    [JsonPropertyName("emote_set_id")]
    public string EmoteSetId { get; set; }
    
    [JsonPropertyName("owner_id")]
    public string OwnerId { get; set; }
}