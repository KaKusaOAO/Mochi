using System.Text.Json.Serialization;

namespace Mochi.StreamKit.Twitch.API;

public class BadgeVersion
{
    [JsonPropertyName("id")]
    public string Id { get; set; }
    [JsonPropertyName("image_url_1x")]
    public string ImageUrlSmall { get; set; }
    [JsonPropertyName("image_url_2x")]
    public string ImageUrlMedium { get; set; }
    [JsonPropertyName("image_url_4x")]
    public string ImageUrlLarge { get; set; }
    [JsonPropertyName("title")]
    public string Title { get; set; }
    [JsonPropertyName("description")]
    public string Description { get; set; }
}