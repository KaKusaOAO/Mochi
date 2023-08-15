using System.Text.Json.Serialization;

namespace Mochi.StreamKit.Twitch.API;

public class EmoteImages
{
    [JsonPropertyName("url_1x")]
    public string ImageSmall { get; set; }    
    [JsonPropertyName("url_2x")]
    public string ImageMedium { get; set; }    
    [JsonPropertyName("url_4x")]
    public string ImageLarge { get; set; }
}